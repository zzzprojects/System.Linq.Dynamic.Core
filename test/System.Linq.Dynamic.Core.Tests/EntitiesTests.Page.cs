using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class EntitiesTests
{
    [Fact]
    public void Entities_Page()
    {
        //Arrange
        const int total = 25;
        const int page = 2;
        const int pageSize = 10;

        //Act
        var expected = _context.Blogs.OrderBy(b => b.BlogId).Page(page, pageSize).ToArray();
        IOrderedQueryable queryable = _context.Blogs.Select("it").OrderBy("BlogId");
        bool any = queryable.Any();
        var count = queryable.Count();
        var result = queryable.Page(page, pageSize).ToDynamicArray<Blog>();

        //Assert
        Assert.True(any);
        Assert.Equal(total, count);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Entities_PageResult()
    {
        //Arrange
        const int total = 25;
        const int page = 2;
        const int pageSize = 10;

        //Act
        var expectedResult = _context.Blogs.OrderBy(b => b.BlogId).PageResult(page, pageSize).Queryable.ToArray();
        IQueryable queryable = _context.Blogs.Select("it").OrderBy("BlogId");
        var count = queryable.Count();
        var result = queryable.PageResult(page, pageSize);

        //Assert
        Assert.Equal(total, count);
        Assert.Equal(page, result.CurrentPage);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(total, result.RowCount);
        Assert.Equal(5, result.PageCount);
        Assert.Equal(expectedResult, result.Queryable.ToDynamicArray<Blog>());
    }
}