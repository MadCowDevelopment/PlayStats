using System;

namespace PlayStats.Models
{
    public class LinkedGameModel : GameModelBase
    {
        public LinkedGameModel(Guid id, Guid gameId) : base(id)
        {
            GameId = gameId;
        }

        public Guid GameId { get; }
    }
}