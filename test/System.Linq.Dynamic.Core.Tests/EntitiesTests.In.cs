#if EFCORE
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
#endif

using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    /// <summary>
    /// Test for https://github.com/zzzprojects/System.Linq.Dynamic.Core/pull/524
    /// </summary>
    [Fact]
    public void Entities_Where_In_And()
    {
        // Arrange
        var expected = _context.Blogs.Include(b => b.Posts).Where(b => new[] { 1000, 1001, 1002 }.Contains(b.BlogId) && new[] { "Blog1", "Blog2" }.Contains(b.Name)).ToArray();

        // Act
        var test = _context.Blogs.Include(b => b.Posts).Where(@"BlogId in (1000, 1001, 1002) and Name in (""Blog1"", ""Blog2"")").ToArray();

        // Assert
        Assert.Equal(expected, test);
    }
}