using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core.Tests.Entities;
using System.Linq.Expressions;
using FluentAssertions;
using Moq;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser;

public partial class ExpressionParserTests
{
    private readonly Mock<IDynamicLinqCustomTypeProvider> _dynamicTypeProviderMock;

    private readonly ParsingConfig _parsingConfig;

    [Flags]
    public enum ExampleFlags
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4,
        D = 8,
    };

    public class MyView
    {
        public Dictionary<string, string>? Properties { get; set; }
    }

    public ExpressionParserTests()
    {
        _dynamicTypeProviderMock = new Mock<IDynamicLinqCustomTypeProvider>();
        _dynamicTypeProviderMock.Setup(dt => dt.GetCustomTypes()).Returns([typeof(Company), typeof(MainCompany)]);
        _dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(Company).FullName!)).Returns(typeof(Company));
        _dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(MainCompany).FullName!)).Returns(typeof(MainCompany));
        _dynamicTypeProviderMock.Setup(dt => dt.ResolveTypeBySimpleName("Company")).Returns(typeof(Company));
        _dynamicTypeProviderMock.Setup(dt => dt.ResolveTypeBySimpleName("MainCompany")).Returns(typeof(MainCompany));

        _parsingConfig = new ParsingConfig
        {
            CustomTypeProvider = _dynamicTypeProviderMock.Object
        };
    }

    [Fact]
    public void Parse_BitwiseOperatorOr_On_2EnumFlags()
    {
        // Arrange
        var expression = "@0 | @1";
#if NET48
        var expected = "Convert((Convert(A) | Convert(B)))";
#else
        var expected = "Convert((Convert(A, Int32) | Convert(B, Int32)), ExampleFlags)";
#endif
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(int), "x")];
        var sut = new ExpressionParser(parameters, expression, [ExampleFlags.A, ExampleFlags.B], null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        parsedExpression.Should().Be(expected);

        // Arrange
        var query = new[] { 0 }.AsQueryable();

        // Act
        var result = query.Select(expression, ExampleFlags.A, ExampleFlags.B).First();

        // Assert
        Assert.IsType<ExampleFlags>(result);
        Assert.Equal(ExampleFlags.A | ExampleFlags.B, result);
    }

    [Fact]
    public void Parse_BitwiseOperatorAnd_On_2EnumFlags()
    {
        // Arrange
        var expression = "@0 & @1";
#if NET48
        var expected = "Convert((Convert(A) & Convert(B)))";
#else
        var expected = "Convert((Convert(A, Int32) & Convert(B, Int32)), ExampleFlags)";
#endif
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(int), "x")];
        var sut = new ExpressionParser(parameters, expression, [ExampleFlags.A, ExampleFlags.B], null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        parsedExpression.Should().Be(expected);

        // Arrange
        var query = new[] { 0 }.AsQueryable();

        // Act
        var result = query.Select(expression, ExampleFlags.A, ExampleFlags.B).First();

        // Assert
        Assert.IsType<ExampleFlags>(result);
        Assert.Equal(ExampleFlags.A & ExampleFlags.B, result);
    }

    [Fact]
    public void Parse_BitwiseOperatorOr_On_3EnumFlags()
    {
        // Arrange
        var expression = "@0 | @1 | @2";
#if NET48
        var expected = "Convert(((Convert(A) | Convert(B)) | Convert(C)))";
#else
        var expected = "Convert(((Convert(A, Int32) | Convert(B, Int32)) | Convert(C, Int32)), ExampleFlags)";
#endif
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(int), "x")];
        var sut = new ExpressionParser(parameters, expression, [ExampleFlags.A, ExampleFlags.B, ExampleFlags.C], null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        parsedExpression.Should().Be(expected);

        // Arrange
        var query = new[] { 0 }.AsQueryable();

        // Act
        var result = query.Select(expression, ExampleFlags.A, ExampleFlags.B, ExampleFlags.C).First();

        // Assert
        Assert.IsType<ExampleFlags>(result);
        Assert.Equal(ExampleFlags.A | ExampleFlags.B | ExampleFlags.C, result);
    }

    [Fact]
    public void Parse_BitwiseOperatorAnd_On_3EnumFlags()
    {
        // Arrange
        var expression = "@0 & @1 & @2";
#if NET48
        var expected = "Convert(((Convert(A) & Convert(B)) & Convert(C)))";
#else
        var expected = "Convert(((Convert(A, Int32) & Convert(B, Int32)) & Convert(C, Int32)), ExampleFlags)";
#endif
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(int), "x")];
        var sut = new ExpressionParser(parameters, expression, [ExampleFlags.A, ExampleFlags.B, ExampleFlags.C], null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        parsedExpression.Should().Be(expected);

        // Arrange
        var query = new[] { 0 }.AsQueryable();

        // Act
        var result = query.Select(expression, ExampleFlags.A, ExampleFlags.B, ExampleFlags.C).First();

        // Assert
        Assert.IsType<ExampleFlags>(result);
        Assert.Equal(ExampleFlags.A & ExampleFlags.B & ExampleFlags.C, result);
    }

    [Fact]
    public void Parse_ParseBinaryInteger()
    {
        // Arrange
        var expression = "0b1100000011101";
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(int), "x")];
        var sut = new ExpressionParser(parameters, expression, null, null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        parsedExpression.Should().Be("6173");
    }

    [Fact]
    public void Parse_ParseHexadecimalInteger()
    {
        // Arrange
        var expression = "0xFF";
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(int), "x")];
        var sut = new ExpressionParser(parameters, expression, null, null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        parsedExpression.Should().Be("255");
    }

    [Theory]
    [InlineData("it == 1", "(x == 1)")]
    [InlineData("it eq 1", "(x == 1)")]
    [InlineData("it equal 1", "(x == 1)")]
    [InlineData("it != 1", "(x != 1)")]
    [InlineData("it ne 1", "(x != 1)")]
    [InlineData("it neq 1", "(x != 1)")]
    [InlineData("it notequal 1", "(x != 1)")]
    [InlineData("it lt 1", "(x < 1)")]
    [InlineData("it LessThan 1", "(x < 1)")]
    [InlineData("it le 1", "(x <= 1)")]
    [InlineData("it LessThanEqual 1", "(x <= 1)")]
    [InlineData("it gt 1", "(x > 1)")]
    [InlineData("it GreaterThan 1", "(x > 1)")]
    [InlineData("it ge 1", "(x >= 1)")]
    [InlineData("it GreaterThanEqual 1", "(x >= 1)")]
    public void Parse_ParseComparisonOperator(string expression, string result)
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(int), "x")];
        var sut = new ExpressionParser(parameters, expression, null, null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        Check.That(parsedExpression).Equals(result);
    }

    [Theory]
    [InlineData("it || true", "(x OrElse True)")]
    [InlineData("it or true", "(x OrElse True)")]
    [InlineData("it OrElse true", "(x OrElse True)")]
    public void Parse_ParseOrOperator(string expression, string result)
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(bool), "x")];
        var sut = new ExpressionParser(parameters, expression, null, null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        Check.That(parsedExpression).Equals(result);
    }

    [Theory]
    [InlineData("it && true", "(x AndAlso True)")]
    [InlineData("it and true", "(x AndAlso True)")]
    [InlineData("it AndAlso true", "(x AndAlso True)")]
    public void Parse_ParseAndOperator(string expression, string result)
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(bool), "x")];
        var sut = new ExpressionParser(parameters, expression, null, null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        Check.That(parsedExpression).Equals(result);
    }

    [Fact]
    public void Parse_ParseMultipleInOperators()
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(Company), "x")];
        var sut = new ExpressionParser(parameters, "MainCompanyId in (1, 2) and Name in (\"A\", \"B\") && 'y' in Name && 'z' in Name", null, null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        Check.That(parsedExpression).Equals("(((((x.MainCompanyId == 1) OrElse (x.MainCompanyId == 2)) AndAlso ((x.Name == \"A\") OrElse (x.Name == \"B\"))) AndAlso x.Name.Contains(y)) AndAlso x.Name.Contains(z))");
    }

    [Fact]
    public void Parse_ParseMultipleInAndNotInOperators()
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(Company), "x")];
        var sut = new ExpressionParser(parameters, "MainCompanyId in (1, 2) and Name not in (\"A\", \"B\") && 'y' in Name && 'z' not in Name", null, null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        Check.That(parsedExpression).Equals("(((((x.MainCompanyId == 1) OrElse (x.MainCompanyId == 2)) AndAlso Not(((x.Name == \"A\") OrElse (x.Name == \"B\")))) AndAlso x.Name.Contains(y)) AndAlso Not(x.Name.Contains(z)))");
    }


    [Fact]
    public void Parse_ParseMultipleInAndNotInAndNot_InOperators()
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(Company), "x")];
        var sut = new ExpressionParser(parameters, "MainCompanyId in (1, 2) and MainCompanyId not in (3, 4) and Name not_in (\"A\", \"B\") && 'y' in Name && 'z' not in Name && 's' not_in Name", null, null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        Check.That(parsedExpression).Equals("(((((((x.MainCompanyId == 1) OrElse (x.MainCompanyId == 2)) AndAlso Not(((x.MainCompanyId == 3) OrElse (x.MainCompanyId == 4)))) AndAlso Not(((x.Name == \"A\") OrElse (x.Name == \"B\")))) AndAlso x.Name.Contains(y)) AndAlso Not(x.Name.Contains(z))) AndAlso Not(x.Name.Contains(s)))");
    }

    [Fact]
    public void Parse_ParseInWrappedInParenthesis()
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(Company), "x")];
        var sut = new ExpressionParser(parameters, "(MainCompanyId in @0)", [new long?[] { 1, 2 }], null);

        // Act
        var parsedExpression = sut.Parse(null).ToString();

        // Assert
        Check.That(parsedExpression).Equals("value(System.Nullable`1[System.Int64][]).Contains(x.MainCompanyId)");
    }

    [Fact]
    public void Parse_CastActingOnIt()
    {
        // Arrange
        var parameters = new[] { ParameterExpressionHelper.CreateParameterExpression(typeof(User), "u") };
        var sut = new ExpressionParser(parameters, "DisplayName.Any(int(it) > 109)", null, null);

        // Act
        var result = sut.Parse(null);

        // Assert
        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("string(\"\")", "")]
    [InlineData("string(\"a\")", "a")]
    [InlineData("int(42)", 42)]
    public void Parse_CastStringIntShouldReturnConstantExpression(string expression, object result)
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(bool), "x")];
        var sut = new ExpressionParser(parameters, expression, null, null);

        // Act
        var constantExpression = (ConstantExpression)sut.Parse(null);

        // Assert
        Check.That(constantExpression.Value).Equals(result);
    }

    [Theory]
