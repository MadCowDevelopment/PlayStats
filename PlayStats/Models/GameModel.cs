using System;
using System.Collections.ObjectModel;
using PlayStats.Data;
using ReactiveUI;

namespace PlayStats.Models
{
    public class GameModel : GameModelBase
    {
        public GameModel(Guid id, ReadOnlyObservableCollection<PlayModel> plays, ReadOnlyObservableCollection<LinkedGameModel> linkedGames) : base(id)
        {
            LinkedGames = linkedGames;
            Plays = plays;
        }

        private SoloMode _soloMode;
        public SoloMode SoloMode
        {
            get => _soloMode;
            set => this.RaiseAndSetIfChanged(ref _soloMode, value);
        }

        private int _rating;
        public int Rating
        {
            get => _rating;
            set => this.RaiseAndSetIfChanged(ref _rating, value);
        }

        private int _desireToPlay;
        public int DesireToPlay
        {
            get => _desireToPlay;
            set => this.RaiseAndSetIfChanged(ref _desireToPlay, value);
        }

        public ReadOnlyObservableCollection<LinkedGameModel> LinkedGames { get; }

        public ReadOnlyObservableCollection<PlayModel> Plays { get; }
    }
}