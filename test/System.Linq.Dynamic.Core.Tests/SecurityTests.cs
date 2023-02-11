using System.IO;
using System.Linq.Dynamic.Core.Exceptions;
using System.Net;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public class SecurityTests
{
    class Message
    {
        public string Sender { get; }
        public string Receiver { get; }

        public Message(string sender, string receiver)
        {
            Sender = sender;
            Receiver = receiver;
        }
    }

    [Fact]
    public void MethodsShouldOnlyBeCallableOnPredefinedTypes_Test1()
    {
        // Arrange
        var baseQuery = new[] { 1, 2, 3 }.AsQueryable();
        string predicate = "\"\".GetType().Assembly.DefinedTypes.Where(it.name == \"Assembly\").First().DeclaredMethods.Where(it.Name == \"GetName\").First().Invoke(\"\".GetType().Assembly, new Object[] {} ).Name.ToString() != \"Test\"";

        // Act
        Action action = () => baseQuery.OrderBy(predicate);

        // Assert
        action.Should().Throw<ParseException>().WithMessage("Methods on type 'MethodBase' are not accessible");
    }

    [Fact]
    public void MethodsShouldOnlyBeCallableOnPredefinedTypes_Test2()
    {
        // Arrange
        var messages = new[]
        {
            new Message("Alice", "Bob"),
            new Message("Bob", "Alice")
        }.AsQueryable();

        Action action = () => messages.Where(
            "\"\".GetType().Assembly.GetType(\"System.AppDomain\").GetMethods()[104].Invoke(\"\".GetType().Assembly.GetType(\"System.AppDomain\").GetProperty(\"CurrentDomain\").GetValue(null), \"System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;System.Diagnostics.Process\".Split(\";\".ToCharArray())).GetType().GetMethods()[80].Invoke(null, \"cmd;/T:4A /K whoami && echo was HACKED\".Split(\";\".ToCharArray()))"
        );

        // Assert
        action.Should().Throw<ParseException>().WithMessage($"Methods on type 'Assembly' are not accessible");
    }

    [Theory]
    [InlineData(typeof(FileStream), "Close()", "Stream")]
    [InlineData(typeof(Assembly), "GetName().Name.ToString()", "Assembly")]
    public void DynamicExpressionParser_ParseLambda_IllegalMethodCall_ThrowsException(Type itType, string expression, string type)
    {
        // Act
        Action action = () => DynamicExpressionParser.ParseLambda(itType, null, expression);

        // Assert
        action.Should().Throw<ParseException>().WithMessage($"Methods on type '{type}' are not accessible");
    }
}