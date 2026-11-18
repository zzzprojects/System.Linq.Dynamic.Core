using System.Collections.Concurrent;
using System.Collections.Generic;
#if !(NET35 || NET40)
using System.Collections.ObjectModel;
#endif
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser;

internal static class PredefinedMethodsHelper
{
    internal static readonly MethodInfo ObjectToString = typeof(object).GetMethod(nameof(ToString), BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null)!;
    internal static readonly MethodInfo ObjectInstanceEquals = typeof(object).GetMethod(nameof(Equals), BindingFlags.Instance | BindingFlags.Public, null, [typeof(object)], null)!;
    internal static readonly MethodInfo ObjectStaticEquals = typeof(object).GetMethod(nameof(Equals), BindingFlags.Static | BindingFlags.Public, null, [typeof(object), typeof(object)], null)!;
    internal static readonly MethodInfo ObjectStaticReferenceEquals = typeof(object).GetMethod(nameof(ReferenceEquals), BindingFlags.Static | BindingFlags.Public, null, [typeof(object), typeof(object)], null)!;

    private static readonly HashSet<string> AllowedDictionaryMethodNames =
    [
        "ContainsKey",
        "ContainsValue",
        "TryGetValue",
        "get_Item"
    ];

    private static readonly HashSet<MemberInfo> ObjectToStringAndObjectEquals =
    [
        ObjectToString,
        ObjectInstanceEquals,
        ObjectStaticEquals,
        ObjectStaticReferenceEquals
    ];

    public static bool IsPredefinedDictionaryMethod(MethodInfo method)
    {
        Check.NotNull(method);

        var declaringType = method.DeclaringType;
        if (!AllowedDictionaryMethodNames.Contains(method.Name) || declaringType == null || !declaringType.GetTypeInfo().IsGenericType)
        {
            return false;
        }

        var genericType = declaringType.GetGenericTypeDefinition();
        return genericType == typeof(Dictionary<,>) ||
               genericType == typeof(ConcurrentDictionary<,>) ||
               genericType == typeof(IDictionary<,>)
#if !(NET35 || NET40)
               || genericType == typeof(IReadOnlyDictionary<,>) ||
               genericType == typeof(ReadOnlyDictionary<,>)
#endif
               ;
    }

    public static bool IsPredefinedMethod(ParsingConfig config, MemberInfo member)
    {
        Check.NotNull(config);
        Check.NotNull(member);

        return config.AllowEqualsAndToStringMethodsOnObject && ObjectToStringAndObjectEquals.Contains(member);
    }
}