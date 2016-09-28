using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public void Cast_To_nullableint()
        {
            // Arrange
            PopulateTestData(1, 0);

            // Act
            var expectedResult = _context.Blogs.Select(b => (int?)b.BlogId);
            var result = _context.Blogs.AsQueryable().Select("int?(BlogId)");

            // Assert
            Assert.Equal(expectedResult.ToArray(), result.ToDynamicArray<int?>());
        }

        [Fact]
        public void Cast_To_newnullableint()
        {
            // Arrange
            PopulateTestData(1, 0);

            // Act
            var expectedResult = _context.Blogs.Select(x => new { i = (int?)x.BlogId });
            var result = _context.Blogs.AsQueryable().Select("new (int?(BlogId) as i)");
           
            //Assert
            Assert.Equal(expectedResult.Count(), result.Count());
        }
    }
}