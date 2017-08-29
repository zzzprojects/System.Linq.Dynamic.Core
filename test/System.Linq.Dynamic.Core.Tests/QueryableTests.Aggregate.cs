using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Aggregate_Average()
        {
            // Arrange
            var queryable = new[]
            {
                new AggregateTest { Double = 50, Float = 1.0f, Int = 42, NullableDouble = 400, NullableFloat = 100f, NullableInt = 60 },
                new AggregateTest { Double = 0, Float = 0.0f, Int = 0, NullableDouble = 0, NullableFloat = 0, NullableInt = 0 }
            }.AsQueryable();

            // Act
            var resultNullableFloat = queryable.Aggregate("Average", "NullableFloat");
            var resultFloat = queryable.Aggregate("Average", "Float");
            var resultDouble = queryable.Aggregate("Average", "Double");
            var resultNullableDouble = queryable.Aggregate("Average", "NullableDouble");
            var resultInt = queryable.Aggregate("Average", "Int");
            var resultNullableInt = queryable.Aggregate("Average", "NullableInt");

            // Assert
            Assert.Equal(50f, resultNullableFloat);
            Assert.Equal(200.0, resultNullableDouble);
            Assert.Equal(25.0, resultDouble);
            Assert.Equal(0.5f, resultFloat);
            Assert.Equal(21.0, resultInt);
            Assert.Equal(30.0, resultNullableInt);
        }

        [Fact]
        public void Aggregate_Min()
        {
            // Arrange
            var queryable = new[]
            {
                new AggregateTest { Double = 50, Float = 1.0f, Int = 42, NullableDouble = 400, NullableFloat = 100f, NullableInt = 60 },
                new AggregateTest { Double = 51, Float = 2.0f, Int = 90, NullableDouble = 800, NullableFloat = 101f, NullableInt = 61 }
            }.AsQueryable();

            // Act
            var resultDouble = queryable.Aggregate("Min", "Double");
            var resultFloat = queryable.Aggregate("Min", "Float");
            var resultInt = queryable.Aggregate("Min", "Int");
            var resultNullableDouble = queryable.Aggregate("Min", "NullableDouble");
            var resultNullableFloat = queryable.Aggregate("Min", "NullableFloat");
            var resultNullableInt = queryable.Aggregate("Min", "NullableInt");

            // Assert
            Assert.Equal(50.0, resultDouble);
            Assert.Equal(1.0f, resultFloat);
            Assert.Equal(42, resultInt);
            Assert.Equal(400.0, resultNullableDouble);
            Assert.Equal(100f, resultNullableFloat);
            Assert.Equal(60, resultNullableInt);
        }

        public class AggregateTest
        {
            public double Double { get; set; }

            public double? NullableDouble { get; set; }

            public float Float { get; set; }

            public float? NullableFloat { get; set; }

            public int Int { get; set; }

            public int? NullableInt { get; set; }
        }
    }
}
