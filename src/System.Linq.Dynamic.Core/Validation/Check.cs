using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// Copied from https://github.com/StefH/Stef.Validation
namespace System.Linq.Dynamic.Core.Validation;

[DebuggerStepThrough]
internal static class Check
{
    public static T Condition<T>(T value, Predicate<T> predicate, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        NotNull(predicate, nameof(predicate));

        if (!predicate(value))
        {
            NotNullOrEmpty(parameterName, nameof(parameterName));

            throw new ArgumentOutOfRangeException(parameterName);
        }

        return value;
    }

    public static T NotNull<T>(T value, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value is null)
        {
            NotNullOrEmpty(parameterName, nameof(parameterName));

            throw new ArgumentNullException(parameterName);
        }

        return value;
    }

    public static T NotNull<T>(T value, string parameterName, string propertyName)
    {
        if (value is null)
        {
            NotNullOrEmpty(parameterName, nameof(parameterName));
            NotNullOrEmpty(propertyName, nameof(propertyName));

            throw new ArgumentException(CoreStrings.ArgumentPropertyNull(propertyName, parameterName));
        }

        return value;
    }

    public static IEnumerable<T> NotNullOrEmpty<T>(IEnumerable<T> value, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        IEnumerable<T> result = NotNull(value, parameterName);

        // ReSharper disable once PossibleMultipleEnumeration
        if (!result.Any())
        {
            NotNullOrEmpty(parameterName, nameof(parameterName));

            throw new ArgumentException(CoreStrings.CollectionArgumentIsEmpty(parameterName));
        }

        // ReSharper disable once PossibleMultipleEnumeration
        return result;
    }

    public static string NotEmpty(string? value, [CallerArgumentExpression("value")] string? parameterName = null) =>
        NotNullOrWhiteSpace(value, parameterName);

    public static string NotNullOrEmpty(string? value, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value is null)
        {
            NotNullOrEmpty(parameterName, nameof(parameterName));

            throw new ArgumentNullException(parameterName);
        }

        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException(CoreStrings.ArgumentIsEmpty(parameterName));
        }

        return value;
    }

    public static string NotNullOrWhiteSpace(string? value, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value is null)
        {
            NotNullOrEmpty(parameterName, nameof(parameterName));

            throw new ArgumentNullException(parameterName);
        }

        if (value.IsNullOrWhiteSpace())
        {
            throw new ArgumentException(CoreStrings.ArgumentIsEmpty(parameterName));
        }

        return value;
    }

    public static IEnumerable<T> HasNoNulls<T>(IEnumerable<T> value, [CallerArgumentExpression("value")] string? parameterName = null)
    {
        if (value is null)
        {
            NotNullOrEmpty(parameterName, nameof(parameterName));

            throw new ArgumentNullException(parameterName);
        }

        // ReSharper disable once PossibleMultipleEnumeration
        if (value.Any(v => v is null))
        {
            NotNullOrEmpty(parameterName, nameof(parameterName));

            throw new ArgumentException(parameterName);
        }

        // ReSharper disable once PossibleMultipleEnumeration
        return value;
    }
}