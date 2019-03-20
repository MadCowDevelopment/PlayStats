using ReactiveUI;
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

        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => this.RaiseAndSetIfChanged(ref _date, value);
        }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get => _duration;
            set => this.RaiseAndSetIfChanged(ref _duration, value);
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set => this.RaiseAndSetIfChanged(ref _comment, value);
        }

        private int _playerCount;
        public int PlayerCount
        {
            get => _playerCount;
            set => this.RaiseAndSetIfChanged(ref _playerCount, value);
        }
    }
}