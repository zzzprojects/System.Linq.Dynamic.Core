using FluentAssertions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Dynamic.Core.TypeConverters;
using Xunit;
#if !NET452
using static System.Linq.Dynamic.Core.Tests.TypeConvertors.NodaTimeConverterTests;
using NodaTime;
#endif

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

#if !NET452
        [Theory]
        [InlineData(typeof(LocalDate), typeof(LocalDateConverter))]
        [InlineData(typeof(LocalDate?), typeof(LocalDateConverter))]
        public void GetConverter_WithCustomConverer_ReturnsCorrectTypeConverter(Type type, Type expected)
        {
            // Arrange
            var parsingConfig = new ParsingConfig
            {
                TypeConverters = new Dictionary<Type, TypeConverter>
                {
                    { typeof(LocalDate), new LocalDateConverter() }
                }
            };
            var factory = new TypeConverterFactory(parsingConfig);

            // Act
            var typeConverter = factory.GetConverter(type);

            // Assert
            typeConverter.Should().BeOfType(expected);
        }
#endif
    }
}
