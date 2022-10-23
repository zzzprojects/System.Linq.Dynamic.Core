// ReSharper disable once CheckNamespace
namespace System;

internal static class StringExtensions
{
    /// <summary>
    /// Indicates whether a specified string is null, empty, or consists only of white-space
    /// characters.
    /// 
    /// Recreates the same functionality as System.String.IsNullOrWhiteSpace but included here
    /// for compatibility with net35.
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// true if the value parameter is null or System.String.Empty, or if value consists
    /// exclusively of white-space characters.
    /// </returns>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
#if !NET35
        return string.IsNullOrWhiteSpace(value);
#else
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
#endif
    }
}