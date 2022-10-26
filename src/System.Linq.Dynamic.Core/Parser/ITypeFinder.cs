using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser
{
    interface ITypeFinder
    {
        Type? FindTypeByName(string name, ParameterExpression?[]? expressions, bool forceUseCustomTypeProvider);
    }
}