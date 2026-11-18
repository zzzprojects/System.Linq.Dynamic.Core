#if !NET452
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using FluentAssertions;
using NodaTime;
using NodaTime.Text;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.TypeConvertors
{
    public class NodaTimeConverterTests
    {
        private class Entity
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public DateTime? MyDateTimeNullable { get; set; }
            public LocalDate BirthDate { get; set; }
            public LocalDate? BirthDateNullable { get; set; }
            public LocalTime? Time { get; set; }
        }

        private readonly IQueryable<Entity> entities;

        public NodaTimeConverterTests()
        {
            entities = new List<Entity>()
            {
                new Entity { Id = Guid.NewGuid(), FirstName = "Paul", MyDateTimeNullable = new DateTime(1987, 10, 12), BirthDate = new LocalDate(1987, 10, 12), BirthDateNullable = new LocalDate(1987, 10, 12), Time = new LocalTime(11, 1) },
                new Entity { Id = Guid.NewGuid(), FirstName = "Abigail", BirthDate = new LocalDate(1970, 02, 13), Time = new LocalTime(12, 2) },
                new Entity { Id = Guid.NewGuid(), FirstName = "Sophia", BirthDate = new LocalDate(1983, 05, 01), Time = new LocalTime(13, 3) }
            }.AsQueryable();
        }

        [Fact]
        public void FilterByLocalDate_WithTypeConverter()
        {
            // Arrange
            var config = new ParsingConfig
            {
                TypeConverters = new Dictionary<Type, TypeConverter>
                {
                    { typeof(LocalDate), new LocalDateConverter() }
                }
            };

            // Act
            var result = entities.AsQueryable().Where(config, "BirthDate == @0", "1987-10-12").ToList();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void FilterByNullableLocalDate_WithTypeConverter()
        {
            // Arrange
            var config = new ParsingConfig
            {
                TypeConverters = new Dictionary<Type, TypeConverter>
                {
                    { typeof(LocalDate), new LocalDateConverter() }
                }
            };

            // Act
            var result = entities.AsQueryable().Where(config, "BirthDateNullable == @0", "1987-10-12").ToList();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void FilterByLocalDate_WithDynamicExpressionParser()
        {
            // Arrange
            var config = new ParsingConfig
            {
                TypeConverters = new Dictionary<Type, TypeConverter>
                {
                    { typeof(LocalDate), new LocalDateConverter() }
                }
            };

            // Act
            var expr = DynamicExpressionParser.ParseLambda<Entity, bool>(config, false, "BirthDate == @0", "1987-10-12");
            var result = entities.AsQueryable().Where(expr).ToList();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void FilterByNullableLocalDate_WithDynamicExpressionParser()
        {
            // Arrange
            var config = new ParsingConfig
            {
                TypeConverters = new Dictionary<Type, TypeConverter>
                {
                    { typeof(LocalDate), new LocalDateConverter() }
                }
            };

            // Act
            var expr = DynamicExpressionParser.ParseLambda<Entity, bool>(config, false, "BirthDateNullable == \"1987-10-12\"");
            var result = entities.AsQueryable().Where(expr).ToList();

            // Assert
            Assert.Single(result);
        }

        [Theory]
        [InlineData("!= null", 1)]
        [InlineData("== null", 2)]
        public void FilterByNullableDateTime_WithDynamicExpressionParser_CompareWithNull(string equal, int numberOfEntities)
        {
            // Arrange
            var config = new ParsingConfig
            {
                TypeConverters = new Dictionary<Type, TypeConverter>
                {
                    { typeof(LocalDate), new LocalDateConverter() }
                }
            };

            // Act
            var expr = DynamicExpressionParser.ParseLambda<Entity, bool>(config, false, $"MyDateTimeNullable {equal}");
            var result = entities.AsQueryable().Where(expr).ToList();

            // Assert
            result.Should().HaveCount(numberOfEntities);
        }

        [Theory]
        [InlineData("!= null", 1)]
        [InlineData("== null", 2)]
        public void FilterByNullableLocalDate_WithDynamicExpressionParser_CompareWithNull(string equal, int numberOfEntities)
        {
            // Arrange
            var config = new ParsingConfig
            {
                TypeConverters = new Dictionary<Type, TypeConverter>
                {
                    { typeof(LocalDate), new LocalDateConverter() }
                }
            };

            // Act
            var expr = DynamicExpressionParser.ParseLambda<Entity, bool>(config, false, $"BirthDateNullable {equal}");
            var result = entities.AsQueryable().Where(expr).ToList();

            // Assert
            result.Should().HaveCount(numberOfEntities);
        }

        private class EntityWithInstant
        {
            public Instant Timestamp { get; set; }
            public Instant? TimestampNullable { get; set; }
        }

        [Theory]
        [InlineData(">", 1)]
        [InlineData(">=", 2)]
        [InlineData("<", 1)]
        [InlineData("<=", 2)]
        [InlineData("==", 1)]
        [InlineData("!=", 2)]
        public void FilterByInstant_WithRelationalOperator(string op, int expectedCount)
        {
            // Arrange
            var now = SystemClock.Instance.GetCurrentInstant();
            var data = new List<EntityWithInstant>
            {
                new EntityWithInstant { Timestamp = now - Duration.FromHours(1) },
                new EntityWithInstant { Timestamp = now },
                new EntityWithInstant { Timestamp = now + Duration.FromHours(1) }
            }.AsQueryable();

            // Act
            var result = data.Where($"Timestamp {op} @0", now).ToList();

            // Assert
            result.Should().HaveCount(expectedCount);
        }

        [Theory]
        [InlineData(">", 1)]
        [InlineData(">=", 2)]
        [InlineData("<", 1)]
        [InlineData("<=", 2)]
        [InlineData("==", 1)]
        [InlineData("!=", 3)] // null != now evaluates to true in C# nullable semantics
        public void FilterByNullableInstant_WithRelationalOperator(string op, int expectedCount)
        {
            // Arrange
            var now = SystemClock.Instance.GetCurrentInstant();
            var data = new List<EntityWithInstant>
            {
                new EntityWithInstant { TimestampNullable = now - Duration.FromHours(1) },
                new EntityWithInstant { TimestampNullable = now },
                new EntityWithInstant { TimestampNullable = now + Duration.FromHours(1) },
                new EntityWithInstant { TimestampNullable = null }
            }.AsQueryable();

            // Act
            var result = data.Where($"TimestampNullable {op} @0", now).ToList();

            // Assert - null values are excluded from comparison results
            result.Should().HaveCount(expectedCount);
        }

        public class LocalDateConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => sourceType == typeof(string);

            public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
            {
                var result = Convert(value);
                return result.Success ? result.Value : throw new FormatException(value?.ToString());
            }

            private static ParseResult<LocalDate> Convert(object value)
            {
                if (value is string stringValue)
                {
                    return LocalDatePattern.Iso.Parse(stringValue);
                }

                return ParseResult<LocalDate>.ForException(() => new FormatException(value?.ToString()));
            }
        }
    }
}
#endif
