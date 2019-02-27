using Linq.PropertyTranslator.Core;
using LinqKit;
using NFluent;
using QueryInterceptor.Core;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using Xunit;
#if EFCORE
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
#endif

namespace System.Linq.Dynamic.Core.Tests
{
    public class DefaultQueryableAnalyzerTests : IDisposable
    {
        private readonly IQueryableAnalyzer _queryableAnalyzer;

        BlogContext _context;

        // static readonly Random Rnd = new Random(1);

        public DefaultQueryableAnalyzerTests()
        {
#if EFCORE
            var builder = new DbContextOptionsBuilder();
            // builder.UseSqlite($"Filename=System.Linq.Dynamic.Core.{Guid.NewGuid()}.db");
            builder.UseInMemoryDatabase($"System.Linq.Dynamic.Core.{Guid.NewGuid()}");

            _context = new BlogContext(builder.Options);
            _context.Database.EnsureCreated();
#else
            _context = new BlogContext($"data source=(LocalDB)\\MSSQLLocalDB;attachdbfilename=|DataDirectory|\\System.Linq.Dynamic.Core.{Guid.NewGuid()}.mdf;integrated security=True;connect timeout=30;MultipleActiveResultSets=True;App=EntityFramework");
            _context.Database.CreateIfNotExists();
#endif
            _queryableAnalyzer = new DefaultQueryableAnalyzer();
        }

        public void Dispose()
        {
#if EFCORE
            _context.Database.EnsureDeleted();
#else
            _context.Database.Delete();
#endif
            _context.Dispose();
            _context = null;
        }

        [Fact]
        public void DefaultQueryableAnalyzer_SupportsLinqToObjects_ObjectQuery()
        {
            // Assign
            var query = new[] { 1, 2 }.AsQueryable().Where(x => x > 0);

            // Act
            bool result = _queryableAnalyzer.SupportsLinqToObjects(query);

            // Assert
            Check.That(result).IsTrue();
        }

        [Fact]
        public void DefaultQueryableAnalyzer_SupportsLinqToObjects_ObjectQuery_With_QueryInterceptor()
        {
            // Assign
            var query = new[] { 1, 2 }.AsQueryable().Where(x => x > 0).InterceptWith(new PropertyVisitor());

            // Act
            bool result = _queryableAnalyzer.SupportsLinqToObjects(query);

            // Assert
            Check.That(result).IsTrue();
        }

        [Fact]
        public void DefaultQueryableAnalyzer_SupportsLinqToObjects_ObjectQuery_With_ExpandableQuery()
        {
            // Assign
            var query = new[] { 1, 2 }.AsQueryable().Where(x => x > 0).AsExpandable();

            // Act
            bool result = _queryableAnalyzer.SupportsLinqToObjects(query);

            // Assert
            Check.That(result).IsTrue();
        }

        [Fact]
        public void DefaultQueryableAnalyzer_SupportsLinqToObjects_EntitiesQuery()
        {
            // Assign
            var query = _context.Blogs.Where(b => b.BlogId > 0);

            // Act
            bool result = _queryableAnalyzer.SupportsLinqToObjects(query);

            // Assert
            Check.That(result).IsFalse();
        }

        [Fact]
        public void DefaultQueryableAnalyzer_SupportsLinqToObjects_EntitiesQuery_With_QueryInterceptor()
        {
            // Assign
            var query = _context.Blogs.Where(b => b.BlogId > 0).InterceptWith(new PropertyVisitor());

            // Act
            bool result = _queryableAnalyzer.SupportsLinqToObjects(query);

            // Assert
            Check.That(result).IsFalse();
        }

        [Fact]
        public void DefaultQueryableAnalyzer_SupportsLinqToObjects_EntitiesQuery_With_ExpandableQuery()
        {
            // Assign
            var query = _context.Blogs.Where(b => b.BlogId > 0).AsExpandable();

            // Act
            bool result = _queryableAnalyzer.SupportsLinqToObjects(query);

            // Assert
            Check.That(result).IsFalse();
        }
    }
}
