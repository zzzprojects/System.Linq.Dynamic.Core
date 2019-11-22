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
        public void Entities_All()
        {
            //Arrange
            PopulateTestData(1, 0);

            var expected = _context.Blogs.All(b => b.BlogId > 2000);

            //Act
            var actual = _context.Blogs.All("BlogId > 2000");

            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
