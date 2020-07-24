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
        public async Task Entities_FirstAsync()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expectedQueryable1 = _context.Blogs.Where(b => b.BlogId > 0);

            var expected = await expectedQueryable1.FirstAsync();

            //Act
            IQueryable queryable1 = _context.Blogs.Where("BlogId > 0");
            var result = await queryable1.FirstAsync();

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task Entities_FirstAsync_Predicate()
        {
            //Arrange
            PopulateTestData(1, 0);

#if EFCORE
            var expected = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstAsync(_context.Blogs, b => b.BlogId > 0);
#else
            var expected = await System.Data.Entity.QueryableExtensions.FirstAsync(_context.Blogs, b => b.BlogId > 0);
#endif
            //Act
            var result = await (_context.Blogs.AsQueryable() as IQueryable).FirstAsync("it.BlogId > 0");

            //Assert
            Assert.Equal(expected, result);
        }
    }
}
