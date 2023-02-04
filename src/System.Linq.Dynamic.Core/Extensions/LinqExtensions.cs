using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;

namespace System.Linq.Dynamic.Core.Extensions;

internal static class LinqExtensions
{
    // Ex: collection.TakeLast(5);
    public static IEnumerable<T> TakeLast<T>(this IList<T> source, int n)
    {
        return source.Skip(Math.Max(0, source.Count() - n));
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> sequence, Func<T, bool>? predicate = null)
    {
        Check.NotNull(sequence);

        return sequence
            .Where(e => e != null)
            .Where(e => predicate != null && predicate(e!))!;
    }
}