#if EFCORE
using System.Linq.Dynamic.Core.Tests.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
#else
using System.Data.Entity;
#endif

namespace System.Linq.Dynamic.Core.Tests.Helpers.Entities
{
#if EFCORE
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions options)
            : base(options)
        {
        }

        public void EnableLogging()
        {
            var serviceProvider = this.GetInfrastructure();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new DbLoggerProvider());
        }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }
    }
#else
    [DbConfigurationType(typeof(CodeConfig))]
    public class BlogContext : DbContext
    {
        // http://stackoverflow.com/questions/20460357/problems-using-entity-framework-6-and-sqlite
        // http://stackoverflow.com/questions/18882560/entity-framework-code-first-update-database-fails-on-create-database
        public BlogContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Directory.GetCurrentDirectory());
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    System.Data.Entity.SqlServer.SqlProviderServices.
        //    //var sqliteConnectionInitializer = new SqliteDropCreateDatabaseAlways<BlogContext>(modelBuilder);
        //    //Database.SetInitializer(sqliteConnectionInitializer);
        //}

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }
    }

    public class CodeConfig : DbConfiguration
    {
        public CodeConfig()
        {
            SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);
        }
    }
#endif
}