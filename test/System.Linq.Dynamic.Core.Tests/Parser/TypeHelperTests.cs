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
    public void TypeHelper_IsCompatibleWith_Int_And_Long_Returns_True()
    {
        // Assign + Act
        var result = TypeHelper.IsCompatibleWith(typeof(int), typeof(long));

        // Assert
        Check.That(result).IsTrue();
    }

    [Theory]

    // True (enum underlying Int32 compatible targets)
    [InlineData(typeof(DayOfWeek), true)]
    [InlineData(typeof(DayOfWeek?), true)]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(int?), true)]
    [InlineData(typeof(long), true)]
    [InlineData(typeof(long?), true)]
    [InlineData(typeof(float), true)]
    [InlineData(typeof(float?), true)]
    [InlineData(typeof(double), true)]
    [InlineData(typeof(double?), true)]
    [InlineData(typeof(decimal), true)]
    [InlineData(typeof(decimal?), true)]
    [InlineData(typeof(object), true)]

    // False (not compatible with enum's Int32 widening rules or reference types)
    [InlineData(typeof(char), false)]
    [InlineData(typeof(char?), false)]
    [InlineData(typeof(short), false)]
    [InlineData(typeof(short?), false)]
    [InlineData(typeof(byte), false)]
    [InlineData(typeof(byte?), false)]
    [InlineData(typeof(sbyte), false)]
    [InlineData(typeof(sbyte?), false)]
    [InlineData(typeof(ushort), false)]
    [InlineData(typeof(ushort?), false)]
    [InlineData(typeof(uint), false)]
    [InlineData(typeof(uint?), false)]
    [InlineData(typeof(ulong), false)]
    [InlineData(typeof(ulong?), false)]
    [InlineData(typeof(bool), false)]
    [InlineData(typeof(bool?), false)]
    [InlineData(typeof(string), false)]
    public void TypeHelper_IsCompatibleWith_Enum(Type targetType, bool expected)
    {
        // Assign + Act
        var result = TypeHelper.IsCompatibleWith(typeof(DayOfWeek), targetType);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void TypeHelper_IsCompatibleWith_Long_And_Int_Returns_False()
    {
        // Assign + Act
        var result = TypeHelper.IsCompatibleWith(typeof(long), typeof(int));

        // Assert
        Check.That(result).IsFalse();
    }
}