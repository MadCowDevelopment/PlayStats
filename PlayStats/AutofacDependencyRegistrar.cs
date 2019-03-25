using Autofac;
using PlayStats.Data;
using PlayStats.Models;
using PlayStats.Services;
using PlayStats.UI;

namespace PlayStats
{
    public class AutofacDependencyRegistrar
    {
        protected ContainerBuilder builder;

        private IContainer _container;

        public AutofacDependencyRegistrar()
        {
            builder = new ContainerBuilder();
            RegisterServices();
        }

        public IContainer Build()
        {
            return _container;
        }

        private void RegisterServices()
        {
            // ViewModels
            builder.RegisterType<MainWindowViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<GameListViewModel>().AsSelf();

            // Data
            builder.RegisterType<Repository>().As<IRepository>().SingleInstance();
            builder.RegisterType<PlayAccessor>().As<IDataAccessor<PlayEntity>>();
            builder.RegisterType<GameAccessor>().As<IDataAccessor<GameEntity>>();
            builder.RegisterType<LinkedGameAccessor>().As<IDataAccessor<LinkedGameEntity>>();

            // Services
            builder.RegisterType<ViewModelFactory>().As<IViewModelFactory>();

            builder.Register(ctx => _container);
            _container = builder.Build();
        }
    }
}
