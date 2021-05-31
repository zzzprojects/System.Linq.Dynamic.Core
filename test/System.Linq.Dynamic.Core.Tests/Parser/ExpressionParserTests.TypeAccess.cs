using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    partial class ExpressionParserTests
    {
        [Fact]
        public void ParseTypeAccess_Via_Constructor_CharAndInt_To_String()
        {
            // Arrange
            var parameter = Expression.Parameter(typeof(string));

            // Act
            var parser = new ExpressionParser(new[] { parameter }, $"string('c', 3)", new object[] { }, ParsingConfig.Default);
            var expression = parser.Parse(typeof(string));

            // Assert
            expression.ToString().Should().Be("new String(c, 3)");
        }

        [Theory]
        [InlineData(123, "new DateTime(123)")]
        [InlineData(633979008000000000, "new DateTime(633979008000000000)")]
        public void ParseTypeAccess_Via_Constructor_Ticks_To_DateTime(object ticks, string result)
        {
            // Arrange
            var parameter = Expression.Parameter(typeof(DateTime));

            // Act
            var parser = new ExpressionParser(new[] { parameter }, $"DateTime({ticks})", new object[] { }, ParsingConfig.Default);
            var expression = parser.Parse(typeof(DateTime));

            // Assert
            expression.ToString().Should().Be(result);
        }

        [Fact]
        public void ParseTypeAccess_Via_Constructor_String_To_DateTime_Valid()
        {
            // Arrange
            string str = "\"2020-10-31 09:15:11\"";
            var parameter = Expression.Parameter(typeof(DateTime));

            // Act
            var parser = new ExpressionParser(new[] { parameter }, $"DateTime({str})", new object[] { }, ParsingConfig.Default);
            var expression = parser.Parse(typeof(DateTime));

            // Assert
            expression.ToString().Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("\"abc\"")]
        public void ParseTypeAccess_Via_Constructor_Any_To_DateTime_Invalid(object any)
        {
            // Arrange
            var parameter = Expression.Parameter(typeof(DateTime));

            // Act
            var parser = new ExpressionParser(new[] { parameter }, $"DateTime({any})", new object[] { }, ParsingConfig.Default);
            Action a = () => parser.Parse(typeof(DateTime));

            // Assert
            a.Should().Throw<Exception>();
        }

        [Fact]
        public void ParseTypeAccess_Via_Constructor_String_To_Uri()
        {
            // Arrange
            string selector = "Uri(\"https://www.example.com/\")";
            var parameter = Expression.Parameter(typeof(Uri));

            // Act
            var parser = new ExpressionParser(new[] { parameter }, selector, new object[] { }, ParsingConfig.Default);
            var expression = parser.Parse(typeof(Uri));

            // Assert
            expression.ToString().Should().Be("https://www.example.com/");
        }

        [Fact]
        public void ParseTypeAccess_Via_Constructor_String_And_UriKind_To_Uri()
        {
            // Arrange
            string selector = "Uri(\"https://www.example.com/\", UriKind.Absolute)";
            var parameter = Expression.Parameter(typeof(Uri));

            // Act
            var parser = new ExpressionParser(new[] { parameter }, selector, new object[] { }, ParsingConfig.Default);
            var expression = parser.Parse(typeof(Uri));

            // Assert
            expression.ToString().Should().Be("new Uri(\"https://www.example.com/\", Absolute)");
        }
    }
}
