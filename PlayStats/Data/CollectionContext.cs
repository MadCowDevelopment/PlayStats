
using LiteDB;
using System;
using System.Collections.Generic;

namespace PlayStats.Data
{
    public class CollectionContext
    {
        public void SampleAccess()
        {
            using (var db = new LiteDatabase(@"C:\Users\MGailer\OneDrive\Data\PlayStats\lite.db"))
            {
                var col = db.GetCollection<Game>("games");

                var game = new Game
                {
                    Id = Guid.NewGuid(),
                    Name = "John Doe"
                };

                col.Insert(game);

                game.Name = "Joana Doe";
                col.Update(game);

                col.EnsureIndex(x => x.Name);

                var results = col.Find(x => x.Name.StartsWith("Jo"));
            }
        }

        public IEnumerable<Game> GetGames()
        {
            using (var db = new LiteDatabase(@"C:\Users\MGailer\OneDrive\Data\PlayStats\lite.db"))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<Game>("games");
                return col.FindAll();
            }
        }
    }
}
