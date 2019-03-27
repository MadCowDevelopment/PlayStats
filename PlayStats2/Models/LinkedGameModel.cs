using System;

namespace PlayStats.Models
{
    public class LinkedGameModel : GameModelBase
    {
        public LinkedGameModel(Guid id, Guid gameId) : base(id)
        {
            gameId = GameId;
        }

        public Guid GameId { get; }
    }
}