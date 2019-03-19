using PlayStats.Data;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GoogleSheetImporter
{
    partial class Program
    {
        public class YearlyPlayImporter
        {
            private readonly string _year;

            public YearlyPlayImporter(string year)
            {
                _year = year;
            }

            public void Run()
            {
                var games = new GameAccessor().GetAll();

                var db = new PlayAccessor();

                var tsv = File.ReadAllLines($"{_year}.tsv");
                for (int i = 1; i < tsv.Length; i++)
                {
                    var line = tsv[i];
                    var cells = line.Split('\t');

                    var play = new Play();
                    play.Date = DateTime.Parse(cells[0]);
                    play.Duration = TimeSpan.FromHours(double.Parse(cells[2], CultureInfo.InvariantCulture));
                    var game = games.FirstOrDefault(p => p.Name == cells[1]);
                    if (game == null)
                    {
                        Console.WriteLine($"Couldn't find game {cells[1]}.");
                        continue;
                    }

                    play.GameId = game.Id;
                    play.Comment = cells.Length == 4 ? cells[3] : null;
                    play.PlayerCount = game.SoloMode == SoloMode.None ? 2 : 1;

                    db.Create(play);
                }
            }
        }
    }
}
