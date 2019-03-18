using System.Collections.Generic;

namespace PlayStats.Data
{
    public class Game : GameBase
    {
        public SoloMode SoloMode { get; set; }

        public int Rating { get; set; }

        public int DesireToPlay { get; set; }

        public List<LinkedGame> LinkedGames { get; set; }
    }
}
