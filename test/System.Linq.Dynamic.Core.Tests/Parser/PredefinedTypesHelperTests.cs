using System.Linq.Dynamic.Core.Parser;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser;

public class PredefinedTypesHelperTests
{
    [Fact]
    public void IsPredefinedType_ShouldReturnFalse_ForNonPredefinedTypes()
    {
        // Arrange
        var config = new ParsingConfig();

        // Act & Assert
        PredefinedTypesHelper.IsPredefinedType(config, typeof(IO.File)).Should().BeFalse();
    }

    [Fact]
    public void IsPredefinedType_ShouldReturnTrue_ForPredefinedTypes()
    {
        // Arrange
        var config = new ParsingConfig();
        var predefinedTypes = PredefinedTypesHelper.PredefinedTypes.Keys;

        // Act & Assert
        foreach (var predefinedType in predefinedTypes)
        {
            PredefinedTypesHelper.IsPredefinedType(config, predefinedType).Should().BeTrue();
        }
    }
}