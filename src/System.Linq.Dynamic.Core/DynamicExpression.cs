using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core
{
    internal static class DynamicExpression
    {
        public static LambdaExpression ParseLambda(bool createParameterCtor, Type itType, Type resultType, string expression, params object[] values)
        {
            return ParseLambda(createParameterCtor, new[] { Expression.Parameter(itType, "") }, resultType, expression, values);
        }

        public static LambdaExpression ParseLambda(bool createParameterCtor, ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
        {
            var parser = new ExpressionParser(parameters, expression, values);

            return Expression.Lambda(parser.Parse(resultType, createParameterCtor), parameters);
        }

        public static Type CreateType(IList<DynamicProperty> properties, bool createParameterCtor)
        {
            return DynamicClassFactory.CreateType(properties, createParameterCtor);
            //return ClassFactory.Instance.CreateType(properties);
        }
    }
}