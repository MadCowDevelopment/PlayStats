using System;
using System.IO;
using PlayStats.Data;

namespace GoogleSheetImporter
{
    public class CSVGameImporter
    {
        public void Run()
        {
            var db = new GameAccessor();

            var csv = File.ReadAllLines("GameList.csv");
            for(int i = 1; i< csv.Length; i++)
            {
                var line = csv[i];
                var cells = line.Split(',');

                var game = new Game();
                game.Name = cells[0];
                game.SoloMode = GetSoloMode(cells[4]);
                game.WantToSell = GetBool(cells[5]);
                game.IsGenuine = GetBool(cells[6]);
                game.IsDelivered = GetBool(cells[7]);
                game.PurchasePrice = double.Parse(cells[8]);
                game.SellPrice = string.IsNullOrEmpty(cells[9]) ? 0 : double.Parse(cells[9]);
                game.Rating = string.IsNullOrEmpty(cells[10]) ? 0 : int.Parse(cells[10]);
                game.DesireToPlay = string.IsNullOrEmpty(cells[11]) ? 0 : int.Parse(cells[11]);

                db.Create(game);
            }
        }

        private bool GetBool(string value)
        {
            return "Yes" == value ? true : false;
        }

        private static SoloMode GetSoloMode(string soloMode) => soloMode switch
        {
            "Yes" => SoloMode.Official,
            "No" => SoloMode.None,
            "Variant" => SoloMode.Variant,
            _ => throw new InvalidOperationException()
        };
    }
}
