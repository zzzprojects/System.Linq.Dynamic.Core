using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Extensions
{
    internal static class LinqExtensions
    {
        // Ex: collection.TakeLast(5);
        public static IEnumerable<T> TakeLast<T>(this IList<T> source, int n)
        {
            return source.Skip(Math.Max(0, source.Count() - n));
        }
    }
}