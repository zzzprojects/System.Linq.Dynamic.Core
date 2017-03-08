#if EFCORE
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
#else
using System.Data.Entity;
using EntityFramework.DynamicLinq;
#endif
using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public async Task Entities_AnyAsync()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expectedQueryable1 = _context.Blogs.Where(b => b.BlogId > 0);
            bool expectedAny1 = await expectedQueryable1.AnyAsync();

            var expectedQueryable2 = _context.Blogs.Where(b => b.BlogId > 9999);
            bool expectedAny2 = await expectedQueryable2.AnyAsync();

            //Act
            IQueryable queryable1 = _context.Blogs.Where("BlogId > 0");
            bool any1 = await queryable1.AnyAsync();

            IQueryable queryable2 = _context.Blogs.Where("BlogId > 9999");
            bool any2 = await queryable2.AnyAsync();

            //Assert
            Assert.Equal(expectedAny1, any1);
            Assert.Equal(expectedAny2, any2);
        }

        [Fact]
        public async Task Entities_AnyAsync_Predicate()
        {
            //Arrange
            PopulateTestData(1, 0);

#if EFCORE
            bool expectedAny1 = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_context.Blogs, b => b.BlogId > 0);
            bool expectedAny2 = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(_context.Blogs, b => b.BlogId > 9999);
#else
            bool expectedAny1 = await System.Data.Entity.QueryableExtensions.AnyAsync(_context.Blogs, b => b.BlogId > 0);
            bool expectedAny2 = await System.Data.Entity.QueryableExtensions.AnyAsync(_context.Blogs, b => b.BlogId > 9999);
#endif

            //Act
            bool any1 = await _context.Blogs.AsQueryable().AnyAsync("it.BlogId > 0");
            bool any2 = await _context.Blogs.AsQueryable().AnyAsync("it.BlogId > 9999");

            //Assert
            Assert.Equal(expectedAny1, any1);
            Assert.Equal(expectedAny2, any2);
        }
    }
}