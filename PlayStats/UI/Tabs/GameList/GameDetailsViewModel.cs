// MainWindow class derives off ReactiveWindow which implements the IViewFor<TViewModel>
// interface using a WPF DependencyProperty. We need this to use WhenActivated extension
// method that helps us handling View and ViewModel activation and deactivation.

using System;
using System.Diagnostics;
using System.Reactive;
using PlayStats.Models;
using ReactiveUI;

namespace PlayStats.UI.Tabs.GameList
{
    public class GameDetailsViewModel : ReactiveObject
    {
        private readonly GameModel _game;

        public GameDetailsViewModel(GameModel game)
        {
            OpenPage = ReactiveCommand.Create(() => { Process.Start(ProjectUrl.ToString()); });
            _game = game;
        }

        public string Name => _game.Name;
        public Uri ProjectUrl => new Uri("https://git.io/fAlfh");
        public Guid Id => _game.Id;

        // ReactiveCommand allows us to execute logic without exposing any of the 
        // implementation details with the View. The generic parameters are the 
        // input into the command and it's output. In our case we don't have any 
        // input or output so we use Unit which in Reactive speak means a void type.
        public ReactiveCommand<Unit, Unit> OpenPage { get; }
    }
}