using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser
{
    internal static class ConstantExpressionHelper
    {
#if DEBUG
        private static readonly TimeSpan TimeToLivePeriod = TimeSpan.FromSeconds(10);
#else
        private static readonly TimeSpan TimeToLivePeriod = TimeSpan.FromMinutes(10);
#endif

        public static readonly ThreadSafeSlidingCache<object, Expression> Expressions = new(TimeToLivePeriod);
        private static readonly ThreadSafeSlidingCache<Expression, string> Literals = new(TimeToLivePeriod);


        public static bool TryGetText(Expression expression, out string? text)
        {
            return Literals.TryGetValue(expression, out text);
        }

        public static Expression CreateLiteral(object value, string text)
        {
            if (Expressions.TryGetValue(value, out var outputValue))
            {
                return outputValue;
            }

            ConstantExpression constantExpression = Expression.Constant(value);

            Expressions.AddOrUpdate(value, constantExpression);
            Literals.AddOrUpdate(constantExpression, text);

            return constantExpression;
        }
    }
}