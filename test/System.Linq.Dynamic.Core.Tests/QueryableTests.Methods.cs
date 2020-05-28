using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void CallMethod()
        {
            // Arrange
            var query = new[] { new Test() }.AsQueryable();

            // Act
            var result = query.Select<decimal>("t => t.GetDecimal()").First();

            // Assert
            Assert.Equal(42, result);
        }

        [Fact]
        public void CallMethodWhichReturnsNullable()
        {
            // Arrange
            var query = new[] { new Test() }.AsQueryable();

            // Act
            var result = query.Select<decimal?>("t => t.GetNullableDecimal()").First();

            // Assert
            Assert.Equal(null, result);
        }

        [Fact]
        public void CallMethodWhichReturnsNullable_WithValue()
        {
            // Arrange
            var query = new[] { new Test() }.AsQueryable();

            // Act
            var result = query.Select<decimal?>("t => t.GetNullableDecimalWithValue()").First();

            // Assert
            Assert.Equal(100, result);
        }
    }

    class Test
    {
        public decimal GetDecimal()
        {
            return 42;
        }

        public decimal? GetNullableDecimal()
        {
            return null;
        }

        public decimal? GetNullableDecimalWithValue()
        {
            return 100;
        }
    }
}
