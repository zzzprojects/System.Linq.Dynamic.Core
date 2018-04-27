using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core
{
    internal static class ParameterExpressionHelper
    {
        public static ParameterExpression CreateParameterExpression(Type type, string name)
        {
            return Expression.Parameter(type, name);
        }
    }
}
