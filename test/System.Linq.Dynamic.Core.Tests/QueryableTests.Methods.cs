using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class QueryableTests
{
    [DynamicLinqType]
    public class QueryTest
    {
        public int Foo { get; set; }
    }

    [DynamicLinqType]
    public class QueryStart
    {
        public IMyQuery<QueryTest> Start { get; set; } = null!;
    }

    [DynamicLinqType]
    public interface IMyQuery<T>
    {
        IMyQuery<TNew> Select<TNew>(Expression<Func<T, TNew>> exp);
    }

    [Fact]
    public void CallMethod_With_ExpressionAsArgument()
    {
        // Arrange
        var query = new[] { new QueryStart() }.AsQueryable();

        var real = query.Select(q => q.Start.Select(s => s.Foo));

        // Act
        var result = query.Select<Foo>("Start.Select(q => q.Foo)");

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void CallMethod()
    {
        // Arrange
        var customTypeProvider = new Mock<IDynamicLinkCustomTypeProvider>();
        customTypeProvider.Setup(c => c.GetCustomTypes()).Returns(new HashSet<Type> { typeof(Test) });
        var config = new ParsingConfig
        {
            CustomTypeProvider = customTypeProvider.Object
        };
        var query = new[] { new Test() }.AsQueryable();

        // Act
        var result = query.Select<decimal>(config, "t => t.GetDecimal()").First();

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public void CallMethodWhichReturnsNullable()
    {
        // Arrange
        var customTypeProvider = new Mock<IDynamicLinkCustomTypeProvider>();
        customTypeProvider.Setup(c => c.GetCustomTypes()).Returns(new HashSet<Type> { typeof(Test) });
        var config = new ParsingConfig
        {
            CustomTypeProvider = customTypeProvider.Object
        };
        var query = new[] { new Test() }.AsQueryable();

        // Act
        var result = query.Select<decimal?>(config, "t => t.GetNullableDecimal()").First();

        // Assert
        Assert.Equal(null, result);
    }

    [Fact]
    public void CallMethodWhichReturnsNullable_WithValue()
    {
        // Arrange
        var customTypeProvider = new Mock<IDynamicLinkCustomTypeProvider>();
        customTypeProvider.Setup(c => c.GetCustomTypes()).Returns(new HashSet<Type> { typeof(Test) });
        var config = new ParsingConfig
        {
            CustomTypeProvider = customTypeProvider.Object
        };
        var query = new[] { new Test() }.AsQueryable();

        // Act
        var result = query.Select<decimal?>(config, "t => t.GetNullableDecimalWithValue()").First();

        // Assert
        Assert.Equal(100, result);
    }
}

class Test
{
    public decimal GetDecimal()
    {
        return 42;
    }

    public decimal? GetNullableDecimal()
    {
        return null;
    }

    public decimal? GetNullableDecimalWithValue()
    {
        return 100;
    }
}