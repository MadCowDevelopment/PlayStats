using LiteDB;
using System;
using System.Collections.Generic;

namespace PlayStats.Data
{
    public class LiteDBAccessor
    {
        private const string DatabaseFile = @"C:\Users\MGailer\OneDrive\Data\PlayStats\lite.db";

        private const string GamesCollection = "games";

        public void SampleAccess()
        {
            using (var db = CreateLiteDatabase())
            {
                var games = db.GetCollection<Game>(GamesCollection);
                games.EnsureIndex(x => x.Name);
            }
        }

        public void AddGame(Game game)
        {
            using (var db = CreateLiteDatabase())
            {
                var col = db.GetCollection<Game>(GamesCollection);
                col.Insert(game);
            }
        }

        public void AddGames(IEnumerable<Game> games)
        {
            using (var db = CreateLiteDatabase())
            {
                var col = db.GetCollection<Game>(GamesCollection);
                foreach (var game in games)
                {
                    col.Insert(game);
                }
            }
        }

        public void UpdateGame(Game game)
        {
            using (var db = CreateLiteDatabase())
            {
                var col = db.GetCollection<Game>(GamesCollection);
                var existingGame = col.FindById(new BsonValue(game.Id));
                if (existingGame == null) return;
                existingGame.SetProperties(game);
                col.Update(game);
            }
        }

        public Tuple<IEnumerable<Game>> LoadAll()
        {
            using (var db = CreateLiteDatabase())
            {
                var col = db.GetCollection<Game>(GamesCollection);
                return new Tuple<IEnumerable<Game>>(col.FindAll());
            }
        }

        private LiteDatabase CreateLiteDatabase()
        {
            return new LiteDatabase(DatabaseFile);
        }
    }
}
