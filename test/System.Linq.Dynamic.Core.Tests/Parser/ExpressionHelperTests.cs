using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser;

public class ExpressionHelperTests
{
    private readonly ExpressionHelper _sut;

    public ExpressionHelperTests()
    {
        _sut = new ExpressionHelper(ParsingConfig.Default);
    }

    [Fact]
    public void ExpressionHelper_WrapConstantExpression_false()
    {
        // Assign
        var config = new ParsingConfig
        {
            UseParameterizedNamesInDynamicQuery = false
        };
        var expressionHelper = new ExpressionHelper(config);

        string value = "test";
        Expression expression = Expression.Constant(value);

        // Act
        expressionHelper.WrapConstantExpression(ref expression);

        // Assert
        Assert.IsType<ConstantExpression>(expression);
        Assert.Equal("\"test\"", expression.ToString());
    }

    [Fact]
    public void ExpressionHelper_WrapNullableConstantExpression_false()
    {
        // Assign
        var config = new ParsingConfig
        {
            UseParameterizedNamesInDynamicQuery = false
        };
        var expressionHelper = new ExpressionHelper(config);

        int? value = 42;
        Expression expression = Expression.Constant(value);

        // Act
        expressionHelper.WrapConstantExpression(ref expression);

        // Assert
        Assert.IsType<ConstantExpression>(expression);
        Assert.Equal("42", expression.ToString());
    }

    [Fact]
    public void ExpressionHelper_WrapConstantExpression_true()
    {
        // Assign
        var config = new ParsingConfig
        {
            UseParameterizedNamesInDynamicQuery = true
        };
        var expressionHelper = new ExpressionHelper(config);

        string value = "test";
        Expression expression = Expression.Constant(value);

        // Act
        expressionHelper.WrapConstantExpression(ref expression);
        expressionHelper.WrapConstantExpression(ref expression);

        // Assert
        Assert.Equal("System.Linq.Expressions.PropertyExpression", expression.GetType().FullName);
        Assert.Equal("value(System.Linq.Dynamic.Core.Parser.WrappedValue`1[System.String]).Value", expression.ToString());
    }

    [Fact]
    public void ExpressionHelper_WrapNullableConstantExpression_true()
    {
        // Assign
        var config = new ParsingConfig
        {
            UseParameterizedNamesInDynamicQuery = true
        };
        var expressionHelper = new ExpressionHelper(config);

        int? value = 42;
        Expression expression = Expression.Constant(value);

        // Act
        expressionHelper.WrapConstantExpression(ref expression);
        expressionHelper.WrapConstantExpression(ref expression);

        // Assert
        Assert.Equal("System.Linq.Expressions.PropertyExpression", expression.GetType().FullName);
        Assert.Equal("value(System.Linq.Dynamic.Core.Parser.WrappedValue`1[System.Int32]).Value", expression.ToString());
    }

    [Fact]
    public void ExpressionHelper_OptimizeStringForEqualityIfPossible_Guid()
    {
        // Assign
        string guidAsString = Guid.NewGuid().ToString();

        // Act
        var result = _sut.OptimizeStringForEqualityIfPossible(guidAsString, typeof(Guid));

        // Assert
        var ce = Assert.IsType<ConstantExpression>(result);
        ce.ToString().Equals(guidAsString);
    }

