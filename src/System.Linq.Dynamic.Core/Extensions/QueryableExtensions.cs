using JetBrains.Annotations;
using System.Linq.Dynamic.Core.Validation;
using ReflectionBridge.Extensions;

namespace System.Linq.Dynamic.Core.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Check if the Provider from IQueryable is a LinqToObjects provider.
        /// </summary>
        /// <param name="source">The IQueryable</param>
        /// <returns>true if provider is LinqToObjects, else false</returns>
        public static bool IsLinqToObjects([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

#if NET35
            return source.Provider.GetType().BaseType().FullName.Contains("EnumerableQuery");
#else
            return source.Provider.GetType().BaseType() == typeof(EnumerableQuery);
#endif
        }
    }
}