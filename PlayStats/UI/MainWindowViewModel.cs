using ReactiveUI;
using System.Windows;
using System.Windows.Input;

namespace PlayStats.UI
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel(GameListViewModel gameList)
        {
            GameList = gameList;

            Exit = ReactiveCommand.Create(() => { Application.Current.Shutdown(); });
        }

        private GameListViewModel _gameList;
        public GameListViewModel GameList
        {
            get => _gameList;
            set => this.RaiseAndSetIfChanged(ref _gameList, value);
        }

        public ICommand Exit { get; internal set; }
    }
}