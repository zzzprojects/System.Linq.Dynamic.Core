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
            Check.NotNull(source.Provider, nameof(source.Provider));

            return IsProviderEnumerableQuery(source.Provider);
        }

        private static bool IsProviderEnumerableQuery(IQueryProvider provider)
        {
            Type baseType = provider.GetType().BaseType();
#if NET35
            bool isLinqToObjects = baseType.FullName.Contains("EnumerableQuery");
#else
            bool isLinqToObjects = baseType == typeof(EnumerableQuery);
#endif
            if (!isLinqToObjects)
            {
                if (baseType.Name == "QueryTranslatorProvider")
                {
                    //IQueryProvider queryTranslatorProvider = provider.GetSourceProvider();

                    //return queryTranslatorProvider != null && IsProviderEnumerableQuery(queryTranslatorProvider);
                }
            }

            return isLinqToObjects;
        }

//        private static IQueryProvider GetSourceProvider(this IQueryProvider provider)
//        {
//#if NET35
//            var type = provider.GetType();
//            var propInfo = type.GetProperty("Source");
//            IQueryProvider queryTranslatorProvider = propInfo.GetValue(provider, null) as IQueryProvider;
//#else
//            dynamic p = provider;
//            IQueryProvider queryTranslatorProvider = p.Source;

//#endif
//            return queryTranslatorProvider;
//        }
    }
}