using DynamicData;
using PlayStats.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace PlayStats.UI
{
    public class GameGridViewModel : ReactiveObject
    {
        private readonly ReadOnlyObservableCollection<GameModel> _games;
        public IEnumerable<GameModel> Games => _games;

        public GameGridViewModel(IRepository repository)
        {
            repository.Games
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _games).Subscribe();
        }
    }
}