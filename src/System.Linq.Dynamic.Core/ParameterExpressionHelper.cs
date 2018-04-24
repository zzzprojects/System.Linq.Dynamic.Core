using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core
{
    internal static class ParameterExpressionHelper
    {
        private static readonly IDictionary<string, ParameterExpression> ParameterExpressions = new ConcurrentDictionary<string, ParameterExpression>();

        public static ParameterExpression CreateParameterExpression(Type type, string name)
        {
            string key = type.FullName + "_" + name;

            if (!ParameterExpressions.ContainsKey(key))
            {
                ParameterExpressions.Add(key, Expression.Parameter(type, name));
            }

            return ParameterExpressions[key];
        }
    }
}
