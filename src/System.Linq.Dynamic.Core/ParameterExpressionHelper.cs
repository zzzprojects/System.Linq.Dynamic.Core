using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core
{
    internal static class ParameterExpressionHelper
    {
        public static ParameterExpression CreateParameterExpression(Type type, string name, bool renameEmpty = false)
        {
            string paramName = name;
            if (renameEmpty && IsNullOrWhiteSpace(paramName))
            {
                paramName = GenerateRandomWord();
            }
            return Expression.Parameter(type, paramName);
        }

        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space
        /// characters.
        /// 
        /// Recreates the same functionality as System.String.IsNullOrWhiteSpace but included here
        /// for compatibility with net35.
        /// </summary>
        /// <param name="value">
        /// The string to test.
        /// </param>
        /// <returns>
        /// true if the value parameter is null or System.String.Empty, or if value consists
        /// exclusively of white-space characters.
        /// </returns>
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
}
