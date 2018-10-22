using NFluent;
using System.Linq.Dynamic.Core.Util;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Util
{
    public class ParameterExpressionRenamerTests
    {
        [Theory]
        [InlineData("test", "(test + 42)")]
        public void ParameterExpressionRenamer_Rename_ToNewName(string newName, string resultString)
        {
            // Assign
            var expression = Expression.Add(Expression.Parameter(typeof(int), ""), Expression.Constant(42));
            var sut = new ParameterExpressionRenamer(newName);

            // Act
            string result = sut.Rename(expression, out ParameterExpression parameterExpression).ToString();

            // Assert
            Check.That(result).IsEqualTo(resultString);
            Check.That(parameterExpression.Name).IsEqualTo(newName);
        }

        [Theory]
        [InlineData("", "test", "(test + 42)")]
        [InlineData("x", "test", "(test + 42)")]
        public void ParameterExpressionRenamer_Rename_OldNameInNewName(string oldName, string newName, string resultString)
        {
            // Assign
            var expression = Expression.Add(Expression.Parameter(typeof(int), oldName), Expression.Constant(42));
            var sut = new ParameterExpressionRenamer(oldName, newName);

            // Act
            string result = sut.Rename(expression, out ParameterExpression parameterExpression).ToString();

            // Assert
            Check.That(result).IsEqualTo(resultString);
            Check.That(parameterExpression.Name).IsEqualTo(newName);
        }

        [Fact]
        public void ParameterExpressionRenamer_Rename_NoParameterExpressionPresent()
        {
            // Assign
            var expression = Expression.Add(Expression.Constant(1), Expression.Constant(2));
            var sut = new ParameterExpressionRenamer("test");

            // Act
            string result = sut.Rename(expression, out ParameterExpression parameterExpression).ToString();

            // Assert
            Check.That(result).IsEqualTo("(1 + 2)");
            Check.That(parameterExpression).IsNull();
        }
    }
}
