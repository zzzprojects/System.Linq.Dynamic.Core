using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core
{
    internal class DynamicOrdering
    {
        public Expression Selector;
        public bool Ascending;
        public string MethodName;
    }
}