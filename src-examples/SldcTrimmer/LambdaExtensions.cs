using System;
using System.Linq.Dynamic.Core;

namespace SldcTrimmer
{
    internal static class LambdaExtensions
    {
        public static Func<T1, T2> ParseLambda<T1, T2>(this string expression)
        {
            return DynamicExpressionParser.ParseLambda<T1, T2>(null, false, expression).Compile();
        }
    }
}
