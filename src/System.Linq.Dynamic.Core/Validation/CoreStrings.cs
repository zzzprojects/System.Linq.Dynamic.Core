// Copied from https://github.com/StefH/Stef.Validation
namespace System.Linq.Dynamic.Core.Validation;

internal static class CoreStrings
{
    public static string ArgumentPropertyNull(string property, string argument)
    {
        return $"The property '{property}' of the argument '{argument}' cannot be null.";
    }

    public static string ArgumentIsEmpty(string? argumentName)
    {
        return $"Value cannot be empty. (Parameter '{argumentName}')";
    }

    public static string CollectionArgumentIsEmpty(string? argumentName)
    {
        return $"The collection argument '{argumentName}' must contain at least one element.";
    }
}