    [Fact]
    public void ExpressionHelper_OptimizeStringForEqualityIfPossible_Guid_Invalid()
    {
        // Assign
        string guidAsString = "x";

        // Act
        var result = _sut.OptimizeStringForEqualityIfPossible(guidAsString, typeof(Guid));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ExpressionHelper_TryGenerateAndAlsoNotNullExpression_Nested3NonNullable()
    {
        // Assign
        Expression<Func<Item, int>> expression = x => x.Relation1.Relation2.Id;

        // Act
        bool result = _sut.TryGenerateAndAlsoNotNullExpression(expression, true, out Expression generatedExpression);

        // Assert
        Assert.True(result);
        Assert.Equal("((((x != null) AndAlso (x.Relation1 != null)) AndAlso (x.Relation1.Relation2 != null)) AndAlso (x => x.Relation1.Relation2.Id != null))", generatedExpression.ToString());
    }

    [Fact]
    public void ExpressionHelper_TryGenerateAndAlsoNotNullExpression_Nested3NonNullable_Config_Has_UseDefault()
    {
        // Assign
        var config = new ParsingConfig
        {
            NullPropagatingUseDefaultValueForNonNullableValueTypes = true
        };
        var expressionHelper = new ExpressionHelper(config);

        Expression<Func<Item, int>> expression = x => x.Relation1.Relation2.Id;

        // Act
        bool result = expressionHelper.TryGenerateAndAlsoNotNullExpression(expression, true, out Expression generatedExpression);

        // Assert
        Assert.True(result);
        Assert.Equal("((((x != null) AndAlso (x.Relation1 != null)) AndAlso (x.Relation1.Relation2 != null)) AndAlso (x => x.Relation1.Relation2.Id != null))", generatedExpression.ToString());
    }

    [Fact]
    public void ExpressionHelper_TryGenerateAndAlsoNotNullExpression_Nested1NullableInt()
    {
        // Assign
        Expression<Func<Relation2, int?>> expression = x => x.IdNullable;

        // Act
        bool result = _sut.TryGenerateAndAlsoNotNullExpression(expression, true, out Expression generatedExpression);

        // Assert
        Assert.True(result);
        Assert.Equal("((x != null) AndAlso (x => x.IdNullable != null))", generatedExpression.ToString());
    }

    [Fact]
    public void ExpressionHelper_TryGenerateAndAlsoNotNullExpression_Nested1NullableString()
    {
        // Assign
        Expression<Func<Relation2, string>> expression = x => x.S;

        // Act
        bool result = _sut.TryGenerateAndAlsoNotNullExpression(expression, true, out Expression generatedExpression);

        // Assert
        Assert.True(result);
        Assert.Equal("((x != null) AndAlso (x => x.S != null))", generatedExpression.ToString());
    }

    [Fact]
    public void ExpressionHelper_TryGenerateAndAlsoNotNullExpression_Nested3Nullable_AddSelfFalse()
    {
        // Assign
        Expression<Func<Item, int?>> expression = x => x.Relation1.Relation2.IdNullable;

        // Act
        bool result = _sut.TryGenerateAndAlsoNotNullExpression(expression, false, out Expression generatedExpression);

        // Assert
        Assert.True(result);
        Assert.Equal("(((x != null) AndAlso (x.Relation1 != null)) AndAlso (x.Relation1.Relation2 != null))", generatedExpression.ToString());
    }

    [Fact]
    public void ExpressionHelper_TryGenerateAndAlsoNotNullExpression_Nested3Nullable_AddSelfTrue()
    {
        // Assign
        Expression<Func<Item, int?>> expression = x => x.Relation1.Relation2.IdNullable;

        // Act
        bool result = _sut.TryGenerateAndAlsoNotNullExpression(expression, true, out Expression generatedExpression);

        // Assert
        Assert.True(result);
        Assert.Equal("((((x != null) AndAlso (x.Relation1 != null)) AndAlso (x.Relation1.Relation2 != null)) AndAlso (x => x.Relation1.Relation2.IdNullable != null))", generatedExpression.ToString());
    }

    [Fact]
    public void ExpressionHelper_TryGenerateAndAlsoNotNullExpression_Nullable()
    {
        // Assign
        Expression<Func<Item, int?>> expression = x => x.Id;

        // Act
        bool result = _sut.TryGenerateAndAlsoNotNullExpression(expression, true, out Expression generatedExpression);

        // Assert
        result.Should().BeTrue();
        generatedExpression.ToString().Should().StartWith("((x != null) AndAlso (x =>").And.EndWith("!= null))");
    }

    [Fact]
    public void ConvertAnyArrayToObjectArray_ShouldConvertIntArrayToObjectArray()
    {
        // Arrange
        var array = new[] { 1, 2, 3 };
        var arrayExpression = Expression.Constant(array);

        // Act
        var expression = _sut.ConvertAnyArrayToObjectArray(arrayExpression);

        // Assert
        expression.Should().NotBeNull();

        var lambdaExpressionCompiled = Expression.Lambda(expression).Compile();
        var result = (object[])lambdaExpressionCompiled.DynamicInvoke();

        result.Should().HaveCount(array.Length).And.ContainInOrder((object)1, (object)2, (object)3);
    }

    class Item
    {
        public int Id { get; set; }
        public Relation1 Relation1 { get; set; }
    }

    class Relation1
    {
        public int Id { get; set; }

        public Relation2 Relation2 { get; set; }
    }

    class Relation2
    {
        public int Id { get; set; }

        public int? IdNullable { get; set; }

        public string S { get; set; } = string.Empty;
    }

    [Fact]
    public void GenerateBinaryOrElseTree_With7Expressions()
    {
        // Arrange
        // Build 7 equality comparisons: "it == 1", "it == 2", ... "it == 7"
        var parameter = Expression.Parameter(typeof(int), "it");
        var comparisons = Enumerable.Range(1, 7)
            .Select(i => (Expression)Expression.Equal(parameter, Expression.Constant(i)))
            .ToList();

        // Act
        var result = _sut.GenerateBinaryOrElseTree(comparisons);

        // Assert - tree structure
        // With 7 inputs the balanced binary tree should have depth ceil(log2(7)) = 3
        // Round 1 (7 nodes): (1||2), (3||4), (5||6), 7        => 4 nodes
        // Round 2 (4 nodes): ((1||2)||(3||4)), ((5||6)||7)     => 2 nodes
        // Round 3 (2 nodes): (((1||2)||(3||4))||((5||6)||7))   => 1 node
        result.Should().NotBeNull();
        result.NodeType.Should().Be(ExpressionType.OrElse);

        // Compile and verify correctness: lambda should return true for values 1-7, false otherwise
        var lambda = Expression.Lambda<Func<int, bool>>(result, parameter);
        var compiled = lambda.Compile();

        for (var i = 1; i <= 7; i++)
        {
            compiled(i).Should().BeTrue(because: $"value {i} is in the list");
        }

        compiled(0).Should().BeFalse(because: "0 is not in the list");
        compiled(8).Should().BeFalse(because: "8 is not in the list");

        // Verify the tree is balanced: no OrElse node should have a depth difference > 1
        // by checking that the expression is not a degenerate left-linear chain:
        // a left-linear chain would look like ((((((a||b)||c)||d)||e)||f)||g)
        // the balanced tree root's left child should itself be an OrElse of two OrElse nodes
        var rootOrElse = (BinaryExpression)result;
        rootOrElse.Left.NodeType.Should().Be(ExpressionType.OrElse,
            because: "a balanced tree root's left child should be an OrElse");
        rootOrElse.Right.NodeType.Should().Be(ExpressionType.OrElse,
            because: "a balanced tree root's right child should be an OrElse");
    }
}