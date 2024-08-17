#if NETSTANDARD1_3
using System.Linq;

namespace System.Reflection;

internal static class TypeExtensions
{
    public static MethodInfo? GetMethod(this Type type, string name, BindingFlags bindingAttr, object? binder, Type[] types, object[]? modifiers)
    {
        return type
            .GetMethods(bindingAttr)
            .FirstOrDefault(m => m.Name == name && m.GetGenericArguments().SequenceEqual(types));
    }
}
#endif