using Microsoft.EntityFrameworkCore;

namespace PlayStats.Data
{
    public class CollectionContext : DbContext
    {

        public DbSet<Game> Games { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\Users\MGailer\OneDrive\Data\PlayStats\collection.db");
        }
    }
}
