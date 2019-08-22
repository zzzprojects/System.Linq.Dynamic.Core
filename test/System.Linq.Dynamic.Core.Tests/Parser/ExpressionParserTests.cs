using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    public class ExpressionParserTests
    {
        [Theory]
        [InlineData("it == 1", "(x == 1)")]
        [InlineData("it eq 1", "(x == 1)")]
        [InlineData("it equal 1", "(x == 1)")]
        [InlineData("it != 1", "(x != 1)")]
        [InlineData("it ne 1", "(x != 1)")]
        [InlineData("it neq 1", "(x != 1)")]
        [InlineData("it notequal 1", "(x != 1)")]
        [InlineData("it lt 1", "(x < 1)")]
        [InlineData("it LessThan 1", "(x < 1)")]
        [InlineData("it le 1", "(x <= 1)")]
        [InlineData("it LessThanEqual 1", "(x <= 1)")]
        [InlineData("it gt 1", "(x > 1)")]
        [InlineData("it GreaterThan 1", "(x > 1)")]
        [InlineData("it ge 1", "(x >= 1)")]
        [InlineData("it GreaterThanEqual 1", "(x >= 1)")]
        public void Parse_ParseComparisonOperator(string expression, string result)
        {
            // Arrange
            ParameterExpression[] parameters = { ParameterExpressionHelper.CreateParameterExpression(typeof(int), "x") };
            var sut = new ExpressionParser(parameters, expression, null, null);

            // Act
            var parsedExpression = sut.Parse(null).ToString();

            // Assert
            Check.That(parsedExpression).Equals(result);
        }

        [Theory]
        [InlineData("it || true", "(x OrElse True)")]
        [InlineData("it or true", "(x OrElse True)")]
        [InlineData("it OrElse true", "(x OrElse True)")]
        public void Parse_ParseOrOperator(string expression, string result)
        {
            // Arrange
            ParameterExpression[] parameters = { ParameterExpressionHelper.CreateParameterExpression(typeof(bool), "x") };
            var sut = new ExpressionParser(parameters, expression, null, null);

            // Act
            var parsedExpression = sut.Parse(null).ToString();

            // Assert
            Check.That(parsedExpression).Equals(result);
        }

        [Theory]
        [InlineData("it && true", "(x AndAlso True)")]
        [InlineData("it and true", "(x AndAlso True)")]
        [InlineData("it AndAlso true", "(x AndAlso True)")]
        public void Parse_ParseAndOperator(string expression, string result)
        {
            // Arrange
            ParameterExpression[] parameters = { ParameterExpressionHelper.CreateParameterExpression(typeof(bool), "x") };
            var sut = new ExpressionParser(parameters, expression, null, null);

            // Act
            var parsedExpression = sut.Parse(null).ToString();

            // Assert
            Check.That(parsedExpression).Equals(result);
        }
    }
}
