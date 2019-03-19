namespace GoogleSheetImporter
{
    partial class Program
    {
        public class LegacyDataImporter
        {
            public void Run()
            {
                new CSVGameImporter().Run();
                new CSVPlayImporter().Run();
                new YearlyPlayImporter("2015").Run();
                new YearlyPlayImporter("2016").Run();
                new YearlyPlayImporter("2017").Run();
                new YearlyPlayImporter("2018").Run();
                new YearlyPlayImporter("2019").Run();
            }
        }
    }
}
