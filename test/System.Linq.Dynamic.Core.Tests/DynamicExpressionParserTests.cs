using System.Collections.Generic;
using System.Globalization;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using System.Linq.Dynamic.Core.Tests.TestHelpers;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Moq;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests;

public class DynamicExpressionParserTests
{
    [DynamicLinqType]
    public class Z
    {
    }

    public class Foo
    {
        public Foo FooValue { get; set; }

        public string Zero() => null;

        public string One(int x) => null;

        public string Two(int x, int y) => null;
    }

    private class MyClass
    {
        public List<string> MyStrings { get; set; }

        public List<MyClass> MyClasses { get; set; }

        public int Foo()
        {
            return 42;
        }

        public void Bar()
        {
            Name = nameof(Foo);
        }

        public string Name { get; set; }

        public MyClass Child { get; set; }
    }

    private class MyClassCustomTypeProvider : DefaultDynamicLinqCustomTypeProvider
    {
        public override HashSet<Type> GetCustomTypes()
        {
            var customTypes = base.GetCustomTypes();
            customTypes.Add(typeof(MyClass));
            return customTypes;
        }
    }

    private class ComplexParseLambda1Result
    {
        public int? Age;
        public int TotalIncome;
        public string Name;
    }

    [DynamicLinqType]
    public class ComplexParseLambda3Result
    {
        public int? Age { get; set; }
        public int TotalIncome { get; set; }
    }

    public class CustomClassWithStaticMethod
    {
        public static int GetAge(int x) => x;
    }

    public class CustomClassWithMethod
    {
        public int GetAge(int x) => x;
    }

    [DynamicLinqType]
    public class CustomClassWithMethodWithDynamicLinqTypeAttribute
    {
        public int GetAge(int x) => x;
    }

    [DynamicLinqType]
    public interface ICustomInterfaceWithMethodWithDynamicLinqTypeAttribute
    {
        int GetAge(int x);
    }

    public class CustomClassImplementingInterface : ICustomInterfaceWithMethodWithDynamicLinqTypeAttribute
    {
        public int GetAge(int x) => x;
    }

    [DynamicLinqType]
    public class CustomClassWithStaticMethodWithDynamicLinqTypeAttribute
    {
        public static int GetAge(int x) => x;
    }

    public class CustomTextClass
    {
        public CustomTextClass(string origin)
        {
            Origin = origin;
        }

        public string Origin { get; }

        public static implicit operator string(CustomTextClass customTextValue)
        {
            return customTextValue?.Origin;
        }

        public static implicit operator CustomTextClass(string origin)
        {
            return new CustomTextClass(origin);
        }

        public override string ToString()
        {
            return Origin;
        }
    }

    public class CustomClassWithOneWayImplicitConversion
    {
        public CustomClassWithOneWayImplicitConversion(string origin)
        {
            Origin = origin;
        }

        public string Origin { get; }

        public static implicit operator CustomClassWithOneWayImplicitConversion(string origin)
        {
            return new CustomClassWithOneWayImplicitConversion(origin);
        }

        public override string ToString()
        {
            return Origin;
        }
    }

    public class CustomClassWithReversedImplicitConversion
    {
        public CustomClassWithReversedImplicitConversion(string origin)
        {
            Origin = origin;
        }

        public string Origin { get; }

        public static implicit operator string(CustomClassWithReversedImplicitConversion origin)
        {
            return origin.ToString();
        }

        public override string ToString()
        {
            return Origin;
        }
    }

    public class CustomClassWithValueTypeImplicitConversion
    {
        public CustomClassWithValueTypeImplicitConversion(int origin)
        {
            Origin = origin;
        }

        public int Origin { get; }

        public static implicit operator CustomClassWithValueTypeImplicitConversion(int origin)
        {
            return new CustomClassWithValueTypeImplicitConversion(origin);
        }

        public override string ToString()
        {
            return Origin.ToString();
        }
    }

    public class CustomClassWithReversedValueTypeImplicitConversion
    {
        public CustomClassWithReversedValueTypeImplicitConversion(int origin)
        {
            Origin = origin;
        }

        public int Origin { get; }

        public static implicit operator int(CustomClassWithReversedValueTypeImplicitConversion origin)
        {
            return origin.Origin;
        }

        public override string ToString()
        {
            return Origin.ToString();
        }
    }

    public class TestImplicitConversionContainer
    {
        public TestImplicitConversionContainer(
            CustomClassWithOneWayImplicitConversion oneWay,
            CustomClassWithReversedImplicitConversion reversed,
            CustomClassWithValueTypeImplicitConversion valueType,
            CustomClassWithReversedValueTypeImplicitConversion reversedValueType)
        {
            OneWay = oneWay;
            Reversed = reversed;
            ValueType = valueType;
            ReversedValueType = reversedValueType;
        }

        public CustomClassWithOneWayImplicitConversion OneWay { get; }

        public CustomClassWithReversedImplicitConversion Reversed { get; }

        public CustomClassWithValueTypeImplicitConversion ValueType { get; }

        public CustomClassWithReversedValueTypeImplicitConversion ReversedValueType { get; }
    }

    public class TextHolder
    {
        public TextHolder(string name, CustomTextClass note)
        {
            Name = name;
            Note = note;
        }

        public string Name { get; }

        public CustomTextClass Note { get; }

        public override string ToString()
        {
            return Name + " (" + Note + ")";
        }
    }

    public static class StaticHelper
    {
        public static Guid? GetGuid(string name)
        {
            return Guid.NewGuid();
        }

        public static string Filter(string filter)
        {
            return filter;
        }
    }

    public class TestCustomTypeProvider : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
    {
        private HashSet<Type> _customTypes;

        public virtual HashSet<Type> GetCustomTypes()
        {
            if (_customTypes != null)
            {
                return _customTypes;
            }

            _customTypes = new HashSet<Type>(FindTypesMarkedWithDynamicLinqTypeAttribute(new[] { GetType().GetTypeInfo().Assembly }))
            {
                typeof(CustomClassWithStaticMethod),
                typeof(StaticHelper)
            };
            return _customTypes;
        }

        public Dictionary<Type, List<MethodInfo>> GetExtensionMethods()
        {
            var types = GetCustomTypes();

            var list = new List<Tuple<Type, MethodInfo>>();

            foreach (var type in types)
            {
                var extensionMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => x.IsDefined(typeof(ExtensionAttribute), false)).ToList();

                extensionMethods.ForEach(x => list.Add(new Tuple<Type, MethodInfo>(x.GetParameters()[0].ParameterType, x)));
            }

            return list.GroupBy(x => x.Item1, tuple => tuple.Item2).ToDictionary(key => key.Key, methods => methods.ToList());
        }

        public Type ResolveType(string typeName)
        {
            return Type.GetType(typeName);
        }

