#if NETSTANDARD
using Microsoft.EntityFrameworkCore;
#elif NET4
using System.Data.Entity;
using SQLite.CodeFirst;
#else
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
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

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }
    }
}