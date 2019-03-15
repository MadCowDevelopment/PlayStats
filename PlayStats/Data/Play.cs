using System;

namespace PlayStats.Data
{
    public class Play : Entity
    {
        public DateTime Date { get; set; }

        public Guid GameId { get; set; }

        public TimeSpan Duration { get; set; }

        public string Comment { get; set; }

        public int PlayerCount { get; set; }
    }
}
