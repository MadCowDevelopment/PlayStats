using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using PlayStats.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlayStats.Models
{
    public class GameModel : GameModelBase
    {
        public GameModel(Guid id, IObservable<IChangeSet<PlayModel, Guid>> plays, IObservable<IChangeSet<LinkedGameModel, Guid>> linkedGames) : base(id)
        {
            plays.ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _plays)
                .Subscribe();

            foreach(var play in plays.ToEnumerable())
            {
                var p = play;
            }

            linkedGames
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _linkedGames)
                .Subscribe();
                 
            var totalTimePlayed = plays.WhenValueChanged(p => p.Duration).Select(p =>
              {
                  TimeSpan total = new TimeSpan();
                  foreach (var play in Plays)
                  {
                      total += play.Duration;
                  }

                  return total;
              }).ToPropertyEx(this, x => x.TotalTimePlayed);

            this.WhenAny(p => p.TotalTimePlayed, p => TotalTimePlayed.TotalHours != 0 ? PurchasePrice / TotalTimePlayed.TotalHours : 0)
                .ToPropertyEx(this, x => x.Value);


            //var sumDisposable = entries.Connect()
            //  .Transform(x => x.TransferContext)
            //  .AutoRefresh(x => x.BytesTransferred)
            //  .Sum(x => x.BytesTransferred)
            //  .Subscribe(x => progress.Report(x));
        }

        [Reactive] public SoloMode SoloMode { get; set; }

        [Reactive] public int Rating { get; set; }

        [Reactive] public int DesireToPlay { get; set; }

        private ReadOnlyObservableCollection<LinkedGameModel> _linkedGames;
        public ReadOnlyObservableCollection<LinkedGameModel> LinkedGames => _linkedGames;

        private ReadOnlyObservableCollection<PlayModel> _plays;
        public ReadOnlyObservableCollection<PlayModel> Plays => _plays;

        #region Calculated properties

        public double Value { [ObservableAsProperty] get; }

        public TimeSpan TotalTimePlayed { [ObservableAsProperty] get; }

        #endregion
    }
}