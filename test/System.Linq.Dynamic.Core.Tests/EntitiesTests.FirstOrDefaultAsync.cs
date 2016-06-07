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
        public void Entities_FirstOrDefaultAsync()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expectedQueryable1 = _context.Blogs.Where(b => b.BlogId > 0);
            var expectedTask1 = expectedQueryable1.FirstOrDefaultAsync();

            var expectedQueryable2 = _context.Blogs.Where(b => b.BlogId > 9999);
            var expectedTask2 = expectedQueryable2.FirstOrDefaultAsync();

            //Act
            IQueryable queryable1 = _context.Blogs.Where("BlogId > 0");
            var resultTask1 = queryable1.FirstOrDefaultAsync();

            IQueryable queryable2 = _context.Blogs.Where("BlogId > 9999");
            var resultTask2 = queryable2.FirstOrDefaultAsync();

            //Assert
            Assert.Equal(expectedTask1.Result, resultTask1.Result);
            Assert.Null(expectedTask2.Result);
            Assert.Null(resultTask2.Result);
        }

        [Fact]
        public void Entities_FirstOrDefaultAsync_Predicate()
        {
            //Arrange
            PopulateTestData(1, 0);


#if EFCORE
             var expectedTask1 = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.Blogs, b => b.BlogId > 0);
            var expectedTask2 = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.Blogs, b => b.BlogId > 9999);
#else
            var expectedTask1 = System.Data.Entity.QueryableExtensions.FirstOrDefaultAsync(_context.Blogs, b => b.BlogId > 0);
            var expectedTask2 = System.Data.Entity.QueryableExtensions.FirstOrDefaultAsync(_context.Blogs, b => b.BlogId > 9999);
#endif

            //Act
            var resultTask1 = _context.Blogs.AsQueryable().FirstOrDefaultAsync("it.BlogId > 0");
            var resultTask2 = _context.Blogs.AsQueryable().FirstOrDefaultAsync("it.BlogId > 9999");

            //Assert
            Assert.Equal(expectedTask1.Result, resultTask1.Result);
            Assert.Null(expectedTask2.Result);
            Assert.Null(resultTask2.Result);
        }
    }
}