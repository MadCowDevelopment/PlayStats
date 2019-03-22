using Autofac;
using PlayStats.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PlayStats.UI
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly IViewModelFactory _viewModelFactory;

        public MainWindowViewModel(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;

            _content = _viewModelFactory.Create<GameListViewModel>();

            Exit = ReactiveCommand.Create(() => { Application.Current.Shutdown(); });
        }

        private ObservableCollection<ITabViewModel> _tabs;

        public ObservableCollection<ITabViewModel> Tabs
        {
            get => _tabs;
            set => this.RaiseAndSetIfChanged(ref _tabs, value);
        }

        public ICommand Exit { get; internal set; }

        private ReactiveObject _content;
        public ReactiveObject Content
        {
            get => _content;
            private set => this.RaiseAndSetIfChanged(ref _content, value);
        }
    }
}