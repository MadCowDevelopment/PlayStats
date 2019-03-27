using System.Collections.Generic;

namespace PlayStats.Data
{
    public class GameEntity : GameEntityBase
    {
        public SoloMode SoloMode { get; set; }

        public int Rating { get; set; }

        public int DesireToPlay { get; set; }

        public List<LinkedGameEntity> LinkedGames { get; set; }
    }
}
