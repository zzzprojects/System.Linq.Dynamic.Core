using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core;

internal static class ParameterExpressionHelper
{
    public static ParameterExpression CreateParameterExpression(Type type, string name, bool renameEmpty = false)
    {
        string paramName = name;
        if (renameEmpty && paramName.IsNullOrWhiteSpace())
        {
            paramName = GenerateRandomWord();
        }

        return Expression.Parameter(type, paramName);
    }

    /// <summary>
    /// Generates a random 16 character word derived from a Guid value.
    /// </summary>
    internal static string GenerateRandomWord()
    {
        const int wordLength = 16;
        const int diff = 'A' - '0';
        return string.Concat(Guid.NewGuid().ToString(@"N").Select(c => (char)(c + diff)).Take(wordLength)).ToLower();
    }
}