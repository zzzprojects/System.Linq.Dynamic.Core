using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using System.Linq.Expressions;
using Xunit;
using static System.Linq.Expressions.Expression;

namespace System.Linq.Dynamic.Core.Tests.Parser;

public class MethodFinderTest
{
    [Fact]
    public void Method_ToString_OnDynamicLinq_And_SystemLinq_ShouldBeEqual()
    {
        // Arrange
        Expression<Func<int, string>> expr = x => x.ToString();

        var selector = "ToString()";
        var prm = Parameter(typeof(int));
        var parser = new ExpressionParser([prm], selector, [], ParsingConfig.Default);

        // Act
        var expression = parser.Parse(null);

        // Assert
        Assert.Equal(((MethodCallExpression)expr.Body).Method.DeclaringType, ((MethodCallExpression)expression).Method.DeclaringType);
    }

    [Fact]
    public void Method_InstanceEquals_OnDynamicLinq_And_SystemLinq_ShouldBeEqual()
    {
        // Arrange
        var config = new ParsingConfig
        {
            AllowEqualsAndToStringMethodsOnObject = true
        };

        Expression<Func<User, bool>> expr = x => x.Equals("a");

        var selector = "Equals(\"a\")";
        var prm = Parameter(typeof(User));
        var parser = new ExpressionParser([prm], selector, [], config);

        // Act
        var expression = parser.Parse(null);

        // Assert
        Assert.Equal(((MethodCallExpression)expr.Body).Method.DeclaringType, ((MethodCallExpression)expression).Method.DeclaringType);
    }

    [Fact]
    public void Method_StaticEquals_OnDynamicLinq_And_SystemLinq_ShouldBeEqual()
    {
        // Arrange
        var config = new ParsingConfig
        {
            AllowEqualsAndToStringMethodsOnObject = true
        };

        // ReSharper disable once RedundantNameQualifier
        Expression<Func<int?, bool>> expr = x => object.Equals("a", "b");

        var selector = "object.Equals(\"a\", \"b\")";
        var prm = Parameter(typeof(int?));
        var parser = new ExpressionParser([prm], selector, [], config);

        // Act
        var expression = parser.Parse(null);

        // Assert
        Assert.Equal(((MethodCallExpression)expr.Body).Method.DeclaringType, ((MethodCallExpression)expression).Method.DeclaringType);
    }

    [Fact]
    public void Method_ReferenceEquals_OnDynamicLinq_And_SystemLinq_ShouldBeEqual()
    {
        // Arrange
        var config = new ParsingConfig
        {
            AllowEqualsAndToStringMethodsOnObject = true
        };

        // ReSharper disable once RedundantNameQualifier
        Expression<Func<int?, bool>> expr = x => object.ReferenceEquals("a", "b");

        var selector = "object.ReferenceEquals(\"a\", \"b\")";
        var prm = Parameter(typeof(int?));
        var parser = new ExpressionParser([prm], selector, [], config);

        // Act
        var expression = parser.Parse(null);

        // Assert
        Assert.Equal(((MethodCallExpression)expr.Body).Method.DeclaringType, ((MethodCallExpression)expression).Method.DeclaringType);
    }
}