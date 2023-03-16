namespace System.Linq.Dynamic.Core.Tests.TestHelpers
{
    public static class ExpressionString
    {
        public static string NullableConversion(string convertedExpr)
        {
#if NET452 || NET461
            return $"Convert({convertedExpr})";
#else
            return $"Convert({convertedExpr}, Nullable`1)";
#endif
        }
    }
}