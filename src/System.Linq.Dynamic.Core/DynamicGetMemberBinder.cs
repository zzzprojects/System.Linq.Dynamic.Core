#if !NET35 && !UAP10_0 && !NETSTANDARD1_3
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Based on SqlLinq by dkackman. https://github.com/dkackman/SqlLinq/blob/210b594e37f14061424397368ed750ce547c21e7/License.md
    /// </summary>
    /// <seealso cref="GetMemberBinder" />
    internal class DynamicGetMemberBinder : GetMemberBinder
    {
        private static readonly Type IDictionaryType = typeof(IDictionary<string, object>);
        private static readonly PropertyInfo Indexer = IDictionaryType.GetProperty("Item");

        public DynamicGetMemberBinder(string name, [CanBeNull] ParsingConfig config) : base(name, !(config?.IsCaseSensitive == true))
        {
        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            var dictionary = target.Value as IDictionary<string, object>;
            if (dictionary == null)
            {
                throw new InvalidOperationException("Target object is not an ExpandoObject");
            }

            var instance = Expression.ConvertChecked(target.Expression, IDictionaryType);
            var indexExpression = Expression.MakeIndex(instance, Indexer, new Expression[] { Expression.Constant(Name) });

            return DynamicMetaObject.Create(dictionary, indexExpression);
        }
    }
}
#endif
