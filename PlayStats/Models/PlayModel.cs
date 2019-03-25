using ReactiveUI.Fody.Helpers;
using System;

namespace PlayStats.Models
{
    public class PlayModel : Model
    {
        public PlayModel(Guid id, Guid gameId) : base(id)
        {
            GameId = gameId;
        }

        public Guid GameId { get; }

        [Reactive] public DateTime Date { get; set; }
        [Reactive] public TimeSpan Duration { get; set; }
        [Reactive] public string Comment { get; set; }
        [Reactive] public int PlayerCount { get; set; }
    }
}