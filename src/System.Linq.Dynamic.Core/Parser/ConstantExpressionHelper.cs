using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser
{
    internal static class ConstantExpressionHelper
    {
        private static readonly ConcurrentDictionary<object, Expression> Expressions = new();
        private static readonly ConcurrentDictionary<Expression, string> Literals = new();

        public static bool TryGetText(Expression expression, out string? text)
        {
            return Literals.TryGetValue(expression, out text);
        }

        public static Expression CreateLiteral(object value, string text)
        {
            if (!Expressions.ContainsKey(value))
            {
                ConstantExpression constantExpression = Expression.Constant(value);

                Expressions.TryAdd(value, constantExpression);
                Literals.TryAdd(constantExpression, text);
            }

            return Expressions[value];
        }
    }
}