using NFluent;
using System.Linq.Dynamic.Core.Parser;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    public class SpracheStringParserTests
    {
        [Theory]
        [InlineData("'s'", "s")]
        [InlineData("'\\'", "\\")]
        public void SpracheStringParser_TryParse_SingleQuotedString(string input, string expectedResult)
        {
            // Act
            string result = SpracheStringParser.ParseString(input);

            // Assert
            Check.That(result).Equals(expectedResult);
        }

        [Theory]
        [InlineData("\"/\"", "/")]
        [InlineData("\"a\"", "a")]
        [InlineData("\"This \\\"is\\\" a test.\"", "This \"is\" a test.")]
        [InlineData(@"""This \""is\"" b test.""", @"This ""is"" b test.")]
        [InlineData("\"ab\\\"cd\"", "ab\"cd")]
        [InlineData("\"\\\"\"", "\"")]
        [InlineData("\"\\\"\\\"\"", "\"\"")]
        [InlineData("\"\\\\\"", "\\")]
        public void SpracheStringParser_TryParse_DoubleQuotedString(string input, string expectedResult)
        {
            // Act
            string result = SpracheStringParser.ParseString(input);

            // Assert
            Check.That(result).Equals(expectedResult);
        }
    }
}
