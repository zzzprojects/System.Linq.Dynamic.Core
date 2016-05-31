using System.Linq.Dynamic.Core.Tests.Helpers.Entities;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public void Entities_Page()
        {
            //Arrange
            const int total = 33;
            const int page = 2;
            const int pageSize = 10;
            PopulateTestData(total, 0);

            //Act
            IQueryable queryable = _context.Blogs.Select("it");
            bool any = queryable.Any();
            var count = queryable.Count();
            var result = queryable.Page(page, pageSize);

            //Assert
            Assert.Equal(true, any);
            Assert.Equal(total, count);
            Assert.Equal(_context.Blogs.Page(page, pageSize).ToArray(), result.ToDynamicArray<Blog>());
        }

        [Fact]
        public void Entities_PageResult()
        {
            //Arrange
            const int total = 44;
            const int page = 2;
            const int pageSize = 10;
            PopulateTestData(total, 0);

            //Act
            IQueryable queryable = _context.Blogs.Select("it");
            var count = queryable.Count();
            var result = queryable.PageResult(page, pageSize);

            //Assert
            Assert.Equal(total, count);
            Assert.Equal(page, result.CurrentPage);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(total, result.RowCount);
            Assert.Equal(5, result.PageCount);
            Assert.Equal(_context.Blogs.Page(page, pageSize).ToArray(), result.Queryable.ToDynamicArray<Blog>());
        }
    }
}