#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#elif NET4
using System.Data.Entity;
using SQLite.CodeFirst;
#else
using System.Linq.Dynamic.Core.Tests.Logging;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#endif

namespace System.Linq.Dynamic.Core.Tests.Helpers.Entities
{
    public class BlogContext : DbContext
    {
#if NET4
        public BlogContext(): base("ConnectionStringName") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<BlogContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }
#else
        public BlogContext(DbContextOptions options)
            : base(options)
        {
        }
#endif

#if !NET4
        public void EnableLogging()
        {
            var serviceProvider = this.GetInfrastructure();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new DbLoggerProvider());
        }
#endif

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }
    }
}