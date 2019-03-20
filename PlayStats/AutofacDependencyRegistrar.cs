using Autofac;
using PlayStats.Data;
using PlayStats.Models;
using PlayStats.UI;
using IContainer = Autofac.IContainer;

namespace PlayStats
{
    public class AutofacDependencyRegistrar
    {
        protected ContainerBuilder builder;

        public AutofacDependencyRegistrar()
        {
            builder = new ContainerBuilder();
            RegisterServices();
        }

        public IContainer Build()
        {
            return builder.Build();
        }

        private void RegisterServices()
        {
            builder.RegisterType<MainWindowViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<GameListViewModel>().AsSelf().SingleInstance();

            builder.RegisterType<Repository>().As<IRepository>().SingleInstance();
            builder.RegisterType<PlayAccessor>().As<IDataAccessor<PlayEntity>>();
            builder.RegisterType<GameAccessor>().As<IDataAccessor<GameEntity>>();
            builder.RegisterType<LinkedGameAccessor>().As<IDataAccessor<LinkedGameEntity>>();
        }
    }
}
