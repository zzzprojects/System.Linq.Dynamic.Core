// ReSharper disable once CheckNamespace
namespace System;

internal static class StringExtensions
{
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