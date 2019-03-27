using DynamicData;
using DynamicData.Aggregation;
using PlayStats.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace PlayStats.Models
{
    public class GameModel : GameModelBase
    {
        public GameModel(Guid id, IObservable<IChangeSet<PlayModel, Guid>> plays, IObservable<IChangeSet<LinkedGameModel, Guid>> linkedGames) : base(id)
        {
            plays.ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _plays)
                .Subscribe();

            linkedGames
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _linkedGames)
                .Subscribe();

            plays.AutoRefresh(p => p.Duration)
                .Sum(p => p.Duration.Ticks)
                .Select(TimeSpan.FromTicks)
                .ToPropertyEx(this, p => p.TotalTimePlayed);
                        
            this.WhenAnyValue(p => p.TotalTimePlayed, p=>p.PurchasePrice)
                .Select(p => TotalTimePlayed.TotalHours != 0 ? PurchasePrice / TotalTimePlayed.TotalHours : 0)
                .ToPropertyEx(this, x => x.Value);
        }

        [Reactive] public SoloMode SoloMode { get; set; }

        [Reactive] public int Rating { get; set; }

        [Reactive] public int DesireToPlay { get; set; }

        private readonly ReadOnlyObservableCollection<LinkedGameModel> _linkedGames;
        public ReadOnlyObservableCollection<LinkedGameModel> LinkedGames => _linkedGames;

        private readonly ReadOnlyObservableCollection<PlayModel> _plays;
        public ReadOnlyObservableCollection<PlayModel> Plays => _plays;

        #region Calculated properties

        public double Value { [ObservableAsProperty] get; }

        public TimeSpan TotalTimePlayed { [ObservableAsProperty] get; }

        #endregion
    }
}