using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class ExpressionTests
{
    [Fact]
    public void ExpressionTests_Add_Number()
    {
        // Arrange
        var values = new[] { -1, 2 }.AsQueryable();

        // Act
        var result = values.Select<int>("it + 1");
        var expected = values.Select(i => i + 1);

        // Assert
        result.Should().Contain(expected);
    }

    [Fact]
    public void ExpressionTests_Add_String()
    {
        // Arrange
        var values = new[] { "a", "b" }.AsQueryable();

        // Act
        var result = values.Select<string>("it + \"z\"");
        var expected = values.Select(i => i + "z");

        // Assert
        result.Should().Contain(expected);
    }

    [Fact]
    public void ExpressionTests_SupportedOperands_IAddAndSubtractSignatures_DateTime_Subtracts_DateTime()
    {
        // Arrange
        var values = new[]
        {
            new
            {
                Value1 = new DateTime(2023, 1, 4),
                Value2 = new DateTime(2023, 1, 3)
            }
        }.AsQueryable();

        // Act
        var result = values.Select<TimeSpan>("Value1 - Value2");
        var expected = values.Select(x => x.Value1 - x.Value2);

        // Assert
        result.Should().Contain(expected);
    }

    [Fact]
    public void ExpressionTests_SupportedOperands_IAddAndSubtractSignatures_DateTime_Adds_TimeSpan()
    {
        // Arrange
        var values = new[]
        {
            new
            {
                Value1 = new DateTime(2023, 1, 4),
                Value2 = TimeSpan.FromDays(1)
            }
        }.AsQueryable();

        // Act
        var result = values.Select<DateTime>("Value1 + Value2");
        var expected = values.Select(x => x.Value1 + x.Value2);

        // Assert
        result.Should().Contain(expected);
    }

    [Fact]
    public void ExpressionTests_SupportedOperands_IAddAndSubtractSignatures_DateTime_Subtracts_TimeSpan()
    {
        // Arrange
        var values = new[]
        {
            new
            {
                Value1 = new DateTime(2023, 1, 4),
                Value2 = TimeSpan.FromDays(1)
            }
        }.AsQueryable();

        // Act
        var result = values.Select<DateTime>("Value1 - Value2");
        var expected = values.Select(x => x.Value1 - x.Value2);

        // Assert
        result.Should().Contain(expected);
    }

    [Fact]
    public void ExpressionTests_SupportedOperands_IAddAndSubtractSignatures_TimeSpan_Adds_TimeSpan()
    {
        // Arrange
        var values = new[]
        {
            new
            {
                Value1 = TimeSpan.FromDays(1),
                Value2 = TimeSpan.FromDays(1)
            }
        }.AsQueryable();

        // Act
        var result = values.Select<TimeSpan>("Value1 + Value2");
        var expected = values.Select(x => x.Value1 + x.Value2);

        // Assert
        result.Should().Contain(expected);
    }

    [Fact]
    public void ExpressionTests_SupportedOperands_IAddAndSubtractSignatures_TimeSpan_Subtracts_TimeSpan()
    {
        // Arrange
        var values = new[]
        {
            new
            {
                Value1 = TimeSpan.FromDays(7),
                Value2 = TimeSpan.FromDays(1)
            }
        }.AsQueryable();

        // Act
        var result = values.Select<TimeSpan>("Value1 - Value2");
        var expected = values.Select(x => x.Value1 - x.Value2);

        // Assert
        result.Should().Contain(expected);
    }
}