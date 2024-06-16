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

namespace System.Linq.Dynamic.Core.Tests;

public class DefaultQueryableAnalyzerTests : IClassFixture<EntitiesTestsDatabaseFixture>
{
    private readonly IQueryableAnalyzer _queryableAnalyzer;

    private readonly BlogContext _context;

    public DefaultQueryableAnalyzerTests(EntitiesTestsDatabaseFixture fixture)
    {
#if EFCORE
        var builder = new DbContextOptionsBuilder();
        if (fixture.UseInMemory)
        {
            builder.UseInMemoryDatabase($"System.Linq.Dynamic.Core.{Guid.NewGuid()}");
        }
        else
        {
            builder.UseSqlServer(fixture.ConnectionString);
        }

        _context = new BlogContext(builder.Options);
#else
        _context = new BlogContext(fixture.ConnectionString);
#endif

        _queryableAnalyzer = new DefaultQueryableAnalyzer();
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