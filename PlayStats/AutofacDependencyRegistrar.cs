using Autofac;
using AutoMapper;
using PlayStats.Data;
using PlayStats.Models;
using PlayStats.Services;
using PlayStats.UI;
using PlayStats.UI.Dialogs.AppStartup;
using PlayStats.UI.Dialogs.MessageBox;
using PlayStats.UI.Dialogs.Update;
using PlayStats.UI.Tabs.AddGame;
using PlayStats.UI.Tabs.AddPlay;
using PlayStats.UI.Tabs.GameGrid;
using PlayStats.UI.Tabs.GameList;
using PlayStats.UI.Tabs.Home;

namespace PlayStats
{
    public class AutofacDependencyRegistrar
    {
        private readonly ContainerBuilder builder;

        private IContainer _container;

        public AutofacDependencyRegistrar()
        {
            builder = new ContainerBuilder();
            RegisterServices();
        }

        public IContainer GetContainer()
        {
            return _container;
        }

        private void RegisterServices()
        {
            // ViewModels - Tabs
            builder.RegisterType<MainWindowViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<HomeViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<AddPlayViewModel>().AsSelf();
            builder.RegisterType<AddGameViewModel>().AsSelf();
            builder.RegisterType<GameListViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<GameGridViewModel>().AsSelf().SingleInstance();

            // ViewModels - Dialogs
            builder.RegisterType<UpdateViewModel>().AsSelf();
            builder.RegisterType<ApplicationStartupViewModel>().AsSelf();
            builder.RegisterType<MessageBoxViewModel>().AsSelf();

            // Data
            builder.RegisterType<Repository>().As<IRepository>().SingleInstance();
            builder.RegisterType<LiteDbPlayAccessor>().As<IDataAccessor<PlayEntity>>();
            builder.RegisterType<LiteDbGameAccessor>().As<IDataAccessor<GameEntity>>();
            builder.RegisterType<LiteDbLinkedGameAccessor>().As<IDataAccessor<LinkedGameEntity>>();

            // Services
            builder.RegisterType<NotificationService>().As<INotificationService>();
            builder.RegisterType<ViewModelFactory>().As<IViewModelFactory>();
            builder.RegisterType<BggService>().As<IBggService>();
            builder.RegisterType<DialogService>().As<IDialogService>();
            builder.RegisterInstance(AutoMapperConfigurator.CreateMapper()).As<IMapper>();

            builder.Register(ctx => _container);
            _container = builder.Build();
        }
    }
}
