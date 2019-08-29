using FluentAssertions;
using System.ComponentModel;
using System.Linq.Dynamic.Core.TypeConverters;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.TypeConvertors
{
    public class TypeConverterFactoryTests
    {
        [Theory]
        [InlineData(typeof(DateTimeOffset), typeof(DateTimeOffsetConverter))]
        [InlineData(typeof(DateTime), typeof(DateTimeConverter))]
        [InlineData(typeof(DateTime?), typeof(NullableConverter))]
        [InlineData(typeof(int), typeof(Int32Converter))]
        public void GetConverter_WithDefaultParsingConfig_ReturnsCorrectTypeConverter(Type type, Type expected)
        {
            // Arrange
            var factory = new TypeConverterFactory(ParsingConfig.Default);

            // Act
            var typeConverter = factory.GetConverter(type);

            // Assert
            typeConverter.Should().BeOfType(expected);
        }

        [Theory]
        [InlineData(typeof(DateTimeOffset), typeof(DateTimeOffsetConverter))]
        [InlineData(typeof(DateTime), typeof(CustomDateTimeConverter))]
        [InlineData(typeof(DateTime?), typeof(CustomDateTimeConverter))]
        [InlineData(typeof(int), typeof(Int32Converter))]
        public void GetConverter_WithDateTimeIsParsedAsUTCIsTrue_ReturnsCorrectTypeConverter(Type type, Type expected)
        {
            // Arrange
            var parsingConfig = new ParsingConfig
            {
                DateTimeIsParsedAsUTC = true
            };
            var factory = new TypeConverterFactory(parsingConfig);

            // Act
            var typeConverter = factory.GetConverter(type);

            // Assert
            typeConverter.Should().BeOfType(expected);
        }
    }
}
