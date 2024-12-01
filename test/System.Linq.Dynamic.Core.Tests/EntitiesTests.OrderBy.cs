using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    [Fact]
    public void Entities_OrderBy_RestrictOrderByIsFalse()
    {
        // Act
        var resultBlogs = _context.Blogs.OrderBy(b => true).ToArray();
        var dynamicResultBlogs = _context.Blogs.OrderBy("IIF(1 == 1, 1, 0)").ToDynamicArray<Blog>();

        // Assert
        Assert.Equal(resultBlogs, dynamicResultBlogs);
    }

    [Fact]
    public void Entities_OrderBy_RestrictOrderByIsTrue_ValidExpressionShouldNotThrow()
    {
        // Arrange
        var config = new ParsingConfig { RestrictOrderByToPropertyOrField = true };

        // Act 1
        var resultBlogs = _context.Blogs.OrderBy(b => b.Name).ToArray();
        var dynamicResultBlogs = _context.Blogs.OrderBy(config, "Name").ToDynamicArray<Blog>();

        // Assert 1
        Assert.Equal(resultBlogs, dynamicResultBlogs);

        // Act 2
        var resultPosts = _context.Posts.OrderBy(p => p.Blog.Name).ToArray();
        var dynamicResultPosts = _context.Posts.OrderBy(config, "Blog.Name").ToDynamicArray<Post>();

        // Assert 2
        Assert.Equal(resultPosts, dynamicResultPosts);
    }

    [Fact]
    public void Entities_OrderBy_RestrictOrderByIsTrue_InvalidExpressionShouldThrow()
    {
        // Arrange
        var config = new ParsingConfig { RestrictOrderByToPropertyOrField = true };

        // Act
        Action action = () => _context.Blogs.OrderBy(config, "IIF(1 == 1, 1, 0)");

        // Assert
        action.Should().Throw<ParseException>().WithMessage("No property or field 'IIF' exists in type 'System.Linq.Dynamic.Core.Tests.Helpers.Entities.Blog'");
    }
}