using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Parser;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    public class StringParserTests
    {
        [Theory]
        [InlineData("'s")]
        [InlineData("\"s")]
        public void StringParser_WithUnexpectedUnclosedString_ThrowsException(string input)
        {
            // Act
            var exception = Assert.Throws<ParseException>(() => StringParser.ParseString(input));

            // Assert
            Assert.Equal($"Unexpected end of string with unclosed string at position 2 near '{input}'.", exception.Message);
        }

        [Fact]
        public void StringParser_With_UnexpectedUnrecognizedEscapeSequence_ThrowsException()
        {
            // Arrange
            string input = new string(new[] { '"', '\\', 'u', '?', '"' });

            // Act
            var exception = Assert.Throws<ParseException>(() => StringParser.ParseString(input));

            // Assert
            Assert.Equal("Unexpected unrecognized escape sequence at position 1 near '\\u?'.", exception.Message);
        }

        [Theory]
        [InlineData("'s'", "s")]
        [InlineData("'\\r'", "\r")]
        [InlineData("'\\\\'", "\\")]
        public void StringParser_Parse_SingleQuotedString(string input, string expectedResult)
        {
            // Act
            string result = StringParser.ParseString(input);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("\"\"", "")]
        [InlineData("\"[]\"", "[]")]
        [InlineData("\"()\"", "()")]
        [InlineData("\"(\\\"\\\")\"", "(\"\")")]
        [InlineData("\"/\"", "/")]
        [InlineData("\"a\"", "a")]
        [InlineData("\"This \\\"is\\\" a test.\"", "This \"is\" a test.")]
        [InlineData(@"""This \""is\"" b test.""", @"This ""is"" b test.")]
        [InlineData("\"ab\\\"cd\"", "ab\"cd")]
        [InlineData("\"\\\"\"", "\"")]
        [InlineData("\"\\\"\\\"\"", "\"\"")]
        [InlineData("\"\\\\\"", "\\")]
        [InlineData("\"AB YZ 19 \uD800\udc05 \u00e4\"", "AB YZ 19 \uD800\udc05 \u00e4")]
        public void StringParser_Parse_DoubleQuotedString(string input, string expectedResult)
        {
            // Act
            string result = StringParser.ParseString(input);

            // Assert
            result.Should().Be(expectedResult);
        }
    }
}
