using Moq;
using NFluent;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core.Tests.Entities;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    public partial class ExpressionParserTests
    {
        private readonly ParsingConfig _parsingConfig;
        private readonly Mock<IDynamicLinkCustomTypeProvider> _dynamicTypeProviderMock;

        public ExpressionParserTests()
        {
            _dynamicTypeProviderMock = new Mock<IDynamicLinkCustomTypeProvider>();
            _dynamicTypeProviderMock.Setup(dt => dt.GetCustomTypes()).Returns(new HashSet<Type>() { typeof(Company), typeof(MainCompany) });
            _dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(Company).FullName)).Returns(typeof(Company));
            _dynamicTypeProviderMock.Setup(dt => dt.ResolveType(typeof(MainCompany).FullName)).Returns(typeof(MainCompany));
            _dynamicTypeProviderMock.Setup(dt => dt.ResolveTypeBySimpleName("Company")).Returns(typeof(Company));
            _dynamicTypeProviderMock.Setup(dt => dt.ResolveTypeBySimpleName("MainCompany")).Returns(typeof(MainCompany));

            _parsingConfig = new ParsingConfig
            {
                CustomTypeProvider = _dynamicTypeProviderMock.Object
            };
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
            ParameterExpression[] parameters = { ParameterExpressionHelper.CreateParameterExpression(typeof(int), "x") };
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
            ParameterExpression[] parameters = { ParameterExpressionHelper.CreateParameterExpression(typeof(bool), "x") };
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
            ParameterExpression[] parameters = { ParameterExpressionHelper.CreateParameterExpression(typeof(bool), "x") };
            var sut = new ExpressionParser(parameters, expression, null, null);

            // Act
            var parsedExpression = sut.Parse(null).ToString();

            // Assert
            Check.That(parsedExpression).Equals(result);
        }

        [Theory]
        [InlineData("string(\"\")", "")]
        [InlineData("string(\"a\")", "a")]
        [InlineData("int(42)", 42)]
        public void Parse_CastStringIntShouldReturnConstantExpression(string expression, object result)
        {
            // Arrange
            ParameterExpression[] parameters = { ParameterExpressionHelper.CreateParameterExpression(typeof(bool), "x") };
            var sut = new ExpressionParser(parameters, expression, null, null);

            // Act
            var constantExpression = (ConstantExpression)sut.Parse(null);

            // Assert
            Check.That(constantExpression.Value).Equals(result);
        }

        [Theory]
#if NET452
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
            ParameterExpression[] parameters = { ParameterExpressionHelper.CreateParameterExpression(typeof(bool), "x") };
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
        [InlineData("MainCompany.Name", "company.MainCompany.Name")]
        [InlineData("Company.Name", "No property or field 'Name' exists in type 'Company'")]
        public void Parse_PrioritizePropertyOrFieldOverTheType(string expression, string result)
        {
            // Arrange
            ParameterExpression[] parameters = { ParameterExpressionHelper.CreateParameterExpression(typeof(Company), "company") };
            var sut = new ExpressionParser(parameters, expression, null, _parsingConfig);

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
            Check.That(parsedExpression).Equals(result);
        }
    }
}
