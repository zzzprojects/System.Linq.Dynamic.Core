// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions
{
    internal static class LambdaExpressionExtensions
    {
        public static Type GetReturnType(this LambdaExpression lambdaExpression)
        {
#if !NET35
            return lambdaExpression.ReturnType;
#else
            return lambdaExpression.Body.Type;
#endif
        }
    }
}
