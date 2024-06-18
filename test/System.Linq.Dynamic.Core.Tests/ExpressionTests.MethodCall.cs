using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class ExpressionTests
{
    private static ParsingConfig CreateParsingConfigForMethodCallTests()
    {
        var customTypeProvider = new Mock<IDynamicLinkCustomTypeProvider>();
        customTypeProvider.Setup(c => c.GetCustomTypes()).Returns(new HashSet<Type> { typeof(User), typeof(Methods), typeof(Foo) });
        return new ParsingConfig
        {
            CustomTypeProvider = customTypeProvider.Object
        };
    }

    private class DefaultDynamicLinqCustomTypeProviderForStaticTesting : DefaultDynamicLinqCustomTypeProvider
    {
        public DefaultDynamicLinqCustomTypeProviderForStaticTesting() : base(ParsingConfig.Default)
        {
        }

        public override HashSet<Type> GetCustomTypes() => new(base.GetCustomTypes()) { typeof(Methods), typeof(MethodsItemExtension) };
    }

    private static ParsingConfig CreateParsingConfigForStaticMethodCallTests()
    {
        return new ParsingConfig
        {
            CustomTypeProvider = new DefaultDynamicLinqCustomTypeProviderForStaticTesting(),
            PrioritizePropertyOrFieldOverTheType = true
        };
    }

    [Fact]
    public void ExpressionTests_MethodCall_WithArgument_And_1_OutArgument()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var users = User.GenerateSampleModels(5);

        // Act
        string s = "";
        var expected = users.Select(u => u.TryParseWithArgument(u.UserName, out s));
        var result = users.AsQueryable().Select<bool>(config, "TryParseWithArgument(it.UserName, $out _)");

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ExpressionTests_MethodCall_WithArgument_And_2_OutArguments()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var users = User.GenerateSampleModels(5);

        // Act
        string s = "?";
        int i = -1;
        var expected = users.Select(u => u.TryParseWithArgumentAndTwoOut(u.UserName, out s, out i));
        var result = users.AsQueryable().Select<bool>(config, "TryParseWithArgumentAndTwoOut(UserName, out _, out _)");

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ExpressionTests_MethodCall_WithoutArgument_And_OutArgument()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var users = User.GenerateSampleModels(5);

        // Act
        string s = "";
        var expected = users.Select(u => u.TryParseWithoutArgument(out s));
        var result = users.AsQueryable().Select<bool>(config, "TryParseWithoutArgument(out _)");

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ExpressionTests_MethodCall_WithArgument_And_NoDiscardForOutArgument_ThrowsException()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var users = User.GenerateSampleModels(5);

        // Act
        Action action = () => users.AsQueryable().Select<bool>(config, "TryParseWithArgument(it.UserName, $out x)");

        // Assert
        action.Should().Throw<ParseException>().WithMessage("When using an out variable, a discard '_' is required.");
    }

    [Fact]
    public void ExpressionTests_MethodCall_NoParams()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var users = User.GenerateSampleModels(3);

        // Act
        var expected = users.Where(u => u.TestMethod1());
        var result = users.AsQueryable().Where(config, "TestMethod1()");

        // Assert
        Assert.Equal(expected.Count(), result.Count());
    }

    [Fact]
    public void ExpressionTests_MethodCall_OneParam_With_it()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var users = User.GenerateSampleModels(3);

        // Act
        var expected = users.Where(u => u.TestMethod2(u));
        var result = users.AsQueryable().Where(config, "TestMethod2(it)");

        // Assert
        Assert.Equal(expected.Count(), result.Count());
    }


    [Fact]
    public void ExpressionTests_MethodCall_OneParam_With_User()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var users = User.GenerateSampleModels(10);
        var testUser = users[2];

        // Act
        var expected = users.Where(u => u.TestMethod3(testUser));
        var result = users.AsQueryable().Where(config, "TestMethod3(@0)", testUser);

        // Assert
        Assert.Equal(expected.Count(), result.Count());
    }

    [Fact]
    public void ExpressionTests_MethodCall_GenericStatic()
    {
        // Arrange
        var config = CreateParsingConfigForStaticMethodCallTests();
        var list = new[] { 0, 1, 2, 3, 4 }.Select(value => new Methods.Item { Value = value }).ToArray();

        // Act
        var expectedResult = list.Where(x => Methods.StaticGenericMethod(x));
        var result = list.AsQueryable().Where(config, "Methods.StaticGenericMethod(it)");

        // Assert
        Assert.Equal(expectedResult.Count(), result.Count());
    }

    [Fact]
    public void ExpressionTests_MethodCall_Generic()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var list = new[] { 0, 1, 2, 3, 4 }.Select(value => new Methods.Item { Value = value }).ToArray();

        // Act
        var methods = new Methods();
        var expectedResult = list.Where(x => methods.GenericMethod(x));
        var result = list.AsQueryable().Where(config, "@0.GenericMethod(it)", methods);

        // Assert
        Assert.Equal(expectedResult.Count(), result.Count());
    }

    [Fact]
    public void ExpressionTests_MethodCall_GenericExtension()
    {
        // Arrange
        var config = CreateParsingConfigForStaticMethodCallTests();
        var list = new[] { 0, 1, 2, 3, 4 }.Select(value => new Methods.Item { Value = value }).ToArray();

        // Act
        var expectedResult = list.Where(x => MethodsItemExtension.Functions.EfCoreCollate(x.Value, "tlh-KX") == 2);
        var result = list.AsQueryable().Where(config, "MethodsItemExtension.Functions.EfCoreCollate(it.Value,\"tlh-KX\")==2");

        // Assert
        Assert.Equal(expectedResult.Count(), result.Count());
    }

    [Fact]
    public void ExpressionTests_MethodCall_ValueTypeToValueTypeParameter()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var list = new[] { 0, 1, 2, 3, 4 };

        // Act
        var methods = new Methods();
        var expectedResult = list.Where(x => methods.Method1(x));
        var result = list.AsQueryable().Where(config, "@0.Method1(it)", methods);

        // Assert
        Assert.Equal(expectedResult.Count(), result.Count());
    }

    [Fact]
    public void ExpressionTests_MethodCall_ValueTypeToObjectParameterWithCast()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var list = new[] { 0, 1, 2, 3, 4 };

        // Act
        var methods = new Methods();
        var expectedResult = list.Where(x => methods.Method2(x));
        var result = list.AsQueryable().Where(config, "@0.Method2(object(it))", methods);

        // Assert
        Assert.Equal(expectedResult.Count(), result.Count());
    }

    [Fact]
    public void ExpressionTests_MethodCall_ValueTypeToObjectParameterWithoutCast()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var list = new[] { 0, 1, 2, 3, 4 };

        // Act
        var methods = new Methods();
        var expectedResult = list.Where(x => methods.Method2(x));
        var result = list.AsQueryable().Where(config, "@0.Method2(it)", methods);

        // Assert
        Assert.Equal(expectedResult.Count(), result.Count());
    }

    [Fact]
    public void ExpressionTests_MethodCall_NullableValueTypeToObjectParameter()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var list = new int?[] { 0, 1, 2, 3, 4, null };

        // Act
        var methods = new Methods();
        var expectedResult = list.Where(x => methods.Method2(x));
        var result = list.AsQueryable().Where(config, "@0.Method2(it)", methods);

        // Assert
        Assert.Equal(expectedResult.Count(), result.Count());
    }

    [Fact]
    public void ExpressionTests_MethodCall_ReferenceTypeToObjectParameter()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var list = new[] { 0, 1, 2, 3, 4 }.Select(value => new Methods.Item { Value = value }).ToArray();

        // Act
        var methods = new Methods();
        var expectedResult = list.Where(x => methods.Method3(x));
        var result = list.AsQueryable().Where(config, "@0.Method3(it)", methods);

        // Assert
        Assert.Equal(expectedResult.Count(), result.Count());
    }

    [Fact]
    public void ExpressionTests_NullPropagating_InstanceMethod_0_Arguments()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var expression = "np(FooValue.Zero().Length)";
        var q = new[] { new Foo { FooValue = new Foo() } }.AsQueryable();

        // Act
        var result = q.Select(config, expression).FirstOrDefault() as int?;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ExpressionTests_NullPropagating_InstanceMethod_1_Argument()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var expression = "np(FooValue.One(1).Length)";
        var q = new[] { new Foo { FooValue = new Foo() } }.AsQueryable();

        // Act
        var result = q.Select(config, expression).FirstOrDefault() as int?;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ExpressionTests_NullPropagating_InstanceMethod_2_Arguments()
    {
        // Arrange
        var config = CreateParsingConfigForMethodCallTests();
        var expression = "np(FooValue.Two(1, 42).Length)";
        var q = new[] { new Foo { FooValue = new Foo() } }.AsQueryable();

        // Act
        var result = q.Select(config, expression).FirstOrDefault() as int?;

        // Assert
        result.Should().BeNull();
    }
}