using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core
{
    internal static class DynamicExpression
    {
        public static LambdaExpression ParseLambda(Type itType, Type resultType, string expression, params object[] values)
        {
            return ParseLambda(new[] { Expression.Parameter(itType, "") }, resultType, expression, values);
        }

        public static LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
        {
            ExpressionParser parser = new ExpressionParser(parameters, expression, values);
            return Expression.Lambda(parser.Parse(resultType), parameters);
        }

        public static Type CreateType(IList<DynamicProperty> properties)
        {
            return DynamicClassFactory.CreateType(properties);
            //return ClassFactory.Instance.CreateType(properties);
        }
    }
}