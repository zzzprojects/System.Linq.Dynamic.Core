using System;
using System.Linq.Dynamic.Core;

namespace TestSLDC
{
    internal static class LambdaExtensions
    {
        public static Func<T1, T2> ParseLambda<T1, T2>(this string expression)
        {
            return (Func<T1, T2>)DynamicExpressionParser.ParseLambda(false, typeof(T1), typeof(T2), expression).Compile();
        }
    }
}
