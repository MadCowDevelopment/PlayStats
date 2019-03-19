using PlayStats.Data;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GoogleSheetImporter
{
    partial class Program
    {
        public class CSVPlayImporter
        {
            public void Run()
            {
                var games = new GameAccessor().GetAll();

                var db = new PlayAccessor();

                var csv = File.ReadAllLines("GameList.tsv");
                for (int i = 1; i < csv.Length; i++)
                {
                    var line = csv[i];
                    var cells = line.Split('\t');

                    var play = new Play();
                    play.Date = new DateTime(2014, 12, 31);
                    play.Duration = TimeSpan.FromHours(double.Parse(cells[3], CultureInfo.InvariantCulture));
                    play.GameId = games.First(p => p.Name == cells[0]).Id;
                    play.PlayerCount = 2;

                    var numberOfOldPlays = int.Parse(cells[2]);
                    for (int x = 0; x < numberOfOldPlays; x++)
                    {
                        db.Create(play);
                        play.Id = Guid.Empty;
                    }
                }
            }
        }
    }
}
