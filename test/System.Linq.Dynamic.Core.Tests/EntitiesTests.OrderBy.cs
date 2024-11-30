using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    [Fact]
    public void Entities_OrderBy_RestrictOrderByIsTrue_ValidExpressionShouldNotThrow()
    {
        // Arrange
        var config = new ParsingConfig { RestrictOrderBy = true };

        // Act
        var result = _context.Blogs.OrderBy(b => b.Name).ToArray();
        var dynamicResult = _context.Blogs.OrderBy(config, "Name").ToDynamicArray<Blog>();

        // Assert
        Assert.Equal(result, dynamicResult);
    }

    [Fact]
    public void Entities_OrderBy_RestrictOrderByIsTrue_InvalidExpressionShouldThrow()
    {
        // Arrange
        var config = new ParsingConfig { RestrictOrderBy = true };

        // Act
        Action action = () => _context.Blogs.OrderBy(config, "IIF(1 == 1, 1, 0)");

        // Assert
        action.Should().Throw<ParseException>().WithMessage("No applicable method 'IIF' exists in type 'Blog'");
    }
}