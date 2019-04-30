using DynamicData;
using PlayStats.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace PlayStats.Models
{
    public class GameModel : GameModelBase
    {
        public GameModel(Guid id, IObservable<IChangeSet<PlayModel, Guid>> plays, IObservable<IChangeSet<LinkedGameModel, Guid>> linkedGames) : base(id)
        {
            IsGenuine = true;
            Rating = 0;
            DesireToPlay = 10;

            plays.ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _plays)
                .Subscribe();

            linkedGames
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _linkedGames)
                .Subscribe();

            plays.AutoRefresh(p => p.Duration)
                .ToCollection().Select(collection => collection.Select(p=>p.Duration.Ticks).Sum())
                .Select(TimeSpan.FromTicks)
                .ToPropertyEx(this, p => p.TotalTimePlayed);

            this.WhenAnyValue(p => p.TotalTimePlayed, p => p.PurchasePrice, p=>p.SellPrice)
                .Select(p => TotalTimePlayed.TotalHours > 0 ? (PurchasePrice - SellPrice) / TotalTimePlayed.TotalHours : 0)
                .ToPropertyEx(this, x => x.Value);
        }

        #region Basic properties

        [Reactive] public SoloMode SoloMode { get; set; }

        [Reactive] public int Rating { get; set; }

        [Reactive] public int DesireToPlay { get; set; }

        private readonly ReadOnlyObservableCollection<LinkedGameModel> _linkedGames;
        public ReadOnlyObservableCollection<LinkedGameModel> LinkedGames => _linkedGames;

        private readonly ReadOnlyObservableCollection<PlayModel> _plays;
        public ReadOnlyObservableCollection<PlayModel> Plays => _plays; 

        #endregion

        #region Calculated properties

        public double Value { [ObservableAsProperty] get; }

        public TimeSpan TotalTimePlayed { [ObservableAsProperty] get; }

        #endregion
    }
}