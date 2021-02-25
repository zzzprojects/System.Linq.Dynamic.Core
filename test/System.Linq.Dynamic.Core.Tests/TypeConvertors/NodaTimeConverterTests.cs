#if !NET452
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Reflection;
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

        public class LocalDateConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => sourceType == typeof(string);

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                var result = LocalDatePattern.Iso.Parse(value as string);

                return result.Success
                    ? result.Value
                    : throw new FormatException(value?.ToString());
            }

            protected ParseResult<LocalDate> Convert(object value) => LocalDatePattern.Iso.Parse(value as string);
        }
    }
}
#endif
