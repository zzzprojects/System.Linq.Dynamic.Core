using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public partial class ExpressionTests
{
    [Fact]
    public void ExpressionTests_Sum()
    {
        // Arrange
        int[] initValues = [1, 2, 3, 4, 5];
        var qry = initValues.AsQueryable().Select(x => new { strValue = "str", intValue = x }).GroupBy(x => x.strValue);

        // Act
        var result = qry.Select("Sum(intValue)").AsDynamicEnumerable().ToArray()[0];

        // Assert
        Assert.Equal(15, result);
    }

    [Fact]
    public void ExpressionTests_Sum_LowerCase()
    {
        // Arrange
        int[] initValues = [1, 2, 3, 4, 5];
        var qry = initValues.AsQueryable().Select(x => new { strValue = "str", intValue = x }).GroupBy(x => x.strValue);

        // Act
        var result = qry.Select("sum(intValue)").AsDynamicEnumerable().ToArray()[0];

        // Assert
        Assert.Equal(15, result);
    }

    [Fact]
    public void ExpressionTests_Sum2()
    {
        // Arrange
        var initValues = new[]
        {
                new SimpleValuesModel { FloatValue = 1 },
                new SimpleValuesModel { FloatValue = 2 },
                new SimpleValuesModel { FloatValue = 3 },
            };

        var qry = initValues.AsQueryable();

        // Act
        var result = qry.Select("FloatValue").Sum();
        var result2 = ((IQueryable<float>)qry.Select("FloatValue")).Sum();

        // Assert
        Assert.Equal(6.0f, result);
        Assert.Equal(6.0f, result2);
    }
}