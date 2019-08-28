using System;
using System.Linq.Expressions;

namespace ETG.SABENTISpro.Utils.DynamicLinkCore.Compatibility
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
