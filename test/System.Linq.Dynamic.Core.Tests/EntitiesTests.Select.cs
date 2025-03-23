using System.Collections;
using System.Linq.Dynamic.Core.Exceptions;
using Newtonsoft.Json;
#if EFCORE
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
#endif
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    [Fact]
    public void Entities_Select_SingleColumn_NullCoalescing()
    {
        var expected1 = _context.Blogs.Select(x => (int?)(x.NullableInt ?? 10)).ToArray();
        var expected2 = _context.Blogs.Select(x => (int?)(x.NullableInt ?? 9 + x.BlogId)).ToArray();

        // Act
        var test1 = _context.Blogs.Select<int?>("NullableInt ?? 10").ToArray();
        var test2 = _context.Blogs.Select<int?>("NullableInt ?? 9 + BlogId").ToArray();

        // Assert
        Assert.Equal(expected1, test1);
        Assert.Equal(expected2, test2);
    }

    [Fact]
    public void Entities_Select_SingleColumn()
    {
        //Arrange
        var expected = _context.Blogs.Select(x => x.BlogId).ToArray();

        //Act
        var test = _context.Blogs.Select<int>("BlogId").ToArray();

        //Assert
        Assert.Equal<ICollection>(expected, test);
    }

    [Fact]
    public void Entities_Select_EmptyObject()
    {
        // Arrange
        ParsingConfig config = ParsingConfig.Default;
        config.EvaluateGroupByAtDatabase = true;
        
        var expected = _context.Blogs.Select(x => new { }).ToList();

        // Act
        var test = _context.Blogs.GroupBy(config, "BlogId", "new()").Select<object>("new()").ToList();

        // Assert
        Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(test));
    }

    [Fact]
    public void Entities_Select_BrokenObject()
    {
        ParsingConfig config = ParsingConfig.Default;
        config.DisableMemberAccessToIndexAccessorFallback = false;

        // Silently creates something that will later fail on materialization
        var test = _context.Blogs.Select(config, "new(~.BlogId)");
        test = test.Select(config, "new(nonexistentproperty as howcanthiswork)");

        // Will fail when creating the expression
        config.DisableMemberAccessToIndexAccessorFallback = true;
        Assert.ThrowsAny<ParseException>(() =>
        {
            test = test.Select(config, "new(nonexistentproperty as howcanthiswork)");
        });
    }

    [Fact]
    public void Entities_Select_MultipleColumn()
    {
        //Arrange
        var expected = _context.Blogs.Select(x => new { X = "x", x.BlogId, x.Name }).ToArray();

        //Act
        var test = _context.Blogs.Select("new (\"x\" as X, BlogId, Name)").ToDynamicArray();

        //Assert
        Assert.Equal(
            expected,
            test.Select(x => new { X = "x", BlogId = (int)x.BlogId, Name = (string)x.Name }).ToArray() // Convert to same anonymous type used by expected, so they can be found equal.
        );
    }

    [Fact]
    public void Entities_Select_BlogPosts()
    {
        //Arrange
        var expected = _context.Blogs.Where(x => x.BlogId == 1).SelectMany(x => x.Posts).Select(x => x.PostId).ToArray();

        //Act
        var test = _context.Blogs.Where(x => x.BlogId == 1).SelectMany("Posts").Select<int>("PostId").ToArray();

        //Assert
        Assert.Equal(expected, test);
    }

    // fixed : EF issue !!! https://github.com/aspnet/EntityFramework/issues/4968
    [Fact]
    public void Entities_Select_BlogAndPosts()
    {
        //Arrange
        var expected = _context.Blogs.Select(x => new { x.BlogId, x.Name, x.Posts }).ToArray();

        //Act
        var test = _context.Blogs.Select("new (BlogId, Name, Posts)").ToDynamicArray();

        //Assert
        Assert.Equal(expected.Length, test.Length);
        for (int i = 0; i < expected.Length; i++)
        {
            var expectedRow = expected[i];
            var testRow = test[i];

            Assert.Equal(expectedRow.BlogId, testRow.BlogId);
            Assert.Equal(expectedRow.Name, testRow.Name);

            Assert.True(expectedRow.Posts != null);
            Assert.Equal(expectedRow.Posts.ToList(), testRow.Posts);
        }
    }

    /// <summary>
    /// #593
    /// </summary>
    [Fact]
    public void Entities_Select_DynamicClass_And_Call_Any()
    {
        // Act
        var result = _context.Blogs
            .Select("new (BlogId, Name)")
            .Any("Name == \"Blog2\"");

        // Assert
        Assert.True(result);
    }
}