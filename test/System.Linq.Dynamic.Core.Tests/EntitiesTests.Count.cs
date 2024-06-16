using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    [Fact]
    public void Entities_Count_Predicate()
    {
        // Arrange
        const string search = "a";
        
        // Act
        int expected = _context.Blogs.Count(b => b.Name.Contains(search));
        int result = _context.Blogs.Count("Name.Contains(@0)", search);

        // Assert
        Assert.Equal(expected, result);
    }
}