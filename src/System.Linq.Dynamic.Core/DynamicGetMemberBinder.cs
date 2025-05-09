#if !NET35 && !UAP10_0 && !NETSTANDARD1_3
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core;

/// <summary>
/// Code is based on SqlLinq by dkackman (https://github.com/dkackman/SqlLinq/blob/210b594e37f14061424397368ed750ce547c21e7/License.md) however it's modified to solve several issues.
/// </summary>
/// <seealso cref="GetMemberBinder" />
internal class DynamicGetMemberBinder : GetMemberBinder
{
    private static readonly MethodInfo DynamicGetMemberMethod = typeof(DynamicGetMemberBinder).GetMethod(nameof(GetDynamicMember))!;
    private readonly ConcurrentDictionary<Tuple<Type, string, bool>, DynamicMetaObject> _metaObjectCache = new();

    public DynamicGetMemberBinder(string name, ParsingConfig? config) : base(name, config?.IsCaseSensitive != true)
    {
    }

    public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject? errorSuggestion)
    {
        var methodCallExpression = Expression.Call(
            DynamicGetMemberMethod,
            target.Expression,
            Expression.Constant(Name),
            Expression.Constant(IgnoreCase));

        // Fix #907 and #912: "The result of the dynamic binding produced by the object with type '<>f__AnonymousType1`4' for the binder 'System.Linq.Dynamic.Core.DynamicGetMemberBinder' needs at least one restriction.".
        // Fix #921: "Slow Performance"
        // Only add TypeRestriction if it's a Dynamic type and make sure to cache the DynamicMetaObject.
        if (target.Value is IDynamicMetaObjectProvider)
        {
            var key = new Tuple<Type, string, bool>(target.LimitType, Name, IgnoreCase);

            return _metaObjectCache.GetOrAdd(key, _ =>
            {
                var restrictions = BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType);
                return new DynamicMetaObject(methodCallExpression, restrictions, target.Value);
            });
        }

        return DynamicMetaObject.Create(target.Value!, methodCallExpression);
    }

    public static object? GetDynamicMember(object value, string name, bool ignoreCase)
    {
        if (value == null)
        {
            throw new InvalidOperationException();
        }

        if (value is IDictionary<string, object> stringObjectDictionary)
        {
            return stringObjectDictionary[name];
        }

        if (value is IDictionary nonGenericDictionary)
        {
            return nonGenericDictionary[name];
        }

        var flags = BindingFlags.Instance | BindingFlags.Public;
        if (ignoreCase)
        {
            flags |= BindingFlags.IgnoreCase;
        }

        var type = value.GetType();
        var property = type.GetProperty(name, flags);
        if (property == null)
        {
            throw new InvalidOperationException($"Unable to find property '{name}' on type '{type}'.");
        }

        return property.GetValue(value, null);
    }
}

#endif