#if NET48
    [InlineData("int?(5)", typeof(int?), "Convert(5)")]
    [InlineData("int?(null)", typeof(int?), "Convert(null)")]
    [InlineData("string(null)", typeof(string), "Convert(null)")]
#else
    [InlineData("int?(5)", typeof(int?), "Convert(5, Nullable`1)")]
    [InlineData("int?(null)", typeof(int?), "Convert(null, Nullable`1)")]
    [InlineData("string(null)", typeof(string), "Convert(null, String)")]
#endif
    public void Parse_NullableShouldReturnNullable(string expression, object resultType, object result)
    {
        // Arrange
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(bool), "x")];
        var sut = new ExpressionParser(parameters, expression, null, null);

        // Act
        var unaryExpression = (UnaryExpression)sut.Parse(null);

        // Assert
        Check.That(unaryExpression.Type).Equals(resultType);
        Check.That(unaryExpression.ToString()).Equals(result);
    }

    [Theory]
    [InlineData("it.MainCompany.Name != null", "(company.MainCompany.Name != null)")]
    [InlineData("@MainCompany.Companies.Count() > 0", "(company.MainCompany.Companies.Count() > 0)")]
    [InlineData("Company.Equals(null, null)", "Equals(null, null)")]
    [InlineData("Equals(null)", "company.Equals(null)")]
    [InlineData("MainCompany.Name", "company.MainCompany.Name")]
    [InlineData("Name", "company.Name")]
    [InlineData("company.Name", "company.Name")]
    [InlineData("DateTime", "company.DateTime")]
    public void Parse_When_PrioritizePropertyOrFieldOverTheType_IsTrue(string expression, string result)
    {
        // Arrange
        var config = new ParsingConfig
        {
            IsCaseSensitive = true,
            CustomTypeProvider = _dynamicTypeProviderMock.Object,
            AllowEqualsAndToStringMethodsOnObject = true
        };
        ParameterExpression[] parameters = [ ParameterExpressionHelper.CreateParameterExpression(typeof(Company), "company") ];
        var sut = new ExpressionParser(parameters, expression, null, config);

        // Act
        string parsedExpression;
        try
        {
            parsedExpression = sut.Parse(null).ToString();
        }
        catch (ParseException e)
        {
            parsedExpression = e.Message;
        }

        // Assert
        parsedExpression.Should().Be(result);
    }

    [Theory]
    [InlineData("it.MainCompany.Name != null", "(company.MainCompany.Name != null)")]
    [InlineData("@MainCompany.Companies.Count() > 0", "(company.MainCompany.Companies.Count() > 0)")]
    [InlineData("Company.Equals(null, null)", "No applicable method 'Equals' exists in type 'Company'")] // Exception
    [InlineData("MainCompany.Name", "company.MainCompany.Name")]
    [InlineData("Name", "company.Name")]
    [InlineData("it.DateTime", "company.DateTime")]
    [InlineData("DateTime", "'.' or '(' or string literal expected")] // Exception
    public void Parse_When_PrioritizePropertyOrFieldOverTheType_IsFalse(string expression, string result)
    {
        // Arrange
        var config = new ParsingConfig
        {
            PrioritizePropertyOrFieldOverTheType = false
        };
        ParameterExpression[] parameters = [ParameterExpressionHelper.CreateParameterExpression(typeof(Company), "company")
        ];

        // Act
        string parsedExpression;
        try
        {
            var sut = new ExpressionParser(parameters, expression, null, config);
            parsedExpression = sut.Parse(null).ToString();
        }
        catch (Exception e)
        {
            parsedExpression = e.Message;
        }

        // Assert
        parsedExpression.Should().StartWith(result);
    }

    [Theory]
    [InlineData("99 & \"txt\"", "Concat(Convert(99, Object), Convert(\"txt\", Object))")]
    [InlineData("\"txt\" & 99", "Concat(Convert(\"txt\", Object), Convert(99, Object))")]
    [InlineData("\"txt\" & \"abc\"", "Concat(\"txt\", \"abc\")")]
    [InlineData("99 + \"txt\"", "Concat(Convert(99, Object), Convert(\"txt\", Object))")]
    [InlineData("\"txt\" + 99", "Concat(Convert(\"txt\", Object), Convert(99, Object))")]
    [InlineData("\"txt\" + \"abc\"", "Concat(\"txt\", \"abc\")")]
    public void Parse_StringConcat(string expression, string result)
    {
        // Arrange
        var parameters = new[] { Expression.Parameter(typeof(int), "VarA") };
        var parser = new ExpressionParser(parameters, expression, [], new ParsingConfig { ConvertObjectToSupportComparison = true });

        // Act
        var parsedExpression = parser.Parse(typeof(string)).ToString();

        // Assert
        parsedExpression.Should().Be(result);
    }

    [Fact]
    public void Parse_InvalidExpressionShouldThrowArgumentException()
    {
        // Arrange & Act
        Action act = () => DynamicExpressionParser.ParseLambda<MyView, bool>(ParsingConfig.Default, false, "Properties[\"foo\"] > 2", []);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Method 'Compare' not found on type 'System.String' or 'System.Int32'");
    }
    
    [Theory]
    [InlineData("List.MAX(x => x)", false, "List.Max(Param_0 => Param_0)")]
    [InlineData("List.min(x => x)", false, "List.Min(Param_0 => Param_0)")]
    [InlineData("List.Min(x => x)", false, "List.Min(Param_0 => Param_0)")]
    [InlineData("List.Min(x => x)", true, "List.Min(Param_0 => Param_0)")]
    [InlineData("List.WHERE(x => x.Ticks >= 100000).max()", false, "List.Where(Param_0 => (Param_0.Ticks >= 100000)).Max()")]
    public void Parse_LinqMethodsRespectCasing(string expression, bool caseSensitive, string result)
    {
        // Arrange
        var parameters = new[] { Expression.Parameter(typeof(DateTime[]), "List") };
        
        var parser = new ExpressionParser(
            parameters,
            expression, 
            [],
            new ParsingConfig
            {
                IsCaseSensitive = caseSensitive
            });

        // Act
        var parsedExpression = parser.Parse(typeof(DateTime)).ToString();

        // Assert
        parsedExpression.Should().Be(result);
    }

    [Fact]
    public void Parse_InvalidCasingShouldThrowInvalidOperationException()
    {
        // Arrange & Act
        var parameters = new[] { Expression.Parameter(typeof(DateTime[]), "List") };
        
        Action act = () => new ExpressionParser(
            parameters,
            "List.MAX(x => x)", 
            [],
            new ParsingConfig
            {
                IsCaseSensitive = true
            })
            .Parse(typeof(DateTime));
        
        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("No generic method 'MAX' on type 'System.Linq.Enumerable' is compatible with the supplied type arguments and arguments. No type arguments should be provided if the method is non-generic.*");
    }
}