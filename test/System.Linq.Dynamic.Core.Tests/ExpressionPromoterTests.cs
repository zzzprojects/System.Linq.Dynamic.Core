using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public class CustomExpressionPromoter : ExpressionPromoter
    {
        public override Expression Promote(Expression expr, Type type, bool exact, bool convertExpr)
        {
            var result = base.Promote(expr, type, exact, convertExpr);

            if (result == null)
            {
                if (type == typeof(string) && expr.Type == typeof(Guid))
                {
                    var ce = expr as ConstantExpression;

                    if (Guid.TryParse(Convert.ToString(ce?.Value), out var guid))
                    {
                        return Expression.Constant(guid, type);
                    }
                }
            }

            return result;
        }
    }

    public class SampleDto
    {
        public Guid data;
    }

    /// <summary>
    /// Test for the expression promoter
    /// </summary>
    public class ExpressionPromoterTests
    {
        [Fact]
        public void PromoteGuidTest()
        {
            var parsingConfig = new ParsingConfig()
            {
                AllowNewToEvaluateAnyType = true
            };

            // Act
            string query = $"new {typeof(SampleDto).FullName}(@0 as data)";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(parsingConfig, null, query, new object[] { Guid.NewGuid().ToString() });
            Delegate del = expression.Compile();
            SampleDto result = (SampleDto) del.DynamicInvoke();

            // Assert
            Assert.NotNull(result);
        }
    }
}
