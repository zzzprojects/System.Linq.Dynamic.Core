using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace ConsoleAppEF2.Database
{
    public class TestContext : DbContext
    {
#if EF3 || EF5
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => {
                builder
                    //.AddFilter("Default", LogLevel.Information)
                    .AddFilter("Microsoft", LogLevel.Information)
                    //.AddFilter("System", LogLevel.Information)
                    //.AddDebug()
                    .AddConsole();
            }
        );
#else
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider((filter, includeScopes) => true, true) });
#endif

        public virtual DbSet<Car> Cars { get; set; }

        public virtual DbSet<Brand> Brands { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(MyLoggerFactory); // Warning: Do not create a new ILoggerFactory instance each time
            optionsBuilder.EnableSensitiveDataLogging();

            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=CarsEF20;Trusted_Connection=True;");
        }

        // https://stackoverflow.com/questions/46212704/how-do-i-write-ef-functions-extension-method
        public static bool Like(string matchExpression, string pattern) => EF.Functions.Like(matchExpression, pattern);
    }
}
