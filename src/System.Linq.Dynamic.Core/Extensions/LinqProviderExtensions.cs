using System.Reflection;
using JetBrains.Annotations;
using System.Linq.Dynamic.Core.Validation;

namespace System.Linq.Dynamic.Core.Extensions
{
    internal static class LinqProviderExtensions
    {
        /// <summary>
        /// Check if the Provider from IQueryable is a LinqToObjects provider.
        /// </summary>
        /// <param name="source">The IQueryable</param>
        /// <returns>true if provider is LinqToObjects, else false</returns>
        public static bool IsLinqToObjects([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(source.Provider, nameof(source.Provider));

            return IsProviderEnumerableQuery(source.Provider);
        }

        private static bool IsProviderEnumerableQuery(IQueryProvider provider)
        {
            Type baseType = provider.GetType().GetTypeInfo().BaseType;
#if NET35
            bool isLinqToObjects = baseType.FullName.Contains("EnumerableQuery");
#else
            bool isLinqToObjects = baseType == typeof(EnumerableQuery);
#endif
            if (!isLinqToObjects)
            {
                // add support for https://github.com/StefH/QueryInterceptor.Core, version 1.0.1 and up
                if (baseType.Name == "QueryTranslatorProvider")
                {
                    try
                    {
                        PropertyInfo property = baseType.GetProperty("OriginalProvider");
                        IQueryProvider originalProvider = property.GetValue(provider, null) as IQueryProvider;
                        return originalProvider != null && IsProviderEnumerableQuery(originalProvider);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            return isLinqToObjects;
        }
    }
}