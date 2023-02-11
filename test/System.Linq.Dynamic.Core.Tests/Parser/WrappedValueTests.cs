using System.Linq.Dynamic.Core.Parser;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser;

public class WrappedValueTests
{
    [Fact]
    public void WrappedValue_OfTypeString_OperatorEquals_String()
    {
        // Arrange
        var wrapped = new WrappedValue<string>("str");

        // Act
        var result1A = wrapped == "str";
        var result1B = "str" == wrapped;
        var result2A = wrapped == "x";
        var result2B = "x" == wrapped;

        // Assert
        result1A.Should().BeTrue();
        result1B.Should().BeTrue();
        result2A.Should().BeFalse();
        result2B.Should().BeFalse();
    }

    [Fact]
    public void WrappedValue_OfTypeString_OperatorNotEquals_String()
    {
        // Arrange
        var wrapped = new WrappedValue<string>("str");

        // Act
        var result1A = wrapped != "str";
        var result1B = "str" != wrapped;
        var result2A = wrapped == "x";
        var result2B = "x" == wrapped;

        // Assert
        result1A.Should().BeFalse();
        result1B.Should().BeFalse();
        result2A.Should().BeFalse();
        result2B.Should().BeFalse();
    }

    [Fact]
    public void WrappedValue_OfTypeString_OperatorEquals_WrappedValue_OfTypeString()
    {
        // Arrange
        var wrapped = new WrappedValue<string>("str");
        var testEqual = new WrappedValue<string>("str");
        var testNotEqual = new WrappedValue<string>("x");

        // Act
        var result1A = wrapped == testEqual;
        var result1B = testEqual == wrapped;
        var result2A = wrapped == testNotEqual;
        var result2B = testNotEqual == wrapped;

        // Assert
        result1A.Should().BeTrue();
        result1B.Should().BeTrue();
        result2A.Should().BeFalse();
        result2B.Should().BeFalse();
    }

    [Fact]
    public void WrappedValue_OfTypeString_OperatorNotEquals_WrappedValue_OfTypeString()
    {
        // Arrange
        var wrapped = new WrappedValue<string>("str");
        var testEqual = new WrappedValue<string>("str");
        var testNotEqual = new WrappedValue<string>("x");

        // Act
        var result1A = wrapped != testEqual;
        var result1B = testEqual != wrapped;
        var result2A = wrapped != testNotEqual;
        var result2B = testNotEqual != wrapped;

        // Assert
        result1A.Should().BeFalse();
        result1B.Should().BeFalse();
        result2A.Should().BeTrue();
        result2B.Should().BeTrue();
    }
}