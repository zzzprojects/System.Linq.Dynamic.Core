using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public void Entities_Distinct()
        {
            //Arrange
            PopulateTestData(10, 5);

            var expectedQueryable1 = _context.Blogs.Select(b => b.Posts.Distinct());

            //Act
            var expected = _context.Blogs.Select(b => b.Posts.Distinct()).ToArray<object>();
            var result = _context.Blogs.Select("Posts.Distinct()").ToDynamicArray();

            //Assert
            Assert.Equal(expected, result);
        }
    }
}