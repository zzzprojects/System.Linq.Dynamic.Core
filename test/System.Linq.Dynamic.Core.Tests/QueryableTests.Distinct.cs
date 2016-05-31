using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Distinct_Basic()
        {
            //Arrange
            var list = new[] { 1, 2, 2, 3 };
            IQueryable queryable = list.AsQueryable();

            //Act
            var expected = Queryable.Distinct(list.AsQueryable());
            var result = queryable.Distinct();

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Distinct_Basic_Dynamic()
        {
            //Arrange
            var list = new[]
            {
                new { x = "a", list = new[] { 1, 2, 2, 3 } },
                new { x = "b", list = new[] { 5, 6, 6, 8 } },
            };
            IQueryable queryable = list.AsQueryable();

            //Act
            var expected = list.Select(l => l.list.Distinct()).ToArray<object>();
            var result = queryable.Select("list.Distinct()").ToDynamicArray();

            //Assert
            Assert.Equal(expected, result);
        }
    }
}