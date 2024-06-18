using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core.Validation;

namespace System.Linq.Dynamic.Core.Extensions;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
internal static class LinqExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> sequence)
    {
        Check.NotNull(sequence);

        return sequence.Where(e => e != null)!;
    }
}