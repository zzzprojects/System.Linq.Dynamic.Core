using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core
{
    internal static class DynamicExpression
    {
        //Commented Out as It's never used.
        //public static Expression Parse(Type resultType, string expression, params object[] values)
        //{
        //    ExpressionParser parser = new ExpressionParser(null, expression, values);
        //    return parser.Parse(resultType);
        //}

        public static LambdaExpression ParseLambda(Type itType, Type resultType, string expression, params object[] values)
        {
            return ParseLambda(new ParameterExpression[] { Expression.Parameter(itType, "") }, resultType, expression, values);
        }

        public static LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expression, params object[] values)
        {
            ExpressionParser parser = new ExpressionParser(parameters, expression, values);
            return Expression.Lambda(parser.Parse(resultType), parameters);
        }

        //Commented Out as It's never used.
        //public static Expression<Func<T, S>> ParseLambda<T, S>(string expression, params object[] values)
        //{
        //    return (Expression<Func<T, S>>)ParseLambda(typeof(T), typeof(S), expression, values);
        //}

        //Commented Out as It's never used.
        //public static Type CreateClass(params DynamicProperty[] properties)
        //{
        //    return ClassFactory.Instance.GetDynamicClass(properties);
        //}

#if !NETFX_CORE
        public static Type CreateClass(IEnumerable<DynamicProperty> properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties);
        }
#endif
    }

}