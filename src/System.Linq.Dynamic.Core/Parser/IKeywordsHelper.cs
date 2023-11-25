using System.Diagnostics.CodeAnalysis;

namespace System.Linq.Dynamic.Core.Parser;

interface IKeywordsHelper
{
    bool TryGetValue(string name, [NotNullWhen(true)] out object? keyWordOrType);
}