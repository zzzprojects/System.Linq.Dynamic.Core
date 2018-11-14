using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        // https://github.com/StefH/System.Linq.Dynamic.Core/issues/44
        [Fact]
        public void Cast_To_nullableint()
        {
            // Arrange
            PopulateTestData(1, 0);

            // Act
            var expectedResult = _context.Blogs.Select(b => (int?)b.BlogId).Count();
            var result = _context.Blogs.AsQueryable().Select("int?(BlogId)").Count();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Cast_To_nullableint_Automatic()
        {
            // Arrange
            PopulateTestData(5, 0);

            // Act
            var expectedResult = _context.Blogs.Select(b => b.BlogId == 2 ? (int?)b.BlogId : null).ToList();
            var result = _context.Blogs.AsQueryable().Select("BlogId == 2 ? BlogId : null").ToDynamicList<int?>();

            // Assert
            Check.That(result).ContainsExactly(expectedResult);
        }

        [Fact]
        public void Cast_To_nullablelong()
        {
            // Arrange
            PopulateTestData(1, 0);

            // Act
            var expectedResult = _context.Blogs.Select(b => (long?)b.BlogId).Count();
            var result = _context.Blogs.AsQueryable().Select("long?(BlogId)").Count();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        // https://github.com/StefH/System.Linq.Dynamic.Core/issues/44
        [Fact]
        public void Cast_To_newnullableint()
        {
            // Arrange
            PopulateTestData(1, 0);

            // Act
            var expectedResult = _context.Blogs.Select(x => new { i = (int?)x.BlogId }).Count();
            var result = _context.Blogs.AsQueryable().Select("new (int?(BlogId) as i)").Count();

            //Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
