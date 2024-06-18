#if EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
#else
using System.Data.Entity;
using EntityFramework.DynamicLinq;
#endif
using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    [Fact]
    public void Entities_AverageAsync()
    {
        // Arrange
        double expected = _context.Blogs.Select(b => b.BlogId).AverageAsync().GetAwaiter().GetResult();

        // Act
        double actual = _context.Blogs.Select("BlogId").AverageAsync().GetAwaiter().GetResult();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Entities_AverageAsync_Selector()
    {
        // Arrange
        double expected = await _context.Blogs.AverageAsync(b => b.BlogId);

        // Act
        double actual = await _context.Blogs.AverageAsync("BlogId");

        // Assert
        Assert.Equal(expected, actual);
    }
}