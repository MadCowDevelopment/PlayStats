using ReactiveUI;

namespace PlayStats.UI
{
    public class MainWindowViewModel : ReactiveObject
    {
        public MainWindowViewModel(GameListViewModel gameList)
        {
            GameList = gameList;
        }

        private GameListViewModel _gameList;
        public GameListViewModel GameList
        {
            get => _gameList;
            set => this.RaiseAndSetIfChanged(ref _gameList, value);
        }
    }
}