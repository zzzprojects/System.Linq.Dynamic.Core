using System.Linq.Expressions;
using JetBrains.Annotations;

namespace System.Linq.Dynamic.Core.Parser
{
    interface ITypeFinder
    {
        Type FindTypeByName([NotNull] string name, [CanBeNull] ParameterExpression[] expressions, bool forceUseCustomTypeProvider = false);
    }
}
