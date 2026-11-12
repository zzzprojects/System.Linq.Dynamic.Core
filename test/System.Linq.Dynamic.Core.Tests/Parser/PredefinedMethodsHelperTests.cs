using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser;

public class PredefinedMethodsHelperTests
{
    [Theory]
    [MemberData(nameof(GetSupportedPropertyNames))]
    public void PredefinedMethodsHelper_SupportedType_CanCall_ToString(string propertyName)
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(SupportedData), "x")];
        var sut = new ExpressionParser(parameters, $"{propertyName}.ToString()", null, ParsingConfig.Default);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        parsedExpression.Should().Be($"x.{propertyName}.ToString()");
    }

    [Theory]
    [MemberData(nameof(GetSupportedPropertyNamesForString1))]
    public void PredefinedMethodsHelper_SupportedType_CanCall_ToString1(string propertyName)
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(SupportedData), "x")];
        var sut = new ExpressionParser(parameters, $"{propertyName}.ToString(\"X\")", null, ParsingConfig.Default);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        parsedExpression.Should().Be($"x.{propertyName}.ToString(\"X\")");
    }

    [Theory]
    [MemberData(nameof(GetNotSupportedPropertyNames))]
    public void PredefinedMethodsHelper_NotSupportedType_Throws(string propertyName)
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(NotSupportedData), "x")];
        var sut = new ExpressionParser(parameters, $"{propertyName}.ToString()", null, ParsingConfig.Default);

        // Act
        Action action = () => _ = sut.Parse(null);

        // Assert
        action.Should().Throw<ParseException>().WithMessage("Method 'ToString' on type 'Object' is not accessible.");
    }

    [Theory]
    [MemberData(nameof(GetNotSupportedPropertyNames))]
    public void PredefinedMethodsHelper_NotSupportedType_AllowEqualsAndToStringMethodsOnObject_CanCall_ToString(string propertyName)
    {
        // Arrange
        var config = new ParsingConfig
        {
            AllowEqualsAndToStringMethodsOnObject = true
        };
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(NotSupportedData), "x")];
        var sut = new ExpressionParser(parameters, $"{propertyName}.ToString()", null, config);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        parsedExpression.Should().Be($"x.{propertyName}.ToString()");
    }

    [Theory]
    [MemberData(nameof(GetSupportedPropertyNames))]
    public void PredefinedMethodsHelper_SupportedType_CanCall_Equals(string propertyName)
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(SupportedData), "x")];
        var sut = new ExpressionParser(parameters, $"{propertyName}.Equals(it)", null, ParsingConfig.Default);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        parsedExpression.Should().Be($"x.{propertyName}.Equals(x)");
    }

    [Theory]
    [MemberData(nameof(GetSupportedPropertyNames))]
    public void PredefinedMethodsHelper_SupportedType_CannotCall_GetType(string propertyName)
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(SupportedData), "x")];
        var sut = new ExpressionParser(parameters, $"{propertyName}.GetType()", null, ParsingConfig.Default);

        // Act
        Action action = () => _ = sut.Parse(null);

        // Assert
        action.Should().Throw<ParseException>().WithMessage("Method 'GetType' on type 'Object' is not accessible.");
    }

    public static IEnumerable<object[]> GetSupportedPropertyNames()
    {
        return typeof(SupportedData)
            .GetProperties()
            .Select(p => new object[] { p.Name });
    }

    public static IEnumerable<object[]> GetSupportedPropertyNamesForString1()
    {
        var exclude = new[] { typeof(bool), typeof(char) };
        return typeof(SupportedData)
            .GetProperties()
            .Where(p => !exclude.Contains(p.PropertyType)  )
            .Where(p => p.PropertyType.GetTypeInfo().IsValueType)
            .Where(p => !TypeHelper.IsNullableType(p.PropertyType))
            .Select(p => new object[] { p.Name });
    }

    public static IEnumerable<object[]> GetNotSupportedPropertyNames()
    {
        return typeof(NotSupportedData)
            .GetProperties()
            .Select(p => new object[] { p.Name });
    }

    internal class SupportedData
    {
        public bool B { get; set; }
        public bool? BoolNullable { get; set; }

        public byte By { get; set; }
        public byte? ByteNullable { get; set; }

        public char C { get; set; }
        public char? CharNullable { get; set; }

        public DateTime Dt { get; set; }
        public DateTime? DateTimeNullable { get; set; }

        public DateTimeOffset Dto { get; set; }
        public DateTimeOffset? DateTimeOffsetNullable { get; set; }

        public decimal De { get; set; }
        public decimal? DecimalNullable { get; set; }

        public double D { get; set; }
        public double? DoubleNullable { get; set; }

        public float F { get; set; }
        public float? FloatNullable { get; set; }

        public Guid G { get; set; }
        public Guid? GuidNullable { get; set; }

        public int I { get; set; }
        public int? IntNullable { get; set; }

        public long L { get; set; }
        public long? LongNullable { get; set; }

        public sbyte Sb { get; set; }
        public sbyte? SByteNullable { get; set; }

        public short S { get; set; }
        public short? SNullable { get; set; }

        public string Str { get; set; } = string.Empty;
        public string? StrNullable { get; set; }

        public TimeSpan Ts { get; set; }
        public TimeSpan? TimeSpanNullable { get; set; }

        public uint Ui { get; set; }
        public uint? UIntNullable { get; set; }

        public ulong Ul { get; set; }
        public ulong? ULongNullable { get; set; }

        public Uri Ur { get; set; } = null!;
        public Uri? UriNullable { get; set; }

        public ushort Us { get; set; }
        public ushort? UShortNullable { get; set; }

#if NET6_0_OR_GREATER
        public DateOnly Do { get; set; }
        public DateOnly? DateOnlyNullable { get; set; }

        public TimeOnly To { get; set; }
        public TimeOnly? TimeOnlyNullable { get; set; }
#endif
    }

    internal class NotSupportedData
    {
        public PredefinedMethodsHelperTestClass Test { get; set; } = null!;
        public PredefinedMethodsHelperTestClass? PredefinedMethodsHelperTestClassNullable { get; set; }

        public object Obj { get; set; } = null!;
        public object? ObjectNullable { get; set; }
    }

    [DynamicLinqType]
    internal class PredefinedMethodsHelperTestClass;
}