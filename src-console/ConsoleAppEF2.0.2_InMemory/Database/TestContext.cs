using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace ConsoleAppEF2.Database
{
    public class TestContext : DbContext
    {
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider((filter, includeScopes) => true, true) });

        public virtual DbSet<Car> Cars { get; set; }

        public virtual DbSet<Brand> Brands { get; set; }

        public virtual DbSet<BaseDto> BaseDtos { get; set; }

        public virtual DbSet<ComplexDto> ComplexDtos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(MyLoggerFactory); // Warning: Do not create a new ILoggerFactory instance each time 
            optionsBuilder.EnableSensitiveDataLogging();

            optionsBuilder.UseInMemoryDatabase("test");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>().HasKey(c => c.Key);
            modelBuilder.Entity<Brand>().HasKey(b => b.BrandType);
            modelBuilder.Entity<BaseDto>().HasKey(t => t.Key);
            modelBuilder.Entity<ComplexDto>().HasKey(t => t.Key);
        }

        // https://stackoverflow.com/questions/46212704/how-do-i-write-ef-functions-extension-method
        public static bool Like(string matchExpression, string pattern) => EF.Functions.Like(matchExpression, pattern);
    }
}
