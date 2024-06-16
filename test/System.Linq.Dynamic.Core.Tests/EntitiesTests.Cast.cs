using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    // https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/577
#if NET6_0_OR_GREATER
    [Fact]
    public void Entities_Cast_To_FromStringToInt()
    {
        // Act
        var result = _context.Blogs
            .Where(b => b.BlogId >= 1000 && b.BlogId <= 1001)
            .AsQueryable()
            .Select("X")
            .Select("Cast(\"int\")")
            .ToDynamicArray<int>();

        // Assert
        Assert.Equal(new[] { 0, 1 }, result);
    }
#endif

    // https://github.com/StefH/System.Linq.Dynamic.Core/issues/44
    [Fact]
    public void Entities_Cast_To_nullableint()
    {
        // Act
        var expectedResult = _context.Blogs.Select(b => (int?)b.BlogId).Count();
        var result = _context.Blogs.AsQueryable().Select("int?(BlogId)").Count();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Entities_Cast_To_nullableint_Automatic()
    {
        // Act
        var expectedResult = _context.Blogs.Select(b => b.BlogId == 2 ? (int?)b.BlogId : null).ToList();
        var result = _context.Blogs.AsQueryable().Select("BlogId == 2 ? BlogId : null").ToDynamicList<int?>();

        // Assert
        Check.That(result).ContainsExactly(expectedResult);
    }

    [Fact]
    public void Entities_Cast_To_nullablelong()
    {
        // Act
        var expectedResult = _context.Blogs.Select(b => (long?)b.BlogId).Count();
        var result = _context.Blogs.AsQueryable().Select("long?(BlogId)").Count();

        // Assert
        Assert.Equal(expectedResult, result);
    }

    // https://github.com/StefH/System.Linq.Dynamic.Core/issues/44
    [Fact]
    public void Entities_Cast_To_newnullableint()
    {
        // Act
        var expectedResult = _context.Blogs.Select(x => new { i = (int?)x.BlogId }).Count();
        var result = _context.Blogs.AsQueryable().Select("new (int?(BlogId) as i)").Count();

        //Assert
        Assert.Equal(expectedResult, result);
    }
}