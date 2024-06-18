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
    public async Task Entities_FirstOrDefaultAsync()
    {
        // Arrange
        var expectedQueryable1 = _context.Blogs.Where(b => b.Name == "SingleBlog");
        var expected1 = await expectedQueryable1.FirstOrDefaultAsync();

        // Act
        var result1 = await _context.Blogs.Where("Name == \"SingleBlog\"").FirstOrDefaultAsync();
        var result2 = await _context.Blogs.Where("Name == \"NotFound\"").FirstOrDefaultAsync();

        // Assert
        Assert.Equal(expected1, result1);
        Assert.Null(result2);
    }

    [Fact]
    public async Task Entities_FirstOrDefaultAsync_Predicate()
    {
        // Arrange
#if EFCORE
        var expected1 = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.Blogs, b => b.Name == "SingleBlog");
#else
        var expected1 = await System.Data.Entity.QueryableExtensions.FirstOrDefaultAsync(_context.Blogs, b => b.Name == "SingleBlog");
#endif

        // Act
        var result1 = await _context.Blogs.AsQueryable().FirstOrDefaultAsync("Name == \"SingleBlog\"");
        var result2 = await _context.Blogs.AsQueryable().FirstOrDefaultAsync("Name == \"NotFound\"");

        // Assert
        Assert.Equal(expected1, result1);
        Assert.Null(result2);
    }
}