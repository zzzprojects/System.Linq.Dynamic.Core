using FluentAssertions;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    public class NumberParserTests
    {
        private readonly ParsingConfig _parsingConfig = new ParsingConfig();

        public static object[][] Decimals()
        {
            return new object[][]
            {
                new object[] { "de-DE", "1", 1m },
                new object[] { "de-DE", "-42", -42m },
                new object[] { "de-DE", "3,215", 3.215m },
                new object[] { "de-DE", "3.215", 3215m },

                new object[] { null, "1", 1m },
                new object[] { null, "-42", -42m },
                new object[] { null, "3,215", 3215m },
                new object[] { null, "3.215", 3.215m }
            };
        }

        [Theory]
        [MemberData(nameof(Decimals))]
        public void NumberParser_ParseNumber_Decimal(string culture, string text, decimal expected)
        {
            // Arrange
            if (culture != null)
            {
                _parsingConfig.NumberParseCulture = CultureInfo.CreateSpecificCulture(culture);
            }

            // Act
            var result = new NumberParser(_parsingConfig).ParseNumber(text, typeof(decimal));

            // Assert
            result.Should().Be(expected);
        }

        public static object[][] Floats()
        {
            return new object[][]
            {
                new object[] { "de-DE", "1", 1f },
                new object[] { "de-DE", "-42", -42f },
                new object[] { "de-DE", "3,215", 3.215f },
                new object[] { "de-DE", "3.215", 3215f },
                new object[] { "de-DE", "1,2345E-4", 0.00012345f },
                new object[] { "de-DE", "1,2345e-4", 0.00012345f },
                new object[] { "de-DE", "1,2345E4", 12345d },
                new object[] { "de-DE", "1,2345e4", 12345d },

                new object[] { null, "1", 1f },
                new object[] { null, "-42", -42f },
                new object[] { null, "3,215", 3215f },
                new object[] { null, "3.215", 3.215f },
                new object[] { null, "1.2345E-4", 0.00012345f },
                new object[] { null, "1.2345e-4", 0.00012345f },
                new object[] { null, "1.2345E4", 12345f },
                new object[] { null, "1.2345e4", 12345f }
            };
        }

        [Theory]
        [MemberData(nameof(Floats))]
        public void NumberParser_ParseNumber_Float(string culture, string text, float expected)
        {
            // Arrange
            if (culture != null)
            {
                _parsingConfig.NumberParseCulture = CultureInfo.CreateSpecificCulture(culture);
            }

            // Act
            var result = new NumberParser(_parsingConfig).ParseNumber(text, typeof(float));

            // Assert
            result.Should().Be(expected);
        }

        public static IEnumerable<object[]> Doubles()
        {
            return new object[][]
            {
                new object[] { "de-DE", "1", 1d },
                new object[] { "de-DE", "-42", -42d },
                new object[] { "de-DE", "3,215", 3.215d },
                new object[] { "de-DE", "3.215", 3215d },
                new object[] { "de-DE", "1,2345E-4", 0.00012345d },
                new object[] { "de-DE", "1,2345e-4", 0.00012345d },
                new object[] { "de-DE", "1,2345E4", 12345d },
                new object[] { "de-DE", "1,2345e4", 12345d },

                new object[] { null, "1", 1d },
                new object[] { null, "-42", -42d },
                new object[] { null, "3,215", 3215d },
                new object[] { null, "3.215", 3.215d },
                new object[] { null, "1.2345E-4", 0.00012345d },
                new object[] { null, "1.2345e-4", 0.00012345d },
                new object[] { null, "1.2345E4", 12345d },
                new object[] { null, "1.2345e4", 12345d }
            };
        }

        [Theory]
        [MemberData(nameof(Doubles))]
        public void NumberParser_ParseNumber_Double(string culture, string text, double expected)
        {
            // Arrange
            if (culture != null)
            {
                _parsingConfig.NumberParseCulture = CultureInfo.CreateSpecificCulture(culture);
            }

            // Act
            var result = new NumberParser(_parsingConfig).ParseNumber(text, typeof(double));

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("42", 42)]
        [InlineData("-42", -42)]
        [InlineData("77u", 77)]
        [InlineData("77l", 77)]
        [InlineData("77ul", 77)]
        [InlineData("0xff", 255)]
        [InlineData("-0xff", -255)]
        [InlineData("0b1100000011101", 6173)]
        [InlineData("-0b1100000011101", -6173)]
        [InlineData("123d", 123d)]
        [InlineData("123f", 123f)]
        [InlineData("123m", 123)]
        [InlineData("-123d", -123d)]
        [InlineData("-123f", -123f)]
        [InlineData("-123m", -123)]
        public void NumberParser_ParseIntegerLiteral(string text, double expected)
        {
            // Arrange

            // Act
            var result = new NumberParser(_parsingConfig).ParseIntegerLiteral(0, text) as ConstantExpression;

            // Assert
            result.Value.Should().Be(expected);
        }

        [Theory]
        [InlineData("42", 'm', 42)]
        [InlineData("-42", 'm', -42)]
        [InlineData("42m", 'm', 42)]
        [InlineData("-42m", 'm', -42)]

        [InlineData("42", 'd', 42)]
        [InlineData("-42", 'd', -42)]
        [InlineData("42d", 'd', 42)]
        [InlineData("-42d", 'd', -42)]

        [InlineData("42", 'f', 42)]
        [InlineData("-42", 'f', -42)]
        [InlineData("42f", 'f', 42)]
        [InlineData("-42f", 'f', -42)]
        public void NumberParser_ParseRealLiteral(string text, char qualifier, double expected)
        {
            // Arrange

            // Act
            var result = new NumberParser(_parsingConfig).ParseRealLiteral(text, qualifier, true) as ConstantExpression;

            // Assert
            result!.Value.Should().Be(expected);
        }
    }
}
