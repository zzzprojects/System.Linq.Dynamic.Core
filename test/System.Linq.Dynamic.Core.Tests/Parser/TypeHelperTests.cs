using System.Linq.Dynamic.Core.Parser;
using FluentAssertions;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser;

public class TypeHelperTests
{
    enum TestEnum
    {
        x = 1
    }

    [Fact]
    public void TypeHelper_TryParseEnum_Valid()
    {
        // Assign + Act
        var result = TypeHelper.TryParseEnum("x", typeof(TestEnum), out var enumValue);

        // Assert
        result.Should().BeTrue();
        enumValue.Should().Be(TestEnum.x);
    }

    [Fact]
    public void TypeHelper_TryParseEnum_Invalid()
    {
        // Assign + Act
        var result = TypeHelper.TryParseEnum("test", typeof(TestEnum), out var enumValue);

        // Assert
        result.Should().BeFalse();
        enumValue.Should().BeNull();
    }

    [Fact]
    public void TypeHelper_IsCompatibleWith_SameTypes_True()
    {
        // Assign + Act
        var result = TypeHelper.IsCompatibleWith(typeof(int), typeof(int));

        // Assert
        Check.That(result).IsTrue();
    }

    [Fact]
    public void TypeHelper_IsCompatibleWith_True()
    {
        // Assign + Act
        var result = TypeHelper.IsCompatibleWith(typeof(int), typeof(long));

        // Assert
        Check.That(result).IsTrue();
    }

    [Fact]
    public void TypeHelper_IsCompatibleWith_False()
    {
        // Assign + Act
        var result = TypeHelper.IsCompatibleWith(typeof(long), typeof(int));

        // Assert
        Check.That(result).IsFalse();
    }
}