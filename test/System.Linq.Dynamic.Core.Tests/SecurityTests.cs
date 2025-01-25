using System.IO;
using System.Linq.Dynamic.Core.Exceptions;
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
        var predicate = "\"\".GetType().Assembly.DefinedTypes.Where(it.name == \"Assembly\").First().DeclaredMethods.Where(it.Name == \"GetName\").First().Invoke(\"\".GetType().Assembly, new Object[] {} ).Name.ToString() != \"Test\"";

        // Act
        Action action = () => baseQuery.OrderBy(predicate);

        // Assert
        action.Should().Throw<ParseException>().WithMessage("Method 'GetType' on type 'Object' is not accessible.");
    }

    [Fact]
    public void MethodsShouldOnlyBeCallableOnPredefinedTypes_Test2()
    {
        // Arrange
        var messages = new[]
        {
            new Message("Alice", "Bob")
        }.AsQueryable();

        Action action = () => messages.Where(
            "\"\".GetType().Assembly.GetType(\"System.AppDomain\").GetMethods()[104].Invoke(\"\".GetType().Assembly.GetType(\"System.AppDomain\").GetProperty(\"CurrentDomain\").GetValue(null), \"System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;System.Diagnostics.Process\".Split(\";\".ToCharArray())).GetType().GetMethods()[80].Invoke(null, \"cmd;/T:4A /K whoami && echo was HACKED\".Split(\";\".ToCharArray()))"
        );

        // Assert
        action.Should().Throw<ParseException>().WithMessage("Method 'GetType' on type 'Object' is not accessible.");
    }

    [Theory]
    [InlineData(typeof(FileStream), "Close()", "Method 'Close' on type 'Stream' is not accessible.")]
    [InlineData(typeof(Assembly), "GetName().Name.ToString()", "Method 'GetName' on type 'Assembly' is not accessible.")]
    public void DynamicExpressionParser_ParseLambda_IllegalMethodCall_ThrowsException(Type itType, string expression, string errorMessage)
    {
        // Act
        Action action = () => DynamicExpressionParser.ParseLambda(itType, null, expression);

        // Assert
        action.Should().Throw<ParseException>().WithMessage(errorMessage);
    }

    [Theory]
    [InlineData("c => string.Join(\"_\", c.GetType().Assembly.DefinedTypes.SelectMany(t => t.CustomAttributes).Select(a => a.AttributeType).Select(t => t.AssemblyQualifiedName))")]
    [InlineData("c => string.Join(\"_\", c.GetType().Assembly.DefinedTypes.Select(t => t.BaseType).Select(t => t.AssemblyQualifiedName))")]
    [InlineData("c => string.Join(\"_\", c.GetType().Assembly.FullName))")]
    public void UsingSystemReflectionAssembly_ThrowsException(string selector)
    {
        // Arrange
        var queryable = new[]
        {
            new Message("Alice", "Bob")
        }.AsQueryable();

        // Act
        Action action = () => queryable.Select(selector);

        // Assert
        action.Should().Throw<ParseException>().WithMessage("Method 'GetType' on type 'Object' is not accessible.");
    }

    [Theory]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings.SettingsProp[\"jwt\"]")]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings.SettingsField[\"jwt\"]")]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings.ConstantField")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings.SettingsProp[\"jwt\"]")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings.SettingsField[\"jwt\"]")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings.ConstantField")]
    public void UsingStaticClassAsType_ThrowsException(string selector)
    {
        // Arrange
        var queryable = new[]
        {
            new Message("Alice", "Bob")
        }.AsQueryable();

        // Act
        Action action = () => queryable.Select(selector);

        // Assert
        action.Should().Throw<ParseException>().WithMessage("Type 'System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings' not found");
    }

    [Theory]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3.SettingsProp[\"jwt\"]")]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3.SettingsField[\"jwt\"]")]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3.ConstantField")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3.SettingsProp[\"jwt\"]")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3.SettingsField[\"jwt\"]")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3.ConstantField")]
    public void UsingClassAsType_ThrowsException(string selector)
    {
        // Arrange
        var queryable = new[]
        {
            new Message("Alice", "Bob")
        }.AsQueryable();

        // Act
        Action action = () => queryable.Select(selector);

        // Assert
        action.Should().Throw<ParseException>().WithMessage("Type 'System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3' not found");
    }

    [Theory]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings.SettingsProp[\"jwt\"]")]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings.SettingsField[\"jwt\"]")]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings.ConstantField")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings.SettingsProp[\"jwt\"]")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings.SettingsField[\"jwt\"]")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings.ConstantField")]
    public void UsingStaticClassAsType_WhenAddedToDefaultDynamicLinqCustomTypeProvider_ShouldBeOk(string selector)
    {
        // Arrange
        var config = new ParsingConfig();
        config.UseDefaultDynamicLinqCustomTypeProvider([typeof(Helpers.Models.AppSettings), typeof(Helpers.Models.AppSettings3)]);

        var queryable = new[]
        {
            new Message("Alice", "Bob")
        }.AsQueryable();

        // Act
        Action action = () => queryable.Select(config, selector);

        // Assert
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3.SettingsProp[\"jwt\"]")]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3.SettingsField[\"jwt\"]")]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3.ConstantField")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3.SettingsProp[\"jwt\"]")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3.SettingsField[\"jwt\"]")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings3.ConstantField")]
    public void UsingClassAsType_WhenAddedToDefaultDynamicLinqCustomTypeProvider_ShouldBeOk(string selector)
    {
        // Arrange
        var config = new ParsingConfig();
        config.UseDefaultDynamicLinqCustomTypeProvider([typeof(Helpers.Models.AppSettings), typeof(Helpers.Models.AppSettings3)]);

        var queryable = new[]
        {
            new Message("Alice", "Bob")
        }.AsQueryable();

        // Act
        Action action = () => queryable.Select(config, selector);

        // Assert
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings2.SettingsProp[\"jwt\"]")]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings2.SettingsField[\"jwt\"]")]
    [InlineData("System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings2.ConstantField")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings2.SettingsProp[\"jwt\"]")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings2.SettingsField[\"jwt\"]")]
    [InlineData("c => System.Linq.Dynamic.Core.Tests.Helpers.Models.AppSettings2.ConstantField")]
    public void UsingStaticClassWithDynamicTypeAttribute_ShouldBeOk(string selector)
    {
        // Arrange
        var queryable = new[]
        {
            new Message("Alice", "Bob")
        }.AsQueryable();

        // Act
        Action action = () => queryable.Select(selector);

        // Assert
        action.Should().NotThrow();
    }
}