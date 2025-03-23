using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core;

internal struct DynamicOrdering
{
    public Expression Selector { get; }
    public bool Ascending { get; }
    public string MethodName { get; }

    public DynamicOrdering(Expression selector, bool ascending, string methodName)
    {
        Selector = selector;
        Ascending = ascending;
        MethodName = methodName;
    }
}