using Autofac;
using ReactiveUI;

namespace PlayStats.Services
{
    public interface IViewModelFactory
    {
        T Create<T>() where T : ReactiveObject;
    }

    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IContainer _container;

        public ViewModelFactory(IContainer container)
        {
            _container = container;
        }

        public T Create<T>() where T : ReactiveObject
        {
            return _container.Resolve<T>();
        }
    }
}
