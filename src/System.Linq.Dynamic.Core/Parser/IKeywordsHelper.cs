using System.Linq.Expressions;
using AnyOfTypes;

namespace System.Linq.Dynamic.Core.Parser;

interface IKeywordsHelper
{
    bool TryGetValue(string name, out AnyOf<string, Expression, Type> keywordOrType);
}