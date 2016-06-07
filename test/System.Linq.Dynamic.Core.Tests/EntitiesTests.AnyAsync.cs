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
        public void Entities_AnyAsync()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expectedQueryable1 = _context.Blogs.Where(b => b.BlogId > 0);
            var expectedAny1 = expectedQueryable1.AnyAsync();

            var expectedQueryable2 = _context.Blogs.Where(b => b.BlogId > 9999);
            var expectedAny2 = expectedQueryable2.AnyAsync();

            //Act
            IQueryable queryable1 = _context.Blogs.Where("BlogId > 0");
            var any1 = queryable1.AnyAsync();

            IQueryable queryable2 = _context.Blogs.Where("BlogId > 9999");
            var any2 = queryable2.AnyAsync();

            //Assert
            Assert.Equal(expectedAny1.Result, any1.Result);
            Assert.Equal(expectedAny2.Result, any2.Result);
        }

        [Fact]
        public void Entities_AnyAsync_Predicate()
        {
            //Arrange
            PopulateTestData(1, 0);

#if EFCORE
            var expectedAny1 = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_context.Blogs, b => b.BlogId > 0);
            var expectedAny2 = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_context.Blogs, b => b.BlogId > 9999);
#else
            var expectedAny1 = System.Data.Entity.QueryableExtensions.AnyAsync(_context.Blogs, b => b.BlogId > 0);
            var expectedAny2 = System.Data.Entity.QueryableExtensions.AnyAsync(_context.Blogs, b => b.BlogId > 9999);
#endif

            //Act
            var any1 = _context.Blogs.AsQueryable().AnyAsync("it.BlogId > 0");
            var any2 = _context.Blogs.AsQueryable().AnyAsync("it.BlogId > 9999");

            //Assert
            Assert.Equal(expectedAny1.Result, any1.Result);
            Assert.Equal(expectedAny2.Result, any2.Result);
        }
    }
}