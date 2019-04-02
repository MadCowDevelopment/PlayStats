using Autofac;
using AutoMapper;
using PlayStats.Data;
using PlayStats.Models;
using PlayStats.Services;
using PlayStats.UI;

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
            // ViewModels
            builder.RegisterType<MainWindowViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<HomeViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<AddPlayViewModel>().AsSelf();
            builder.RegisterType<AddGameViewModel>().AsSelf();
            builder.RegisterType<GameListViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<GameGridViewModel>().AsSelf().SingleInstance();

            // Data
            builder.RegisterType<Repository>().As<IRepository>().SingleInstance();
            builder.RegisterType<PlayAccessor>().As<IDataAccessor<PlayEntity>>();
            builder.RegisterType<GameAccessor>().As<IDataAccessor<GameEntity>>();
            builder.RegisterType<LinkedGameAccessor>().As<IDataAccessor<LinkedGameEntity>>();

            // Services
            builder.RegisterType<NotificationService>().As<INotificationService>();
            builder.RegisterType<ViewModelFactory>().As<IViewModelFactory>();
            builder.RegisterInstance(AutoMapperConfigurator.CreateMapper()).As<IMapper>();

            builder.Register(ctx => _container);
            _container = builder.Build();
        }
    }
}
