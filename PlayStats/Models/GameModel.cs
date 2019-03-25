using System;
using System.Collections.ObjectModel;
using PlayStats.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PlayStats.Models
{
    public class GameModel : GameModelBase
    {
        public GameModel(Guid id, ReadOnlyObservableCollection<PlayModel> plays, ReadOnlyObservableCollection<LinkedGameModel> linkedGames) : base(id)
        {
            LinkedGames = linkedGames;
            Plays = plays;
        }

        [Reactive] public SoloMode SoloMode { get; set; }

        [Reactive] public int Rating { get; set; }

        [Reactive] public int DesireToPlay { get; set; }

        public ReadOnlyObservableCollection<LinkedGameModel> LinkedGames { get; }

        public ReadOnlyObservableCollection<PlayModel> Plays { get; }
    }
}