#if !NET35
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Based on From SqlLinq by dkackman. https://github.com/dkackman/SqlLinq/blob/210b594e37f14061424397368ed750ce547c21e7/License.md
    /// </summary>
    /// <seealso cref="GetMemberBinder" />
    internal class DynamicGetMemberBinder : GetMemberBinder
    {
        private static readonly PropertyInfo Indexer = typeof(IDictionary<string, object>).GetProperty("Item");

        public DynamicGetMemberBinder(string name)
            : base(name, true)
        {
        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            IDictionary<string, object> dictionary = target.Value as IDictionary<string, object>;
            if (dictionary == null)
            {
                throw new InvalidOperationException("Target object is not an ExpandoObject");
            }

            return DynamicMetaObject.Create(dictionary, Expression.MakeIndex(Expression.Constant(dictionary), Indexer, new Expression[] { Expression.Constant(Name) }));
        }
    }
}
#endif
