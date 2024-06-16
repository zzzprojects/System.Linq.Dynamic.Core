using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    [Fact]
    public void Entities_LongCount_Predicate()
    {
        // Arrange
        const string search = "a";

        // Act
        long expected = _context.Blogs.LongCount(b => b.Name.Contains(search));
        long result = _context.Blogs.LongCount("Name.Contains(@0)", search);

        // Assert
        Assert.Equal(expected, result);
    }
}