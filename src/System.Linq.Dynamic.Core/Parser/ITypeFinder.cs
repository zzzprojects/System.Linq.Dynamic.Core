using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser;

internal interface ITypeFinder
{
    bool TryFindTypeByName(string name, ParameterExpression?[]? expressions, bool forceUseCustomTypeProvider, [NotNullWhen(true)] out Type? type);
}