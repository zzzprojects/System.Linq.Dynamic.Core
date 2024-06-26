using System.Linq.Dynamic.Core.Tokenizer;

namespace System.Linq.Dynamic.Core.Extensions;

internal static class TokenIdExtensions
{
    internal static bool IsEqualityOperator(this TokenId tokenId)
    {
        return tokenId is TokenId.Equal or TokenId.DoubleEqual or TokenId.ExclamationEqual or TokenId.LessGreater;
    }

    internal static bool IsComparisonOperator(this TokenId tokenId)
    {
        return tokenId is TokenId.Equal or TokenId.DoubleEqual or TokenId.ExclamationEqual or TokenId.LessGreater or TokenId.GreaterThan or TokenId.GreaterThanEqual or TokenId.LessThan or TokenId.LessThanEqual;
    }
}