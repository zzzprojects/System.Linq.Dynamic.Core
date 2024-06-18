#if EFCORE
using Microsoft.EntityFrameworkCore.DynamicLinq;
#else
using EntityFramework.DynamicLinq;
#endif
using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    [Fact]
    public async Task Entities_SingleOrDefaultAsync()
    {
        // Arrange
        var expectedQueryable1 = _context.Blogs.Where(b => b.Name == "SingleBlog");
        var expected1 = await expectedQueryable1.SingleOrDefaultAsync();

        // Act
        IQueryable queryable1 = _context.Blogs.Where("Name == \"SingleBlog\"");
        var result1 = await queryable1.SingleOrDefaultAsync();

        // Assert
        Assert.Equal(expected1, result1);
    }

    [Fact]
    public async Task Entities_SingleOrDefaultAsync_Predicate()
    {
        // Arrange
#if EFCORE
        var expected1 = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(_context.Blogs, b => b.Name == "SingleBlog");
#else
        var expected1 = await System.Data.Entity.QueryableExtensions.SingleOrDefaultAsync(_context.Blogs, b => b.Name == "SingleBlog");
#endif

        // Act
        var result1 = await _context.Blogs.AsQueryable().SingleOrDefaultAsync("it.Name == \"SingleBlog\"");

        // Assert
        Assert.Equal(expected1, result1);
    }
}