using System.Collections.Generic;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser;

internal class PredefinedMethodsHelper
{
    private static readonly BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.Instance;

    internal static readonly MethodInfo ObjectInstanceToString = typeof(object).GetMethod(nameof(ToString), _bindingFlags, null, Type.EmptyTypes, null)!;
    internal static readonly MethodInfo ObjectInstanceEquals = typeof(object).GetMethod(nameof(Equals), _bindingFlags, null, [typeof(object)], null)!;
    internal static readonly MethodInfo ObjectStaticEquals = typeof(object).GetMethod(nameof(Equals), BindingFlags.Static | BindingFlags.Public, null, [typeof(object), typeof(object)], null)!;
    internal static readonly MethodInfo ObjectStaticReferenceEquals = typeof(object).GetMethod(nameof(ReferenceEquals), BindingFlags.Static | BindingFlags.Public, null, [typeof(object), typeof(object)], null)!;

    private readonly Dictionary<Type, HashSet<MethodInfo>> _supported = new()
    {
        { typeof(bool), new HashSet<MethodInfo>() },
        { typeof(char), new HashSet<MethodInfo>() },
        { typeof(string), new HashSet<MethodInfo>() },
        { typeof(sbyte), new HashSet<MethodInfo>() },
        { typeof(byte), new HashSet<MethodInfo>() },
        { typeof(short), new HashSet<MethodInfo>() },
        { typeof(ushort), new HashSet<MethodInfo>() },
        { typeof(int), new HashSet<MethodInfo>() },
        { typeof(uint), new HashSet<MethodInfo>() },
        { typeof(long), new HashSet<MethodInfo>() },
        { typeof(ulong), new HashSet<MethodInfo>() },
        { typeof(float), new HashSet<MethodInfo>() },
        { typeof(double), new HashSet<MethodInfo>() },
        { typeof(decimal), new HashSet<MethodInfo>() },
        // { typeof(DateTime), new HashSet<MethodInfo>() },
        // { typeof(DateTimeOffset), new HashSet<MethodInfo>() },
        // { typeof(TimeSpan), new HashSet<MethodInfo>() },
        // { typeof(Guid), new HashSet<MethodInfo>() },
        // { typeof(Uri), new HashSet<MethodInfo>() },
        // { typeof(Enum), new HashSet<MethodInfo>() },
#if NET6_0_OR_GREATER
        // { typeof(DateOnly), new HashSet<MethodInfo>() },
        // { typeof(TimeOnly), new HashSet<MethodInfo>() },
#endif
    };

    public PredefinedMethodsHelper(ParsingConfig config)
    {
        foreach (var kvp in _supported)
        {
            Add(kvp.Key.GetMethod(nameof(Equals), _bindingFlags, null, [kvp.Key], null));
            Add(kvp.Key.GetMethod(nameof(Equals), _bindingFlags, null, [typeof(object)], null));

            Add(kvp.Key.GetMethod(nameof(ToString), _bindingFlags, null, Type.EmptyTypes, null));
            Add(kvp.Key.GetMethod(nameof(ToString), _bindingFlags, null, [typeof(string)], null));
            Add(kvp.Key.GetMethod(nameof(ToString), _bindingFlags, null, [typeof(IFormatProvider)], null));
            Add(kvp.Key.GetMethod(nameof(ToString), _bindingFlags, null, [typeof(string), typeof(IFormatProvider)], null));
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

    private void Add(Type type, MethodInfo? method)
    {
        if (method != null)
        {
            _supported[type].Add(method);
        }
    }
}