using Autofac;
using PlayStats.Models;
using PlayStats.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows;
using System.Windows.Input;

namespace PlayStats.UI
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly IRepository _repository;

        public MainWindowViewModel(IViewModelFactory viewModelFactory, IRepository repository)
        {
            _viewModelFactory = viewModelFactory;
            _repository = repository;

            _repository.Load().Wait(); // TODO: Load async

            Content = _viewModelFactory.Create<HomeViewModel>();

            ShowView = ReactiveCommand.Create<Type>(p => Content = _viewModelFactory.Create(p));

            Exit = ReactiveCommand.Create(() => { Application.Current.Shutdown(); });
        }

        public ICommand ShowView { get; }
        public ICommand Exit { get; }

        [Reactive] public ReactiveObject Content { get; private set; }
    }
}