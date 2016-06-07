#if EFCORE
using Microsoft.EntityFrameworkCore.DynamicLinq;
#else
using EntityFramework.DynamicLinq;
#endif
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public void Entities_FirstAsync()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expectedQueryable1 = _context.Blogs.Where(b => b.BlogId > 0);
            var expectedTask1 = expectedQueryable1.FirstAsync();

            //Act
            IQueryable queryable1 = _context.Blogs.Where("BlogId > 0");
            var resultTask1 = queryable1.FirstAsync();

            //Assert
            Assert.Equal(expectedTask1.Result, resultTask1.Result);
        }

        [Fact]
        public void Entities_FirstAsync_Predicate()
        {
            //Arrange
            PopulateTestData(1, 0);


#if EFCORE
            var expectedTask1 = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstAsync(_context.Blogs, b => b.BlogId > 0);
#else
            var expectedTask1 = System.Data.Entity.QueryableExtensions.FirstAsync(_context.Blogs, b => b.BlogId > 0);
#endif
            //Act
            var resultTask1 = _context.Blogs.AsQueryable().FirstAsync("it.BlogId > 0");

            //Assert
            Assert.Equal(expectedTask1.Result, resultTask1.Result);
        }
    }
}