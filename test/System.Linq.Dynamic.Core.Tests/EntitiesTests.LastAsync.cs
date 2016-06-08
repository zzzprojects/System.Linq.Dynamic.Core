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
        // [Maybe not supported : https://msdn.microsoft.com/en-us/library/bb738550.aspx]
#if LASTSUPPORTED
        [Fact]
        public async Task Entities_LastAsync()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expectedQueryable1 = _context.Blogs.Where(b => b.BlogId > 0);
            var expected = await expectedQueryable1.LastAsync();

            //Act
            IQueryable queryable1 = _context.Blogs.Where("BlogId > 0");
            var result = await queryable1.LastAsync();

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task Entities_LastAsync_Predicate()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expected = await EntityFrameworkQueryableExtensions.LastAsync(_context.Blogs, b => b.BlogId > 0);

            //Act
            var result = await (_context.Blogs.AsQueryable() as IQueryable).LastAsync("it.BlogId > 0");

            //Assert
            Assert.Equal(expected, result);
        }
#endif
    }
}