        public Type ResolveTypeBySimpleName(string typeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            return ResolveTypeBySimpleName(assemblies, typeName);
        }
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_UseParameterizedNamesInDynamicQuery_false_String()
    {
        // Assign
        var config = new ParsingConfig
        {
            UseParameterizedNamesInDynamicQuery = false
        };

        // Act
        var expression = DynamicExpressionParser.ParseLambda<string, bool>(config, true, "s => s == \"x\"");

        // Assert
        ConstantExpression constantExpression = (ConstantExpression)((BinaryExpression)expression.Body).Right;
        object value = constantExpression.Value;

        Check.That(value).IsEqualTo("x");
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_UseParameterizedNamesInDynamicQuery_false_DateTime()
    {
        // Assign
        var config = new ParsingConfig
        {
            UseParameterizedNamesInDynamicQuery = false
        };

        // Act
        var expression = DynamicExpressionParser.ParseLambda<Person, bool>(config, true, "D == \"2022-03-02\"");

        // Assert
        ConstantExpression constantExpression = (ConstantExpression)((BinaryExpression)expression.Body).Right;
        object value = constantExpression.Value;

        Check.That(value).IsEqualTo(new DateTime(2022, 3, 2));
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_UseParameterizedNamesInDynamicQuery_true_Int()
    {
        // Assign
        var config = new ParsingConfig
        {
            UseParameterizedNamesInDynamicQuery = true
        };

        // Act
        var expression = DynamicExpressionParser.ParseLambda<Person, bool>(config, false, "Id = 42");
        var expressionAsString = expression.ToString();

        // Assert
        expressionAsString.Should().Be("Param_0 => (Param_0.Id == value(System.Linq.Dynamic.Core.Parser.WrappedValue`1[System.Int32]).Value)");

        ConstantExpression constantExpression = (ConstantExpression)((MemberExpression)((BinaryExpression)expression.Body).Right).Expression;
        var wrappedObj = constantExpression!.Value;

        PropertyInfo propertyInfo = wrappedObj!.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
        object value = propertyInfo!.GetValue(wrappedObj);

        Check.That(value).IsEqualTo(42);
    }

    [Fact(Skip = "Issue 645")]
    public void DynamicExpressionParser_ParseLambda_UseParameterizedNamesInDynamicQuery_true_DateTime()
    {
        // Assign
        var config = new ParsingConfig
        {
            UseParameterizedNamesInDynamicQuery = true
        };

        // Act
        var expression = DynamicExpressionParser.ParseLambda<Person, bool>(config, false, "D = \"2022-11-16\"");
        var expressionAsString = expression.ToString();

        // Assert
        expressionAsString.Should().Be("Param_0 => (Param_0.D == value(System.Linq.Dynamic.Core.Parser.WrappedValue`1[System.DateTime]).Value)");

        ConstantExpression constantExpression = (ConstantExpression)((MemberExpression)((BinaryExpression)expression.Body).Right).Expression;
        var wrappedObj = constantExpression!.Value;

        PropertyInfo propertyInfo = wrappedObj!.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
        object value = propertyInfo!.GetValue(wrappedObj);

        Check.That(value).IsEqualTo(new DateTime(2022, 11, 16));
    }

    [Theory]
    [InlineData("NullableIntValue", "42")]
    [InlineData("NullableDoubleValue", "42.23")]
    public void DynamicExpressionParser_ParseLambda_UseParameterizedNamesInDynamicQuery_ForNullableProperty_true(string propName, string valueString)
    {
        // Assign
        var culture = CultureInfo.CreateSpecificCulture("en-US");
        var config = new ParsingConfig
        {
            UseParameterizedNamesInDynamicQuery = true,
            NumberParseCulture = culture
        };

        // Act
        var expression = DynamicExpressionParser.ParseLambda<SimpleValuesModel, bool>(config, false, $"{propName} = {valueString}");
        var expressionAsString = expression.ToString();

        // Assert
        var queriedProp = typeof(SimpleValuesModel).GetProperty(propName, BindingFlags.Instance | BindingFlags.Public)!;
        var queriedPropType = queriedProp.PropertyType;
        var queriedPropUnderlyingType = Nullable.GetUnderlyingType(queriedPropType);

        Check.That(expressionAsString).IsEqualTo($"Param_0 => (Param_0.{propName} == {ExpressionString.NullableConversion($"value(System.Linq.Dynamic.Core.Parser.WrappedValue`1[{queriedPropUnderlyingType}]).Value")})");
        dynamic constantExpression = (ConstantExpression)((MemberExpression)((UnaryExpression)((BinaryExpression)expression.Body).Right).Operand).Expression;
        object wrapperObj = constantExpression.Value;

        var propertyInfo = wrapperObj.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public)!;
        var value = propertyInfo.GetValue(wrapperObj);

        value.Should().Be(Convert.ChangeType(valueString, Nullable.GetUnderlyingType(queriedPropType) ?? queriedPropType, culture));
    }

    [Theory]
    [InlineData("Where(x => x.SnowflakeId == {0})")]
    [InlineData("Where(x => x.SnowflakeId = {0})")]
    public void DynamicExpressionParser_ParseLambda_WithStructWithEquality(string query)
    {
        // Assign
        var testList = User.GenerateSampleModels(51);
        var qry = testList.AsQueryable();

        // Act
        var expectedX = (ulong)long.MaxValue + 3;

        query = string.Format(query, expectedX);
        var expression = DynamicExpressionParser.ParseLambda(qry.GetType(), null, query);
        var del = expression.Compile();
        var result = del.DynamicInvoke(qry) as IEnumerable<dynamic>;

        var expected = qry.Where(gg => gg.SnowflakeId == new SnowflakeId(expectedX)).ToList();

        // Assert
        Check.That(result).IsNotNull();
        Check.That(result).HasSize(expected.Count);
        Check.That(result.ToArray()[0]).Equals(expected[0]);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_ToList()
    {
        // Arrange
        var testList = User.GenerateSampleModels(51);
        var qry = testList.AsQueryable();

        // Act
        var query = "OrderBy(gg => gg.Income).ToList()";
        var expression = DynamicExpressionParser.ParseLambda(qry.GetType(), null, query);
        var del = expression.Compile();
        var result = del.DynamicInvoke(qry) as IEnumerable<dynamic>;

        var expected = qry.OrderBy(gg => gg.Income).ToList();

        // Assert
        Check.That(result).IsNotNull();
        Check.That(result).HasSize(expected.Count);
        Check.That(result.ToArray()[0]).Equals(expected[0]);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_Complex_1()
    {
        // Arrange
        var testList = User.GenerateSampleModels(51);
        var qry = testList.AsQueryable();

        var externals = new Dictionary<string, object>
        {
            {"Users", qry}
        };

        // Act
        var query =
            "Users.GroupBy(x => new { x.Profile.Age }).OrderBy(gg => gg.Key.Age).Select(j => new (j.Key.Age, j.Sum(k => k.Income) As TotalIncome))";
        var expression = DynamicExpressionParser.ParseLambda(null, query, externals);
        var del = expression.Compile();
        var result = del.DynamicInvoke() as IEnumerable<dynamic>;

        var expected = qry.GroupBy(x => new { x.Profile.Age }).OrderBy(gg => gg.Key.Age)
            .Select(j => new { j.Key.Age, TotalIncome = j.Sum(k => k.Income) })
            .Select(c => new ComplexParseLambda1Result { Age = c.Age, TotalIncome = c.TotalIncome }).Cast<dynamic>()
            .ToArray();

        // Assert
        Check.That(result).IsNotNull();
        Check.That(result).HasSize(expected.Length);
        Check.That(result.ToArray()[0]).Equals(expected[0]);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_Complex_2()
    {
        // Arrange
        var testList = User.GenerateSampleModels(51);
        var qry = testList.AsQueryable();

        // Act
        var query =
            "GroupBy(x => new { x.Profile.Age }, it).OrderBy(gg => gg.Key.Age).Select(j => new (j.Key.Age, j.Sum(k => k.Income) As TotalIncome))";
        var expression = DynamicExpressionParser.ParseLambda(qry.GetType(), null, query);
        var del = expression.Compile();
        var result = del.DynamicInvoke(qry) as IEnumerable<dynamic>;

        var expected = qry.GroupBy(x => new { x.Profile.Age }, x => x).OrderBy(gg => gg.Key.Age)
            .Select(j => new { j.Key.Age, TotalIncome = j.Sum(k => k.Income) })
            .Select(c => new ComplexParseLambda1Result { Age = c.Age, TotalIncome = c.TotalIncome }).Cast<dynamic>()
            .ToArray();

        // Assert
        Check.That(result).IsNotNull();
        Check.That(result).HasSize(expected.Length);
        Check.That(result.ToArray()[0]).Equals(expected[0]);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_Complex_3()
    {
        var config = new ParsingConfig
        {
            CustomTypeProvider = new TestCustomTypeProvider()
        };

        // Arrange
        var testList = User.GenerateSampleModels(51);
        var qry = testList.AsQueryable();

        var externals = new Dictionary<string, object>
        {
            {"Users", qry}
        };

        // Act
        var stringExpression =
            "Users.GroupBy(x => new { x.Profile.Age }).OrderBy(gg => gg.Key.Age).Select(j => new System.Linq.Dynamic.Core.Tests.DynamicExpressionParserTests+ComplexParseLambda3Result{j.Key.Age, j.Sum(k => k.Income) As TotalIncome})";
        var expression =
            DynamicExpressionParser.ParseLambda(config, null, stringExpression, externals);
        var del = expression.Compile();
        var result = del.DynamicInvoke() as IEnumerable<dynamic>;

        var expected = qry.GroupBy(x => new { x.Profile.Age }).OrderBy(gg => gg.Key.Age)
            .Select(j => new ComplexParseLambda3Result { Age = j.Key.Age, TotalIncome = j.Sum(k => k.Income) })
            .Cast<dynamic>().ToArray();

        // Assert
        Check.That(result).IsNotNull();
        Check.That(result).HasSize(expected.Length);
        Check.That(result.ToArray()[0]).Equals(expected[0]);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_Select_1()
    {
        // Arrange
        var testList = User.GenerateSampleModels(51);
        var qry = testList.AsQueryable();

        var externals = new Dictionary<string, object>
        {
            {"Users", qry}
        };

        // Act
        var query = "Users.Select(j => new User(j.Income As Income))";
        var expression = DynamicExpressionParser.ParseLambda(null, query, externals);
        var del = expression.Compile();
        var result = del.DynamicInvoke();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_Select_2()
    {
        // Arrange
        var testList = User.GenerateSampleModels(5);
        var qry = testList.AsQueryable();

        var externals = new Dictionary<string, object>
        {
            {"Users", qry}
        };

        // Act
        var query = "Users.Select(j => j)";
        var expression = DynamicExpressionParser.ParseLambda(null, query, externals);
        var del = expression.Compile();
        var result = del.DynamicInvoke();

        // Assert
        Assert.NotNull(result);
    }

    // https://github.com/StefH/System.Linq.Dynamic.Core/issues/58
    [Fact]
    public void DynamicExpressionParser_ParseLambda_Issue58()
    {
        // Arrange
        var customTypeProvider = new Mock<IDynamicLinkCustomTypeProvider>();
        customTypeProvider.Setup(c => c.GetCustomTypes()).Returns(new HashSet<Type> { typeof(MyClass) });
        var config = new ParsingConfig
        {
            CustomTypeProvider = customTypeProvider.Object
        };
        var expressionParams = new[]
        {
            Expression.Parameter(typeof(MyClass), "myObj")
        };

        var myClassInstance = new MyClass();
        var invokersMerge = new List<object> { myClassInstance };

        // Act
        var expression = DynamicExpressionParser.ParseLambda(config, false, expressionParams, null, "myObj.Foo()");
        var del = expression.Compile();
        var result = del.DynamicInvoke(invokersMerge.ToArray());

        // Assert
        Check.That(result).Equals(42);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_DuplicateParameterNames_ThrowsException()
    {
        // Arrange
        var parameters = new[]
        {
            Expression.Parameter(typeof(int), "x"),
            Expression.Parameter(typeof(int), "x")
        };

        // Act and Assert
        Check.ThatCode(() => DynamicExpressionParser.ParseLambda(parameters, typeof(bool), "it == 42"))
            .Throws<ParseException>()
            .WithMessage("The identifier 'x' was defined more than once");
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_EmptyParameterList()
    {
        // Arrange
        var pEmpty = new ParameterExpression[] { };

        // Act
        var @delegate = DynamicExpressionParser.ParseLambda(pEmpty, null, "1+2").Compile();
        var result = @delegate.DynamicInvoke() as int?;

        // Assert
        Check.That(result).Equals(3);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_ParameterName()
    {
        // Arrange
        var parameters = new[]
        {
            Expression.Parameter(typeof(int), "x")
        };

        // Assert
        var expressionX = DynamicExpressionParser.ParseLambda(parameters, typeof(bool), "x == 42");
        var expressionIT = DynamicExpressionParser.ParseLambda(parameters, typeof(bool), "it == 42");

        // Assert
        Assert.Equal(typeof(bool), expressionX.Body.Type);
        Assert.Equal(typeof(bool), expressionIT.Body.Type);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_ParameterName_Empty()
    {
        // Arrange
        var parameters = new[]
        {
            Expression.Parameter(typeof(int), "")
        };

        // Assert
        var expression = DynamicExpressionParser.ParseLambda(parameters, typeof(bool), "it == 42");

        // Assert
        Assert.Equal(typeof(bool), expression.Body.Type);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_ParameterName_Null()
    {
        // Arrange
        var parameters = new[]
        {
            Expression.Parameter(typeof(int), null)
        };

        // Assert
        var expression = DynamicExpressionParser.ParseLambda(parameters, typeof(bool), "it == 42");

        // Assert
        Assert.Equal(typeof(bool), expression.Body.Type);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_ParameterExpressionMethodCall_ReturnsIntExpression()
    {
        var expression = DynamicExpressionParser.ParseLambda(true,
            new[] { Expression.Parameter(typeof(int), "x") },
            typeof(int),
            "x + 1");
        Assert.Equal(typeof(int), expression.Body.Type);
    }

    public static object[][] Decimals()
    {
        return new object[][]
        {
            new object[] { "de-DE", "1m", 1f },
            new object[] { "de-DE", "-42,0m", -42m },
            new object[] { "de-DE", "3,215m", 3.215m },

            new object[] { null, "1m", 1f },
            new object[] { null, "-42.0m", -42m },
            new object[] { null, "3.215m", 3.215m }
        };
    }
    [Theory]
    [MemberData(nameof(Decimals))]
    public void DynamicExpressionParser_ParseLambda_Decimal(string culture, string expression, decimal expected)
    {
        // Arrange
        var config = new ParsingConfig();
        if (culture != null)
        {
            config.NumberParseCulture = CultureInfo.CreateSpecificCulture(culture);
        }

        var parameters = new ParameterExpression[0];

        // Act
        var lambda = DynamicExpressionParser.ParseLambda(config, parameters, typeof(decimal), expression);
        var result = lambda.Compile().DynamicInvoke();

        // Assert
        result.Should().Be(expected);
    }

    public static object[][] Floats()
    {
        return new object[][]
        {
            new object[] { "de-DE", "1F", 1f },
            new object[] { "de-DE", "1f", 1f },
            new object[] { "de-DE", "-42f", -42d },
            new object[] { "de-DE", "3,215f", 3.215d },

            new object[] { null, "1F", 1f },
            new object[] { null, "1f", 1f },
            new object[] { null, "-42f", -42d },
            new object[] { null, "3.215f", 3.215d },
        };
    }
    [Theory]
    [MemberData(nameof(Floats))]
    public void DynamicExpressionParser_ParseLambda_Float(string culture, string expression, float expected)
    {
        // Arrange
        var config = new ParsingConfig();
        if (culture != null)
        {
            config.NumberParseCulture = CultureInfo.CreateSpecificCulture(culture);
        }

        var parameters = new ParameterExpression[0];

        // Act
        var lambda = DynamicExpressionParser.ParseLambda(config, parameters, typeof(float), expression);
        var result = lambda.Compile().DynamicInvoke();

        // Assert
        result.Should().Be(expected);
    }

    public static IEnumerable<object[]> Doubles()
    {
        return new object[][]
        {
            new object[] { "de-DE", "1D", 1d },
            new object[] { "de-DE", "1d", 1d },
            new object[] { "de-DE", "-42d", -42d },
            new object[] { "de-DE", "3,215d", 3.215d },
            new object[] { "de-DE", "1,2345E-4", 0.00012345d },
            new object[] { "de-DE", "1,2345E4", 12345d },

            new object[] { null, "1D", 1d },
            new object[] { null, "1d", 1d },
            new object[] { null, "-42d", -42d },
            new object[] { null, "3.215d", 3.215d },
            new object[] { null, "1.2345E-4", 0.00012345d },
            new object[] { null, "1.2345E4", 12345d }
        };
    }
    [Theory]
    [MemberData(nameof(Doubles))]
    public void DynamicExpressionParser_ParseLambda_Double(string culture, string expression, double expected)
    {
        // Arrange
        var config = new ParsingConfig();
        if (culture != null)
        {
            config.NumberParseCulture = CultureInfo.CreateSpecificCulture(culture);
        }

        var parameters = new ParameterExpression[0];

        // Act
        var lambda = DynamicExpressionParser.ParseLambda(config, parameters, typeof(double), expression);
        var result = lambda.Compile().DynamicInvoke();

        // Assert
        result.Should().Be(expected);
    }

    public class EntityDbo
    {
        public string Name { get; set; }
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_StringLiteral_WithADot_As_Arg()
    {
        // Act
        var expression = DynamicExpressionParser.ParseLambda(typeof(EntityDbo), typeof(bool), "Name == @0", "System.Int32");
        var del = expression.Compile();
        var result = del.DynamicInvoke(new EntityDbo { Name = "System.Int32" });

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_StringLiteral_WithADot_In_Expression()
    {
        // Act
        var expression = DynamicExpressionParser.ParseLambda(typeof(EntityDbo), typeof(bool), "Name == \"System.Int32\"");
        var del = expression.Compile();
        var result = del.DynamicInvoke(new EntityDbo { Name = "System.Int32" });

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_StringLiteral_ReturnsBooleanLambdaExpression()
    {
        var expression = DynamicExpressionParser.ParseLambda(
            new[] { Expression.Parameter(typeof(string), "Property1") }, typeof(bool), "Property1 == \"test\"");
        Assert.Equal(typeof(bool), expression.Body.Type);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_StringLiteralEmpty_ReturnsBooleanLambdaExpression()
    {
        var expression = DynamicExpressionParser.ParseLambda(
            new[] { Expression.Parameter(typeof(string), "Property1") }, typeof(bool), "Property1 == \"\"");
        Assert.Equal(typeof(bool), expression.Body.Type);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_Config_StringLiteralEmpty_ReturnsBooleanLambdaExpression()
    {
        var config = new ParsingConfig();
        var expression = DynamicExpressionParser.ParseLambda(
            new[] { Expression.Parameter(typeof(string), "Property1") }, typeof(bool), "Property1 == \"\"");
        Assert.Equal(typeof(bool), expression.Body.Type);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_StringLiteralEmbeddedQuote_ReturnsBooleanLambdaExpression()
    {
        // Act
        var expression = DynamicExpressionParser.ParseLambda(
            new[] { Expression.Parameter(typeof(string), "Property1") },
            typeof(bool),
            string.Format("Property1 == {0}", "\"test \\\"string\""));

        var rightValue = ((BinaryExpression)expression.Body).Right.ToString();
        Assert.Equal(typeof(bool), expression.Body.Type);
        Assert.Equal("\"test \"string\"", rightValue);
    }

    /// <summary>
    /// @see https://github.com/StefH/System.Linq.Dynamic.Core/issues/294
    /// </summary>
    [Fact(Skip = "Fails on EntityFramework.DynamicLinq.Tests with 'An unexpected exception occurred while binding a dynamic operation'")]
    public void DynamicExpressionParser_ParseLambda_MultipleLambdas()
    {
        var users = new[]
        {
            new { name = "Juan", age = 25 },
            new { name = "Juan", age = 25 },
            new { name = "David", age = 12 },
            new { name = "Juan", age = 25 },
            new { name = "Juan", age = 4 },
            new { name = "Pedro", age = 2 },
            new { name = "Juan", age = 25 }
        }.ToList();

        IQueryable query;

        // One lambda
        var res1 = "[{\"Key\":{\"name\":\"Juan\"},\"nativeAggregates\":{\"ageSum\":104},\"Grouping\":[{\"name\":\"Juan\",\"age\":25},{\"name\":\"Juan\",\"age\":25},{\"name\":\"Juan\",\"age\":25},{\"name\":\"Juan\",\"age\":4},{\"name\":\"Juan\",\"age\":25}]},{\"Key\":{\"name\":\"David\"},\"nativeAggregates\":{\"ageSum\":12},\"Grouping\":[{\"name\":\"David\",\"age\":12}]},{\"Key\":{\"name\":\"Pedro\"},\"nativeAggregates\":{\"ageSum\":2},\"Grouping\":[{\"name\":\"Pedro\",\"age\":2}]}]";
        query = users.AsQueryable();
        query = query.GroupBy("new(name as name)", "it");
        query = query.Select("new (it.Key as Key, new(it.Sum(x => x.age) as ageSum) as nativeAggregates, it as Grouping)");
        Assert.Equal(res1, Newtonsoft.Json.JsonConvert.SerializeObject(query));

        // Multiple lambdas
        var res2 = "[{\"Key\":{\"name\":\"Juan\"},\"nativeAggregates\":{\"ageSum\":0,\"ageSum2\":104},\"Grouping\":[{\"name\":\"Juan\",\"age\":25},{\"name\":\"Juan\",\"age\":25},{\"name\":\"Juan\",\"age\":25},{\"name\":\"Juan\",\"age\":4},{\"name\":\"Juan\",\"age\":25}]},{\"Key\":{\"name\":\"David\"},\"nativeAggregates\":{\"ageSum\":0,\"ageSum2\":12},\"Grouping\":[{\"name\":\"David\",\"age\":12}]},{\"Key\":{\"name\":\"Pedro\"},\"nativeAggregates\":{\"ageSum\":0,\"ageSum2\":2},\"Grouping\":[{\"name\":\"Pedro\",\"age\":2}]}]";
        query = users.AsQueryable();
        query = query.GroupBy("new(name as name)", "it");
        query = query.Select("new (it.Key as Key, new(it.Sum(x => x.age > 25 ? 1 : 0) as ageSum, it.Sum(x => x.age) as ageSum2) as nativeAggregates, it as Grouping)");
        Assert.Equal(res2, Newtonsoft.Json.JsonConvert.SerializeObject(query));
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_StringLiteralStartEmbeddedQuote_ReturnsBooleanLambdaExpression()
    {
        // Act
        var expression = DynamicExpressionParser.ParseLambda(
            new[] { Expression.Parameter(typeof(string), "Property1") },
            typeof(bool),
            string.Format("Property1 == {0}", "\"\\\"test\""));

        var rightValue = ((BinaryExpression)expression.Body).Right.ToString();
        Assert.Equal(typeof(bool), expression.Body.Type);
        Assert.Equal("\"\"test\"", rightValue);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_StringLiteral_MissingClosingQuote()
    {
        var expectedRightValue = "\"test\\\"";

        Assert.Throws<ParseException>(() => DynamicExpressionParser.ParseLambda(
            new[] { Expression.Parameter(typeof(string), "Property1") },
            typeof(bool),
            string.Format("Property1 == {0}", expectedRightValue)));
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_StringLiteralEscapedBackslash_ReturnsBooleanLambdaExpression()
    {
        // Act
        var expression = DynamicExpressionParser.ParseLambda(
            new[] { Expression.Parameter(typeof(string), "Property1") },
            typeof(bool),
            string.Format("Property1 == {0}", "\"test\\\\string\""));

        var rightValue = ((BinaryExpression)expression.Body).Right.ToString();
        Assert.Equal(typeof(Boolean), expression.Body.Type);
        Assert.Equal("\"test\\string\"", rightValue);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_StringLiteralEscapedNewline_ReturnsBooleanLambdaExpression()
    {
        // Act
        var expression = DynamicExpressionParser.ParseLambda(
            new[] { Expression.Parameter(typeof(string), "Property1") },
            typeof(bool),
            string.Format("Property1 == {0}", "\"test\\\\new\""));

        var rightValue = ((BinaryExpression)expression.Body).Right.ToString();
        Assert.Equal(typeof(Boolean), expression.Body.Type);
        Assert.Equal("\"test\\new\"", rightValue);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_StringLiteral_Backslash()
    {
        // Assign
        var expectedRightValue = "0";

        //Act
        var expression = DynamicExpressionParser.ParseLambda(
            new[] { Expression.Parameter(typeof(string), "Property1") },
            typeof(Boolean),
            string.Format("{0} >= {1}", "Property1.IndexOf(\"\\\\\")", expectedRightValue));

        var leftValue = ((BinaryExpression)expression.Body).Left.ToString();
        var rightValue = ((BinaryExpression)expression.Body).Right.ToString();

        // Assert
        Assert.Equal(typeof(Boolean), expression.Body.Type);
        Assert.Equal("Property1.IndexOf(\"\\\")", leftValue);
        Assert.Equal(expectedRightValue, rightValue);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_StringLiteral_QuotationMark()
    {
        var expectedRightValue = "0";
        var expression = DynamicExpressionParser.ParseLambda(
            new[] { Expression.Parameter(typeof(string), "Property1") },
            typeof(Boolean),
            string.Format("{0} >= {1}", "Property1.IndexOf(\"\\\"\")", expectedRightValue));

        var leftValue = ((BinaryExpression)expression.Body).Left.ToString();
        var rightValue = ((BinaryExpression)expression.Body).Right.ToString();
        Assert.Equal(typeof(Boolean), expression.Body.Type);
        Assert.Equal("Property1.IndexOf(\"\"\")", leftValue);
        Assert.Equal(expectedRightValue, rightValue);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_TupleToStringMethodCall_ReturnsStringLambdaExpression()
    {
        var expression = DynamicExpressionParser.ParseLambda(
            typeof(Tuple<int>),
            typeof(string),
            "it.ToString()");
        Assert.Equal(typeof(string), expression.ReturnType);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_CustomStaticMethod_WhenClassIsReturnedByCustomTypeProvider_ShouldWorkCorrect()
    {
        // Assign
        var config = new ParsingConfig
        {
            CustomTypeProvider = new TestCustomTypeProvider()
        };

        var context = new CustomClassWithStaticMethod();
        var expression = $"{nameof(CustomClassWithStaticMethod)}.{nameof(CustomClassWithStaticMethod.GetAge)}(10)";

        // Act
        var lambdaExpression = DynamicExpressionParser.ParseLambda(config, typeof(CustomClassWithStaticMethod), null, expression);
        var del = lambdaExpression.Compile();
        var result = (int)del.DynamicInvoke(context);

        // Assert
        Check.That(result).IsEqualTo(10);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_CustomStaticMethod_WhenClassHasDynamicLinqTypeAttribute_ShouldWorkCorrect()
    {
        // Assign
        var context = new CustomClassWithStaticMethodWithDynamicLinqTypeAttribute();
        var expression = $"{nameof(CustomClassWithStaticMethodWithDynamicLinqTypeAttribute)}.{nameof(CustomClassWithStaticMethodWithDynamicLinqTypeAttribute.GetAge)}(10)";

        // Act
        var lambdaExpression = DynamicExpressionParser.ParseLambda(typeof(CustomClassWithStaticMethodWithDynamicLinqTypeAttribute), null, expression);
        var del = lambdaExpression.Compile();
        var result = (int?)del.DynamicInvoke(context);

        // Assert
        Check.That(result).IsEqualTo(10);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_CustomMethod_WhenClassHasDynamicLinqTypeAttribute_ShouldWorkCorrect()
    {
        // Assign
        var context = new CustomClassWithMethodWithDynamicLinqTypeAttribute();
        var expression = $"{nameof(CustomClassWithMethodWithDynamicLinqTypeAttribute.GetAge)}(10)";

        // Act
        var lambdaExpression = DynamicExpressionParser.ParseLambda(typeof(CustomClassWithMethodWithDynamicLinqTypeAttribute), null, expression);
        var del = lambdaExpression.Compile();
        var result = (int?)del.DynamicInvoke(context);

        // Assert
        Check.That(result).IsEqualTo(10);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_CustomInterface_WhenInterfaceHasDynamicLinqTypeAttribute_ShouldWorkCorrect()
    {
        // Arrange
        var context = new CustomClassImplementingInterface();
        var expression = $"{nameof(ICustomInterfaceWithMethodWithDynamicLinqTypeAttribute.GetAge)}(10)";

        // Act
        var lambdaExpression = DynamicExpressionParser.ParseLambda(typeof(ICustomInterfaceWithMethodWithDynamicLinqTypeAttribute), null, expression);
        var del = lambdaExpression.Compile();
        var result = (int?)del.DynamicInvoke(context);

        // Assert
        result.Should().Be(10);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_CustomMethod_WhenClassDoesNotHaveDynamicLinqTypeAttribute_ShouldThrowException()
    {
        // Assign
        var expression = $"{nameof(CustomClassWithMethod.GetAge)}(10)";

        // Act
        Action action = () => DynamicExpressionParser.ParseLambda(typeof(CustomClassWithMethod), null, expression);

        // Assert
        action.Should().Throw<ParseException>().WithMessage("Methods on type 'CustomClassWithMethod' are not accessible");
    }

    // [Fact]
    public void DynamicExpressionParser_ParseLambda_With_InnerStringLiteral()
    {
        // Assign
        var originalTrueValue = "simple + \"quoted\"";
        var doubleQuotedTrueValue = "simple + \"\"quoted\"\"";
        var expressionText = $"iif(1>0, \"{doubleQuotedTrueValue}\", \"false\")";

        // Act
        var lambda = DynamicExpressionParser.ParseLambda(typeof(string), null, expressionText);
        var del = lambda.Compile();
        var result = del.DynamicInvoke(string.Empty);

        // Assert
        Check.That(result).IsEqualTo(originalTrueValue);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_With_Guid_Equals_Null()
    {
        // Arrange
        var user = new User();
        var guidEmpty = Guid.Empty;
        var someId = Guid.NewGuid();
        var expressionText = $"iif(@0.Id == null, @0.Id == Guid.Parse(\"{someId}\"), Id == Id)";

        // Act
        var lambda = DynamicExpressionParser.ParseLambda(typeof(User), null, expressionText, user);
        var boolLambda = lambda as Expression<Func<User, bool>>;
        Assert.NotNull(boolLambda);

        var del = lambda.Compile();
        var result = (bool)del.DynamicInvoke(user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_With_Null_Equals_Guid()
    {
        // Arrange
        var user = new User();
        var someId = Guid.NewGuid();
        var expressionText = $"iif(null == @0.Id, @0.Id == Guid.Parse(\"{someId}\"), Id == Id)";

        // Act
        var lambda = DynamicExpressionParser.ParseLambda(typeof(User), null, expressionText, user);
        var boolLambda = lambda as Expression<Func<User, bool>>;
        Assert.NotNull(boolLambda);

        var del = lambda.Compile();
        var result = (bool)del.DynamicInvoke(user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_With_DateTime_Equals_String()
    {
        // Arrange
        var someDateTime = "2022-03-02";
        var user = new Person
        {
            D = new DateTime(2022, 3, 2)
        };
        var expressionText = $"D == \"{someDateTime}\"";

        // Act
        var lambda = DynamicExpressionParser.ParseLambda(typeof(Person), null, expressionText, user);
        var dtLambda = lambda as Expression<Func<Person, bool>>;
        Assert.NotNull(dtLambda);

        var del = lambda.Compile();
        var result = (bool)del.DynamicInvoke(user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_With_DateTime_UtcNow_AddHours_ToString_Issue675()
    {
        // Arrange
        var expressionText = "DateTime.UtcNow.AddHours(2).ToString(\"o\")";
        var parameterExpression = Expression.Parameter(typeof(string));

        // Act
        var lambda = DynamicExpressionParser.ParseLambda(new[] { parameterExpression }, typeof(object), expressionText);

        // Assert
        lambda.Should().NotBeNull();
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_With_Guid_Equals_String()
    {
        // Arrange
        var someId = Guid.NewGuid();
        var anotherId = Guid.NewGuid();
        var user = new User
        {
            Id = someId
        };
        var guidEmpty = Guid.Empty;
        var expressionText = $"iif(@0.Id == \"{someId}\", Guid.Parse(\"{guidEmpty}\"), Guid.Parse(\"{anotherId}\"))";

        // Act
        var lambda = DynamicExpressionParser.ParseLambda(typeof(User), null, expressionText, user);
        var guidLambda = lambda as Expression<Func<User, Guid>>;
        Assert.NotNull(guidLambda);

        var del = lambda.Compile();
        var result = (Guid)del.DynamicInvoke(user);

        // Assert
        Assert.Equal(guidEmpty, result);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_With_Concat_String_CustomType()
    {
        // Arrange
        var name = "name1";
        var note = "note1";
        var textHolder = new TextHolder(name, note);
        var expressionText = "Name + \" (\" + Note + \")\"";

        // Act 1
        var lambda = DynamicExpressionParser.ParseLambda(typeof(TextHolder), null, expressionText, textHolder);
        var stringLambda = lambda as Expression<Func<TextHolder, string>>;

        // Assert 1
        Assert.NotNull(stringLambda);

        // Act 2
        var del = lambda.Compile();
        var result = (string)del.DynamicInvoke(textHolder);

        // Assert 2
        Assert.Equal("name1 (note1)", result);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_With_Concat_CustomType_String()
    {
        // Arrange
        var name = "name1";
        var note = "note1";
        var textHolder = new TextHolder(name, note);
        var expressionText = "Note + \" (\" + Name + \")\"";

        // Act 1
        var lambda = DynamicExpressionParser.ParseLambda(typeof(TextHolder), null, expressionText, textHolder);
        var stringLambda = lambda as Expression<Func<TextHolder, string>>;

        // Assert 1
        Assert.NotNull(stringLambda);

        // Act 2
        var del = lambda.Compile();
        var result = (string)del.DynamicInvoke(textHolder);

        // Assert 2
        Assert.Equal("note1 (name1)", result);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_With_One_Way_Implicit_Conversions()
    {
        // Arrange
        var testString = "test";
        var testInt = 6;
        var container = new TestImplicitConversionContainer(testString, new CustomClassWithReversedImplicitConversion(testString), testInt, new CustomClassWithReversedValueTypeImplicitConversion(testInt));

        var expressionTextString = $"OneWay == \"{testString}\"";
        var expressionTextReversed = $"Reversed == \"{testString}\"";
        var expressionTextValueType = $"ValueType == {testInt}";
        var expressionTextReversedValueType = $"ReversedValueType == {testInt}";

        var invertedExpressionTextString = $"\"{testString}\" == OneWay";
        var invertedExpressionTextReversed = $"\"{testString}\" == Reversed";
        var invertedExpressionTextValueType = $"{testInt} == ValueType";
        var invertedExpressionTextReversedValueType = $"{testInt} == ReversedValueType";

        // Act 1
        var lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, expressionTextString);

        // Assert 1
        Assert.NotNull(lambda);

        // Act 2
        lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, expressionTextReversed);

        // Assert 2
        Assert.NotNull(lambda);

        // Act 3
        lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, expressionTextValueType);

        // Assert 3
        Assert.NotNull(lambda);

        // Act 4
        lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, expressionTextReversedValueType);

        // Assert 4
        Assert.NotNull(lambda);

        // Act 5
        lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, invertedExpressionTextString);

        // Assert 5
        Assert.NotNull(lambda);

        // Act 6
        lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, invertedExpressionTextReversed);

        // Assert 6
        Assert.NotNull(lambda);

        // Act 7
        lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, invertedExpressionTextValueType);

        // Assert 7
        Assert.NotNull(lambda);

        // Act 8
        lambda = DynamicExpressionParser.ParseLambda<TestImplicitConversionContainer, bool>(ParsingConfig.Default, false, invertedExpressionTextReversedValueType);

        // Assert 8
        Assert.NotNull(lambda);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_Operator_Less_Greater_With_Guids()
    {
        var config = new ParsingConfig
        {
            CustomTypeProvider = new TestCustomTypeProvider()
        };

        // Arrange
        var someId = Guid.NewGuid();
        var anotherId = Guid.NewGuid();
        var user = new User
        {
            Id = someId
        };
        var guidEmpty = Guid.Empty;
        var expressionText = $"iif(@0.Id == StaticHelper.GetGuid(\"name\"), Guid.Parse(\"{guidEmpty}\"), Guid.Parse(\"{anotherId}\"))";

        // Act
        var lambda = DynamicExpressionParser.ParseLambda(config, typeof(User), null, expressionText, user);
        var guidLambda = (Expression<Func<User, Guid>>)lambda;

        var del = guidLambda.Compile();
        var result = (Guid?)del.DynamicInvoke(user);

        // Assert
        result.Should().Be(anotherId);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_CustomType_Method_With_ExpressionString()
    {
        // Arrange
        var config = new ParsingConfig
        {
            CustomTypeProvider = new TestCustomTypeProvider()
        };

        var user = new User();

        // Act : char
        var expressionTextChar = "StaticHelper.Filter(\"C == 'x'\")";
        var lambdaChar = DynamicExpressionParser.ParseLambda(config, typeof(User), null, expressionTextChar, user);
        var funcChar = (Expression<Func<User, string>>)lambdaChar;

        var delegateChar = funcChar.Compile();
        var resultChar = (string?)delegateChar.DynamicInvoke(user);

        // Assert : int
        resultChar.Should().Be("C == 'x'");

        // Act : int
        var expressionTextIncome = "StaticHelper.Filter(\"Income == 5\")";
        var lambdaIncome = DynamicExpressionParser.ParseLambda(config, typeof(User), null, expressionTextIncome, user);
        var funcIncome = (Expression<Func<User, string>>)lambdaIncome;

        var delegateIncome = funcIncome.Compile();
        var resultIncome = (string?)delegateIncome.DynamicInvoke(user);

        // Assert : int
        resultIncome.Should().Be("Income == 5");

        // Act : string
        var expressionTextUserName = "StaticHelper.Filter(\"UserName == \"\"x\"\"\")";
        var lambdaUserName = DynamicExpressionParser.ParseLambda(config, typeof(User), null, expressionTextUserName, user);
        var funcUserName = (Expression<Func<User, string>>)lambdaUserName;

        var delegateUserName = funcUserName.Compile();
        var resultUserName = (string?)delegateUserName.DynamicInvoke(user);

        // Assert : string
        resultUserName.Should().Be(@"UserName == ""x""""""");
    }

    [Theory]
    [InlineData(true, "c => c.Age == 8", "c => (c.Age == 8)")]
    [InlineData(true, "c => c.Name == \"test\"", "c => (c.Name == \"test\")")]
    [InlineData(false, "c => c.Age == 8", "Param_0 => (Param_0.Age == 8)")]
    [InlineData(false, "c => c.Name == \"test\"", "Param_0 => (Param_0.Name == \"test\")")]
    public void DynamicExpressionParser_ParseLambda_RenameParameterExpression(bool renameParameterExpression, string expressionAsString, string expected)
    {
        // Arrange
        var config = new ParsingConfig
        {
            RenameParameterExpression = renameParameterExpression
        };

        // Act
        var expression = DynamicExpressionParser.ParseLambda<ComplexParseLambda1Result, bool>(config, true, expressionAsString);
        var result = expression.ToString();

        // Assert
        Check.That(result).IsEqualTo(expected);
    }

    [Theory]
    [InlineData("c => c.Age == 8", "([a-z]{16}) =\\> \\(\\1\\.Age == 8\\)")]
    [InlineData("c => c.Name == \"test\"", "([a-z]{16}) =\\> \\(\\1\\.Name == \"test\"\\)")]
    public void DynamicExpressionParser_ParseLambda_RenameEmptyParameterExpressionNames(string expressionAsString, string expected)
    {
        // Arrange
        var config = new ParsingConfig
        {
            RenameEmptyParameterExpressionNames = true
        };

        // Act
        var expression = DynamicExpressionParser.ParseLambda<ComplexParseLambda1Result, bool>(config, true, expressionAsString);
        var result = expression.ToString();

        // Assert
        Check.That(result).Matches(expected);
    }

    [Theory]
    [InlineData(@"p0.Equals(""Testing"", 3)", "testinG", true)]
    [InlineData(@"p0.Equals(""Testing"", StringComparison.InvariantCultureIgnoreCase)", "testinG", true)]
    public void DynamicExpressionParser_ParseLambda_SupportEnumerationStringComparison(string expressionAsString, string testValue, bool expectedResult)
    {
        // Arrange
        var p0 = Expression.Parameter(typeof(string), "p0");

        // Act
        var expression = DynamicExpressionParser.ParseLambda(new[] { p0 }, typeof(bool), expressionAsString);
        var del = expression.Compile();
        var result = del.DynamicInvoke(testValue) as bool?;

        // Assert
        Check.That(result).IsEqualTo(expectedResult);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_NullPropagation_InstanceMethod_0_Arguments()
    {
        // Arrange
        var customTypeProvider = new Mock<IDynamicLinkCustomTypeProvider>();
        customTypeProvider.Setup(c => c.GetCustomTypes()).Returns(new HashSet<Type> { typeof(Foo) });
        var config = new ParsingConfig
        {
            CustomTypeProvider = customTypeProvider.Object
        };
        var expression = "np(FooValue.Zero().Length)";

        // Act
        var lambdaExpression = DynamicExpressionParser.ParseLambda(config, typeof(Foo), null, expression, new Foo());

        // Assert
#if NETCOREAPP3_1
        lambdaExpression.ToString().Should().Be("Param_0 => IIF((((Param_0 != null) AndAlso (Param_0.FooValue != null)) AndAlso (Param_0.FooValue.Zero() != null)), Convert(Param_0.FooValue.Zero().Length, Nullable`1), null)");
#else
        lambdaExpression.ToString().Should().Be("Param_0 => IIF((((Param_0 != null) AndAlso (Param_0.FooValue != null)) AndAlso (Param_0.FooValue.Zero() != null)), Convert(Param_0.FooValue.Zero().Length), null)");
#endif
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_NullPropagation_InstanceMethod_1_Argument()
    {
        // Arrange
        var customTypeProvider = new Mock<IDynamicLinkCustomTypeProvider>();
        customTypeProvider.Setup(c => c.GetCustomTypes()).Returns(new HashSet<Type> { typeof(Foo) });
        var config = new ParsingConfig
        {
            CustomTypeProvider = customTypeProvider.Object
        };
        var expression = "np(FooValue.One(1).Length)";

        // Act
        var lambdaExpression = DynamicExpressionParser.ParseLambda(config, typeof(Foo), null, expression, new Foo());

        // Assert
#if NETCOREAPP3_1
        lambdaExpression.ToString().Should().Be("Param_0 => IIF((((Param_0 != null) AndAlso (Param_0.FooValue != null)) AndAlso (Param_0.FooValue.One(1) != null)), Convert(Param_0.FooValue.One(1).Length, Nullable`1), null)");
#else
        lambdaExpression.ToString().Should().Be("Param_0 => IIF((((Param_0 != null) AndAlso (Param_0.FooValue != null)) AndAlso (Param_0.FooValue.One(1) != null)), Convert(Param_0.FooValue.One(1).Length), null)");
#endif
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_NullPropagation_InstanceMethod_2_Arguments()
    {
        // Arrange
        var customTypeProvider = new Mock<IDynamicLinkCustomTypeProvider>();
        customTypeProvider.Setup(c => c.GetCustomTypes()).Returns(new HashSet<Type> { typeof(Foo) });
        var config = new ParsingConfig
        {
            CustomTypeProvider = customTypeProvider.Object
        };
        var expression = "np(FooValue.Two(1, 42).Length)";

        // Act
        var lambdaExpression = DynamicExpressionParser.ParseLambda(config, typeof(Foo), null, expression, new Foo());

        // Assert
#if NETCOREAPP3_1
        lambdaExpression.ToString().Should().Be("Param_0 => IIF((((Param_0 != null) AndAlso (Param_0.FooValue != null)) AndAlso (Param_0.FooValue.Two(1, 42) != null)), Convert(Param_0.FooValue.Two(1, 42).Length, Nullable`1), null)");
#else
        lambdaExpression.ToString().Should().Be("Param_0 => IIF((((Param_0 != null) AndAlso (Param_0.FooValue != null)) AndAlso (Param_0.FooValue.Two(1, 42) != null)), Convert(Param_0.FooValue.Two(1, 42).Length), null)");
#endif
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_NullPropagation_MethodCallExpression()
    {
        // Arrange
        var myClass = new MyClass();
        var dataSource = new { MyClasses = new[] { myClass, null } };

        var expressionText = "np(MyClasses.FirstOrDefault())";

        // Act
        var expression = DynamicExpressionParser.ParseLambda(ParsingConfig.Default, dataSource.GetType(), typeof(MyClass), expressionText);
        var del = expression.Compile();
        var result = del.DynamicInvoke(dataSource) as MyClass;

        // Assert
        result.Should().Be(myClass);
    }

    [Theory]
    [InlineData("np(MyClasses.FirstOrDefault().Name)")]
    [InlineData("np(MyClasses.FirstOrDefault(Name == \"a\").Name)")]
    public void DynamicExpressionParser_ParseLambda_NullPropagation_MethodCallExpression_With_Property(string expressionText)
    {
        // Arrange
        var dataSource = new MyClass();

        // Act
        var expression = DynamicExpressionParser.ParseLambda(ParsingConfig.Default, dataSource.GetType(), typeof(string), expressionText);
        var del = expression.Compile();
        var result = del.DynamicInvoke(dataSource) as string;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_ActionDelegate_VoidMethodCallExpression()
    {
        // Arrange
        var dataSource = new MyClass();
        var expressionText = "it.Bar()";
        var parsingConfig = new ParsingConfig { CustomTypeProvider = new MyClassCustomTypeProvider() };
        dataSource.Name.Should().BeNull();

        // Act
        var expression = DynamicExpressionParser.ParseLambda(typeof(Action<MyClass>), parsingConfig, dataSource.GetType(), null, expressionText);
        var del = expression.Compile();
        del.DynamicInvoke(dataSource);

        // Assert
        dataSource.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_String_TrimEnd_0_Parameters()
    {
        // Act
        var expression = DynamicExpressionParser.ParseLambda<string, bool>(new ParsingConfig(), false, "TrimEnd().EndsWith(@0)", "test");

        var @delegate = expression.Compile();

        var result = (bool)@delegate.DynamicInvoke("This is a test  ");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_String_TrimEnd_1_Parameter()
    {
        // Act
        var expression = DynamicExpressionParser.ParseLambda<string, bool>(new ParsingConfig(), false, "TrimEnd('.').EndsWith(@0)", "test");

        var @delegate = expression.Compile();

        var result = (bool)@delegate.DynamicInvoke("This is a test...");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_GenericExtensionMethod()
    {
        // Arrange
        var testList = User.GenerateSampleModels(51);
        var config = new ParsingConfig
        {
            CustomTypeProvider = new DefaultDynamicLinqCustomTypeProviderForGenericExtensionMethod(),
            PrioritizePropertyOrFieldOverTheType = true
        };

        // Act
        var query = "x => MethodsItemExtension.Functions.EfCoreCollate(x.UserName, \"tlh-KX\")==\"User4\" || MethodsItemExtension.Functions.EfCoreCollate(x.UserName, \"tlh-KX\")==\"User2\"";
        var expression = DynamicExpressionParser.ParseLambda<User, bool>(config, false, query);
        var del = expression.Compile();

        var result = Enumerable.Where(testList, del);

        var expected = testList.Where(x => new string[] { "User4", "User2" }.Contains(x.UserName)).ToList();

        // Assert
        Check.That(result).IsNotNull();
        Check.That(result).HasSize(expected.Count);
        Check.That(result).Equals(expected);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_Action()
    {
        // Arrange
        var action = (Action<int>)DynamicExpressionParser.ParseLambda(typeof(Action<int>), new[] { Expression.Parameter(typeof(int), "x") }, typeof(int), "x + 1").Compile();

        // Act
        action(3);
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_Func()
    {
        // Arrange
        var func = (Func<int, int>)DynamicExpressionParser.ParseLambda(
            typeof(Func<int, int>),
            new[]
            {
                Expression.Parameter(typeof(int), "x")
            },
            typeof(int),
            "x + 1"
        ).Compile();

        // Act
        var result = func(4);

        // Assert
        result.Should().Be(5);
    }

    [Theory]
    [InlineData("value", "value != null && value == 1", 1, true)]
    [InlineData("value", "value != null && value == 1", 5, false)]
    [InlineData("x", "value != null && value == 1", 1, true)]
    [InlineData("x", "value != null && value == 1", 5, false)]
    [InlineData(null, "value != null && value == 1", 1, true)]
    [InlineData(null, "value != null && value == 1", 5, false)]
    [InlineData("value", "value => value != null && value == 1", 1, true)]
    [InlineData("value", "value => value != null && value == 1", 5, false)]
    public void DynamicExpressionParser_ParseLambda_Func2(string? paramName, string test, int? input, bool expected)
    {
        // Arrange
        var nullableType = typeof(int?);
        var delegateType = typeof(Func<,>).MakeGenericType(nullableType, typeof(bool));
        var valueParameter = paramName is not null ? Expression.Parameter(nullableType, paramName) : Expression.Parameter(nullableType);

        // Act 1
        var expression = DynamicExpressionParser.ParseLambda(
            delegateType,
            new ParsingConfig(),
            new[] { valueParameter },
            typeof(bool),
            test
        );

        // Act 2
        var compiledExpression = expression.Compile();
        var result = compiledExpression.DynamicInvoke(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void DynamicExpressionParser_ParseComparisonOperator_DynamicClass_For_String()
    {
        // Arrange
        var cc = "firstValue".ToCharArray();
        var field = new
        {
            Name = "firstName",
            Value = new string(cc)
        };

        var props = new DynamicProperty[]
        {
            new DynamicProperty(field.Name, typeof(string))
        };

        var type = DynamicClassFactory.CreateType(props);

        var dynamicClass = (DynamicClass)Activator.CreateInstance(type);
        dynamicClass.SetDynamicPropertyValue(field.Name, field.Value);

        // Act 1
        var parameters = new[] { Expression.Parameter(type, "x") };
        var expression = DynamicExpressionParser.ParseLambda(null, new ParsingConfig(), true, parameters, null, "firstName eq \"firstValue\"");

        var @delegate = expression.Compile();

        var result = (bool)@delegate.DynamicInvoke(dynamicClass);

        // Assert 1
        result.Should().BeTrue();

        // Arrange 2
        var array = new[] { field }.AsQueryable();

        // Act 2
        var isValid = array.Select("it").Any("Value eq \"firstValue\"");

        // Assert 2
        isValid.Should().BeTrue();
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_HandleStringArray_For_StringJoin()
    {
        // Arrange
        var strArray = new[] { "a", "b", "c" };
        var parameterExpressions = new List<ParameterExpression>
        {
            Expression.Parameter(strArray.GetType(), "strArray")
        };

        var expression = "string.Join(\",\", strArray)";

        // Act
        var lambdaExpression = DynamicExpressionParser.ParseLambda(parameterExpressions.ToArray(), null, expression);
        var @delegate = lambdaExpression.Compile();
        var result = (string)@delegate.DynamicInvoke(new object[] { strArray });

        // Assert
        result.Should().Be("a,b,c");
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_HandleObjectArray_For_StringJoin()
    {
        // Arrange
        var objectArray = new object[] { 1, 2, 3 };
        var parameterExpressions = new List<ParameterExpression>
        {
            Expression.Parameter(objectArray.GetType(), "objectArray")
        };

        var expression = "string.Join(\",\", objectArray)";

        // Act
        var lambdaExpression = DynamicExpressionParser.ParseLambda(parameterExpressions.ToArray(), null, expression);
        var @delegate = lambdaExpression.Compile();
        var result = (string)@delegate.DynamicInvoke(new object[] { objectArray });

        // Assert
        result.Should().Be("1,2,3");
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_HandleIntArray_For_StringJoin()
    {
        // Arrange
        var intArray = new[] { 1, 2, 3 };
        var parameterExpressions = new List<ParameterExpression>
        {
            Expression.Parameter(intArray.GetType(), "intArray")
        };

        var expression = "string.Join(\",\", intArray)";

        // Act
        var lambdaExpression = DynamicExpressionParser.ParseLambda(parameterExpressions.ToArray(), null, expression);
        var @delegate = lambdaExpression.Compile();
        var result = (string)@delegate.DynamicInvoke(intArray);

        // Assert
        result.Should().Be("1,2,3");
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_LambdaParameter_SameNameAsDynamicType()
    {
        // Act
        DynamicExpressionParser.ParseLambda<bool>(new ParsingConfig(), false, "new[]{1,2,3}.Any(z => z > 0)");
    }

    public class DefaultDynamicLinqCustomTypeProviderForGenericExtensionMethod : DefaultDynamicLinqCustomTypeProvider
    {
        public override HashSet<Type> GetCustomTypes() => new HashSet<Type>(base.GetCustomTypes()) { typeof(Methods), typeof(MethodsItemExtension) };
    }
}