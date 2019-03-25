using Autofac;
using ReactiveUI;
using System;

namespace PlayStats.Services
{
    public interface IViewModelFactory
    {
        T Create<T>() where T : ReactiveObject;

        ReactiveObject Create(Type type);
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

        public ReactiveObject Create(Type type)
        {
            return (ReactiveObject)_container.Resolve(type);
        }
    }
}
