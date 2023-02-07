using System.Linq.Dynamic.Core.Exceptions;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public class SecurityTests
{
    [Fact]
    public void MethodsShouldOnlyBeCallableOnPredefinedTypes()
    {
        // Arrange
        var baseQuery = new[] { 1, 2, 3 }.AsQueryable();
        string predicate = "\"\".GetType().Assembly.DefinedTypes.Where(it.name == \"Assembly\").First().DeclaredMethods.Where(it.Name == \"GetName\").First().Invoke(\"\".GetType().Assembly, new Object[] {} ).Name.ToString() != \"Test\"";

        // Act
        Action action = () => baseQuery.OrderBy(predicate);

        // Assert
        action.Should().Throw<ParseException>().WithMessage("Methods on type 'MethodBase' are not accessible");
    }

    [Theory]
    [InlineData(typeof(IO.FileStream), "it.Close()", "Stream")]
    [InlineData(typeof(Assembly), "GetName().Name.ToString()", "Assembly")]
    public void DynamicExpressionParser_ParseLambda_IllegalMethodCall_ThrowsException(Type itType, string expression, string type)
    {
        // Act
        Action action = () => DynamicExpressionParser.ParseLambda(itType, null, expression);

        // Assert
        action.Should().Throw<ParseException>().WithMessage($"Methods on type '{type}' are not accessible");
    }
}