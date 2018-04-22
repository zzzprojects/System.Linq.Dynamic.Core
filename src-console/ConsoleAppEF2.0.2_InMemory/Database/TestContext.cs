using Microsoft.EntityFrameworkCore;

namespace ConsoleAppEF2.Database
{
    public class TestContext : DbContext
    {
        public virtual DbSet<Car> Cars { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("cars");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>().HasKey(c => c.Key);
        }

        // https://stackoverflow.com/questions/46212704/how-do-i-write-ef-functions-extension-method
        public static bool Like(string matchExpression, string pattern) => EF.Functions.Like(matchExpression, pattern);
    }
}
