using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser
{
    internal static class ConstantExpressionHelper
    {
        private static readonly IDictionary<Expression, string> Literals = new ConcurrentDictionary<Expression, string>();

        public static bool TryGetText(Expression expresion, out string text)
        {
            return Literals.TryGetValue(expresion, out text);
        }

        public static Expression CreateLiteral(object value, string text)
        {
            ConstantExpression expresion = Expression.Constant(value);

            Literals.Add(expresion, text);
            return expresion;
        }
    }
}
