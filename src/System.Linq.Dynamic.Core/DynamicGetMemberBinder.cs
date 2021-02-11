#if !NET35 && !UAP10_0 && !NETSTANDARD1_3
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace System.Linq.Dynamic.Core
{
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
                throw new InvalidOperationException();

            if (value is IDictionary<string, object> dict1)
                return dict1[name];

            if (value is IDictionary dict2)
                return dict2[name];

            var flags = BindingFlags.Instance | BindingFlags.Public;
            if (ignoreCase) flags |= BindingFlags.IgnoreCase;
            var property = value.GetType().GetProperty(name, flags);
            if (property == null)
                throw new InvalidOperationException();

            return property.GetValue(value, null);
        }
    }
}
#endif
