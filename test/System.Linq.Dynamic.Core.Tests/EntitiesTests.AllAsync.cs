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
        public async Task Entities_AllAsync()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expected = await _context.Blogs.AllAsync(b => b.BlogId > 2000);

            //Act
            var actual = await _context.Blogs.AllAsync("BlogId > 2000");

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
