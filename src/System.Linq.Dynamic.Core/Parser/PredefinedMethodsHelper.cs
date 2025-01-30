using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser;

internal class PredefinedMethodsHelper
{
    private static readonly BindingFlags _publicInstance = BindingFlags.Public | BindingFlags.Instance;
    private static readonly BindingFlags _publicStatic = BindingFlags.Public | BindingFlags.Static;

    internal static readonly MethodInfo ObjectInstanceToString = typeof(object).GetMethod(nameof(ToString), _publicInstance, null, Type.EmptyTypes, null)!;
    internal static readonly MethodInfo ObjectInstanceEquals = typeof(object).GetMethod(nameof(Equals), _publicInstance, null, [typeof(object)], null)!;
    internal static readonly MethodInfo ObjectStaticEquals = typeof(object).GetMethod(nameof(Equals), _publicStatic, null, [typeof(object), typeof(object)], null)!;
    internal static readonly MethodInfo ObjectStaticReferenceEquals = typeof(object).GetMethod(nameof(ReferenceEquals), _publicStatic, null, [typeof(object), typeof(object)], null)!;

    private readonly Dictionary<Type, HashSet<MemberInfo>> _supported = new()
    {
        { typeof(bool), new HashSet<MemberInfo>() },
        { typeof(char), new HashSet<MemberInfo>() },
        { typeof(string), new HashSet<MemberInfo>() },
        { typeof(sbyte), new HashSet<MemberInfo>() },
        { typeof(byte), new HashSet<MemberInfo>() },
        { typeof(short), new HashSet<MemberInfo>() },
        { typeof(ushort), new HashSet<MemberInfo>() },
        { typeof(int), new HashSet<MemberInfo>() },
        { typeof(uint), new HashSet<MemberInfo>() },
        { typeof(long), new HashSet<MemberInfo>() },
        { typeof(ulong), new HashSet<MemberInfo>() },
        { typeof(float), new HashSet<MemberInfo>() },
        { typeof(double), new HashSet<MemberInfo>() },
        { typeof(decimal), new HashSet<MemberInfo>() },
        { typeof(DateTime), new HashSet<MemberInfo>() },
        { typeof(DateTimeOffset), new HashSet<MemberInfo>() },
        { typeof(TimeSpan), new HashSet<MemberInfo>() },
        { typeof(Guid), new HashSet<MemberInfo>() },
        { typeof(Uri), new HashSet<MemberInfo>() },
        { typeof(Enum), new HashSet<MemberInfo>() },
#if NET6_0_OR_GREATER
        { typeof(DateOnly), new HashSet<MemberInfo>() },
        { typeof(TimeOnly), new HashSet<MemberInfo>() },
#endif
    };

    public PredefinedMethodsHelper(ParsingConfig config)
    {
        foreach (var kvp in _supported)
        {
            TryAdd(kvp.Key, ObjectInstanceEquals);
            TryAdd(kvp.Key, kvp.Key.GetMethod(nameof(Equals), _publicInstance, null, [kvp.Key], null));
            TryAdd(kvp.Key, kvp.Key.GetMethod(nameof(Equals), _publicInstance, null, [typeof(object)], null));

            TryAdd(kvp.Key, ObjectInstanceToString);
            TryAdd(kvp.Key, kvp.Key.GetMethod(nameof(ToString), _publicInstance, null, Type.EmptyTypes, null));
            TryAdd(kvp.Key, kvp.Key.GetMethod(nameof(ToString), _publicInstance, null, [typeof(string)], null));
            TryAdd(kvp.Key, kvp.Key.GetMethod(nameof(ToString), _publicInstance, null, [typeof(IFormatProvider)], null));
            TryAdd(kvp.Key, kvp.Key.GetMethod(nameof(ToString), _publicInstance, null, [typeof(string), typeof(IFormatProvider)], null));
        }

        if (config.AllowEqualsAndToStringMethodsOnObject)
        {
            _supported[typeof(object)] = [ObjectInstanceToString, ObjectInstanceEquals, ObjectStaticEquals, ObjectStaticReferenceEquals];
        }
    }

    public bool IsPredefinedMethod(Type type, MemberInfo member)
    {
        Check.NotNull(type);
        Check.NotNull(member);

        if (!_supported.TryGetValue(type, out var supported) || supported.Count == 0)
        {
            return false;
        }

        return supported.Contains(member);
    }

    private void TryAdd(Type type, MethodInfo? method)
    {
        if (method != null)
        {
            _supported[type].Add(method);
        }
    }
}