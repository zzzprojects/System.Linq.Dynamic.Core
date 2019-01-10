#if EFCORE
using Microsoft.EntityFrameworkCore.DynamicLinq;
#else
using EntityFramework.DynamicLinq;
#endif
using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public async Task Entities_SingleOrDefaultAsync()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expectedQueryable1 = _context.Blogs.Where(b => b.BlogId > 0);
            var expected1 = await expectedQueryable1.SingleOrDefaultAsync();

            var expectedQueryable2 = _context.Blogs.Where(b => b.BlogId > 9999);
            var expected2 = await expectedQueryable2.SingleOrDefaultAsync();

            //Act
            IQueryable queryable1 = _context.Blogs.Where("BlogId > 0");
            var result1 = await queryable1.SingleOrDefaultAsync();

            IQueryable queryable2 = _context.Blogs.Where("BlogId > 9999");
            var result2 = await queryable2.SingleOrDefaultAsync();

            //Assert
            Assert.Equal(expected1, result1);
            Assert.Null(expected2);
            Assert.Null(result2);
        }

        [Fact]
        public async Task Entities_SingleOrDefaultAsync_Predicate()
        {
            //Arrange
            PopulateTestData(1, 0);
#if EFCORE
            var expected1 = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(_context.Blogs, b => b.BlogId > 0);
            var expected2 = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(_context.Blogs, b => b.BlogId > 9999);
#else
            var expected1 = await System.Data.Entity.QueryableExtensions.SingleOrDefaultAsync(_context.Blogs, b => b.BlogId > 0);
            var expected2 = await System.Data.Entity.QueryableExtensions.SingleOrDefaultAsync(_context.Blogs, b => b.BlogId > 9999);
#endif

            //Act
            var result1 = await _context.Blogs.AsQueryable().SingleOrDefaultAsync("it.BlogId > 0");
            var result2 = await _context.Blogs.AsQueryable().SingleOrDefaultAsync("it.BlogId > 9999");

            //Assert
            Assert.Equal(expected1, result1);
            Assert.Null(expected2);
            Assert.Null(result2);
        }
    }
}
