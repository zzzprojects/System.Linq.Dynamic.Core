using NFluent;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    public class ExpressionHelperTests
    {
        private readonly IExpressionHelper _expressionHelper;

        public ExpressionHelperTests()
        {
            _expressionHelper = new ExpressionHelper(ParsingConfig.Default);
        }

        [Fact]
        public void ExpressionHelper_WrapConstantExpression_false()
        {
            // Assign
            var config = new ParsingConfig
            {
                UseParameterizedNamesInDynamicQuery = false
            };
            var expressionHelper = new ExpressionHelper(config);

            string value = "test";
            Expression expression = Expression.Constant(value);

            // Act
            expressionHelper.WrapConstantExpression(ref expression);

            // Assert
            Check.That(expression).IsInstanceOf<ConstantExpression>();
            Check.That(expression.ToString()).Equals("\"test\"");
        }

        [Fact]
        public void ExpressionHelper_WrapConstantExpression_true()
        {
            // Assign
            var config = new ParsingConfig
            {
                UseParameterizedNamesInDynamicQuery = true
            };
            var expressionHelper = new ExpressionHelper(config);

            string value = "test";
            Expression expression = Expression.Constant(value);

            // Act
            expressionHelper.WrapConstantExpression(ref expression);
            expressionHelper.WrapConstantExpression(ref expression);

            // Assert
            Check.That(expression.GetType().FullName).Equals("System.Linq.Expressions.PropertyExpression");
            Check.That(expression.ToString()).Equals("value(System.Linq.Dynamic.Core.Parser.WrappedValue`1[System.String]).Value");
        }

        [Fact]
        public void ExpressionHelper_OptimizeStringForEqualityIfPossible_Guid()
        {
            // Assign
            string guidAsString = Guid.NewGuid().ToString();

            // Act
            Expression result = _expressionHelper.OptimizeStringForEqualityIfPossible(guidAsString, typeof(Guid));

            // Assert
            Check.That(result).IsInstanceOf<ConstantExpression>();
            Check.That(result.ToString()).Equals(guidAsString);
        }

        [Fact]
        public void ExpressionHelper_OptimizeStringForEqualityIfPossible_Guid_Invalid()
        {
            // Assign
            string guidAsString = "x";

            // Act
            Expression result = _expressionHelper.OptimizeStringForEqualityIfPossible(guidAsString, typeof(Guid));

            // Assert
            Check.That(result).IsNull();
        }

        [Fact]
        public void ExpressionHelper_GenerateAndAlsoNotNullExpression()
        {
            // Assign
            Expression<Func<Item, int>> expression = (x) => x.Relation1.Relation2.Id;

            // Act
            Expression result = _expressionHelper.GenerateAndAlsoNotNullExpression(expression);

            // Assert
            Check.That(result.ToString()).IsEqualTo("(((x != null) AndAlso (x.Relation1 != null)) AndAlso (x.Relation1.Relation2 != null))");
        }

        class Item
        {
            public int Id { get; set; }
            public Relation1 Relation1 { get; set; }
        }

        class Relation1
        {
            public int Id { get; set; }
            public Relation2 Relation2 { get; set; }
        }

        class Relation2
        {
            public int Id { get; set; }
        }
    }
}
