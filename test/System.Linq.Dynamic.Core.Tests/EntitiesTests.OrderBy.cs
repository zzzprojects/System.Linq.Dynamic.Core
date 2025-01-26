using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    [Theory]
    [InlineData("IIF(1 == 1, 1, 0)")]
    [InlineData("np(Name, \"x\")")]
    public void Entities_OrderBy_RestrictOrderByIsFalse_ShouldAllowAnyExpression(string expression)
    {
        // Arrange
        var config = new ParsingConfig { RestrictOrderByToPropertyOrField = false };

        // Act
        Action action = () => _ = _context.Blogs.OrderBy(config, expression).ToDynamicArray<Blog>();

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void Entities_OrderBy_RestrictOrderByIsTrue_NonRestrictedExpression1ShouldNotThrow()
    {
        // Act 1
        var resultBlogs = _context.Blogs.OrderBy(b => b.Name).ToArray();
        var dynamicResultBlogs = _context.Blogs.OrderBy("Name").ToDynamicArray<Blog>();

        // Assert 1
        Assert.Equal(resultBlogs, dynamicResultBlogs);

        // Act 2
        var resultPosts = _context.Posts.OrderBy(p => p.Blog.Name).ToArray();
        var dynamicResultPosts = _context.Posts.OrderBy("Blog.Name").ToDynamicArray<Post>();

        // Assert 2
        Assert.Equal(resultPosts, dynamicResultPosts);
    }

    [Theory]
    [InlineData(KeywordsHelper.KEYWORD_IT)]
    [InlineData(KeywordsHelper.SYMBOL_IT)]
    [InlineData(KeywordsHelper.KEYWORD_ROOT)]
    [InlineData(KeywordsHelper.SYMBOL_ROOT)]
    [InlineData("\"Blog\" + \"Id\"")]
    public void Entities_OrderBy_RestrictOrderByIsTrue_NonRestrictedExpression2ShouldNotThrow(string expression)
    {
        // Act
        Action action = () => _ = _context.Posts.OrderBy(expression).ToDynamicArray<Post>();

        // Assert 2
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData("IIF(1 == 1, 1, 0)")]
    [InlineData("np(Name, \"x\")")]
    public void Entities_OrderBy_RestrictOrderByIsTrue_RestrictedExpressionShouldThrow(string expression)
    {
        // Act
        Action action = () => _context.Blogs.OrderBy(expression);

        // Assert
        action.Should().Throw<ParseException>().WithMessage("No property or field '*' exists in type 'Blog'");
    }

    [Fact]
    public void Entities_OrderBy_NullPropagation_Int()
    {
        // Arrange
        var config = new ParsingConfig { RestrictOrderByToPropertyOrField = false };
        var resultBlogs = _context.Blogs.OrderBy(b => b.NullableInt ?? -1).ToArray();
        var dynamicResultBlogs = _context.Blogs.OrderBy(config, "np(NullableInt, -1)").ToArray();

        // Assert
        Assert.Equal(resultBlogs, dynamicResultBlogs);
    }

    [Fact]
    public void Entities_OrderBy_NullPropagation_String()
    {
        // Arrange
        var config = new ParsingConfig { RestrictOrderByToPropertyOrField = false };
        var resultBlogs = _context.Blogs.OrderBy(b => b.X ?? "_").ToArray();
        var dynamicResultBlogs = _context.Blogs.OrderBy(config, "np(X, \"_\")").ToArray();

        // Assert
        Assert.Equal(resultBlogs, dynamicResultBlogs);
    }
}