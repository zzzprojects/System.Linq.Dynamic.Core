#if !NET35 && !UAP10_0 && !NETSTANDARD1_3
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Code is based on SqlLinq by dkackman (https://github.com/dkackman/SqlLinq/blob/210b594e37f14061424397368ed750ce547c21e7/License.md) however it's modified to solve several issues.
    /// </summary>
    /// <seealso cref="GetMemberBinder" />
    internal class DynamicGetMemberBinder : GetMemberBinder
    {
        private static readonly MethodInfo DynamicGetMemberMethod = typeof(DynamicGetMemberBinder).GetMethod(nameof(GetDynamicMember));

        public DynamicGetMemberBinder(string name, [CanBeNull] ParsingConfig config) : base(name, !(config?.IsCaseSensitive == true))
        {
        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            var instance = Expression.Call(
                DynamicGetMemberMethod,
                target.Expression,
                Expression.Constant(Name),
                Expression.Constant(IgnoreCase));

            return DynamicMetaObject.Create(target.Value, instance);
        }

        public static object GetDynamicMember(object value, string name, bool ignoreCase)
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
}
#endif
