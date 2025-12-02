using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser;

internal class PredefinedMethodsHelper
{
    private const BindingFlags PublicInstance = BindingFlags.Public | BindingFlags.Instance;
    private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;

    internal static readonly MethodInfo ObjectInstanceToString = typeof(object).GetMethod(nameof(ToString), PublicInstance, null, Type.EmptyTypes, null)!;
    internal static readonly MethodInfo ObjectInstanceEquals = typeof(object).GetMethod(nameof(Equals), PublicInstance, null, [typeof(object)], null)!;
    internal static readonly MethodInfo ObjectStaticEquals = typeof(object).GetMethod(nameof(Equals), PublicStatic, null, [typeof(object), typeof(object)], null)!;
    internal static readonly MethodInfo ObjectStaticReferenceEquals = typeof(object).GetMethod(nameof(ReferenceEquals), PublicStatic, null, [typeof(object), typeof(object)], null)!;

    private readonly Dictionary<Type, HashSet<MemberInfo>> _supported = new()
    {
        { typeof(bool), [] },
        { typeof(byte), [] },
        { typeof(char), [] },
        { typeof(DateTime), [] },
        { typeof(DateTimeOffset), [] },
        { typeof(decimal), [] },
        { typeof(double), [] },
        { typeof(float), [] },
        { typeof(Guid), [] },
        { typeof(int), [] },
        { typeof(long), [] },
        { typeof(sbyte), [] },
        { typeof(short), [] },
        { typeof(string), [] },
        { typeof(TimeSpan), [] },
        { typeof(uint), [] },
        { typeof(ulong), [] },
        { typeof(Uri), [] },
        { typeof(ushort), [] },
#if NET6_0_OR_GREATER
        { typeof(DateOnly), [] },
        { typeof(TimeOnly), [] }
#endif
    };

    internal PredefinedMethodsHelper(ParsingConfig config)
    {
        foreach (var kvp in _supported)
        {
            TryAdd(kvp.Key, kvp.Key.GetMethod(nameof(Equals), PublicInstance, null, [kvp.Key], null));
            TryAdd(kvp.Key, kvp.Key.GetMethod(nameof(Equals), PublicInstance, null, [typeof(object)], null));

            TryAdd(kvp.Key, kvp.Key.GetMethod(nameof(ToString), PublicInstance, null, Type.EmptyTypes, null));
            TryAdd(kvp.Key, kvp.Key.GetMethod(nameof(ToString), PublicInstance, null, [typeof(string)], null));
            TryAdd(kvp.Key, kvp.Key.GetMethod(nameof(ToString), PublicInstance, null, [typeof(IFormatProvider)], null));
            TryAdd(kvp.Key, kvp.Key.GetMethod(nameof(ToString), PublicInstance, null, [typeof(string), typeof(IFormatProvider)], null));
        }

        if (config.AllowEqualsAndToStringMethodsOnObject)
        {
            _supported[typeof(object)] = [ObjectInstanceToString, ObjectInstanceEquals, ObjectStaticEquals, ObjectStaticReferenceEquals];
        }
    }

    internal bool IsPredefinedMethod(Type type, Type declaringType, MemberInfo member)
    {
        if (_supported.TryGetValue(type, out var supportedMethodsForType) && supportedMethodsForType.Count > 0)
        {
            return supportedMethodsForType.Contains(member);
        }

        if (_supported.TryGetValue(declaringType, out var supportedMethodsForDeclaringType) && supportedMethodsForDeclaringType.Count > 0)
        {
            return supportedMethodsForDeclaringType.Contains(member);
        }

        // Last resort, check if the method name is supported for object
        if (_supported.TryGetValue(typeof(object), out var supportedMethodsForObject) && supportedMethodsForObject.Count > 0)
        {
            return supportedMethodsForObject.Any(x => x.Name == member.Name);
        }

        return false;
    }

    private void TryAdd(Type type, MethodInfo? method)
    {
        if (method != null)
        {
            _supported[type].Add(method);
        }
    }
}