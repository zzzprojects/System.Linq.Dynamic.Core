using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core
{
    internal static class ParameterExpressionHelper
    {
        internal const int RandomWordLength = 6;

        public static ParameterExpression CreateParameterExpression(Type type, string name, bool renameEmpty = false)
        {
            string paramName = name;
            if (renameEmpty && IsNullOrWhiteSpace(paramName))
            {
                paramName = GenerateRandomWord();
            }
            return Expression.Parameter(type, paramName);
        }

        internal static bool IsNullOrWhiteSpace(string value)
        {
            if (value == null)
            {
                return true;
            }
            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                {
                    return false;
                }
            }
            return true;
        }

        internal static string GenerateRandomWord()
        {
            const int diff = 'A' - '0';
            return string.Concat(Guid.NewGuid().ToString(@"N").Select(c => (char)(c + diff)).Take(RandomWordLength)).ToLower();
        }
    }
}
