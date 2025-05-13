using System.Collections.Generic;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser;

public partial class ExpressionParserTests
{
    [Fact]
    public void Parse_Aggregate_Sum_With_Predicate()
    {
        // Arrange
        var childType = DynamicClassFactory.CreateType(
        [
            new DynamicProperty("Value", typeof(int))
        ]);

        var parentType = DynamicClassFactory.CreateType(
        [
            new DynamicProperty("SubFoos", childType.MakeArrayType())
        ]);

        // Act
        var parser = new ExpressionParser(
            [
                Expression.Parameter(parentType, "Foo")
            ],
            "Foo.SubFoos.Sum(s => s.Value)",
            [],
            new ParsingConfig());

        // Assert
        parser.Parse(typeof(int));
    }

    [Fact]
    public void Parse_Aggregate_Sum_In_Sum_With_Predicate_On_IEnumerable()
    {
        // Arrange
        var childType = DynamicClassFactory.CreateType(
        [
            new DynamicProperty("DoubleArray", typeof(IEnumerable<double>))
        ]);

        var parentType = DynamicClassFactory.CreateType(
        [
            new DynamicProperty("SubFoos", childType.MakeArrayType())
        ]);

        // Act
        var parser = new ExpressionParser(
            [
                Expression.Parameter(parentType, "Foo")
            ],
            "Foo.SubFoos.Sum(s => s.DoubleArray.Sum())",
            [],
            new ParsingConfig());

        // Assert
        parser.Parse(typeof(double));
    }

    [Fact]
    public void Parse_Aggregate_Sum_In_Sum_With_Predicate_On_Array()
    {
        // Arrange
        var childType = DynamicClassFactory.CreateType(
        [
            new DynamicProperty("DoubleArray", typeof(double[]))
        ]);

        var parentType = DynamicClassFactory.CreateType(
        [
            new DynamicProperty("SubFoos", childType.MakeArrayType())
        ]);

        // Act
        var parser = new ExpressionParser(
            [
                Expression.Parameter(parentType, "Foo")
            ],
            "Foo.SubFoos.Sum(s => s.DoubleArray.Sum())",
            [],
            new ParsingConfig());

        // Assert
        parser.Parse(typeof(double));
    }

    [Fact]
    public void Parse_Aggregate_Sum_In_Sum_In_Sum_With_Predicate_On_ArrayArray()
    {
        // Arrange
        var childType = DynamicClassFactory.CreateType(
        [
            new DynamicProperty("DoubleArrayArray", typeof(double[][]))
        ]);

        var parentType = DynamicClassFactory.CreateType(
        [
            new DynamicProperty("SubFoos", childType.MakeArrayType())
        ]);

        // Act
        var parser = new ExpressionParser(
            [
                Expression.Parameter(parentType, "Foo")
            ],
            "Foo.SubFoos.Sum(s => s.DoubleArrayArray.Sum(x => x.Sum()))",
            [],
            new ParsingConfig());

        // Assert
        parser.Parse(typeof(double));
    }
}