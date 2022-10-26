using System.Linq.Dynamic.Core.Validation;
using System.Reflection;

namespace System.Linq.Dynamic.Core;

/// <summary>
/// Default implementation.
/// </summary>
/// <seealso cref="IQueryableAnalyzer" />
public class DefaultQueryableAnalyzer : IQueryableAnalyzer
{
    /// <inheritdoc cref="IQueryableAnalyzer.SupportsLinqToObjects"/>
    public bool SupportsLinqToObjects(IQueryable query, IQueryProvider? provider = null)
    {
        Check.NotNull(query, nameof(query));
        provider = provider ?? query.Provider;

        Type providerType = provider.GetType();
        Type baseType = providerType.GetTypeInfo().BaseType!;
#if NET35
        bool isLinqToObjects = baseType.FullName!.Contains("EnumerableQuery");
#else
        bool isLinqToObjects = baseType == typeof(EnumerableQuery);
#endif
        if (!isLinqToObjects)
        {
            // Support for https://github.com/StefH/QueryInterceptor.Core, version 1.0.1 and up
            if (providerType.Name.StartsWith("QueryTranslatorProvider"))
            {
                try
                {
                    var property = providerType.GetProperty("OriginalProvider");
                    if (property != null)
                    {
                        return property.GetValue(provider, null) is IQueryProvider originalProvider && SupportsLinqToObjects(query, originalProvider);
                    }

                    return SupportsLinqToObjects(query);
                }
                catch
                {
                    return false;
                }
            }

            // Support for https://github.com/scottksmith95/LINQKit ExpandableQuery
            if (providerType.Name.StartsWith("ExpandableQuery"))
            {
                try
                {
                    var property = query.GetType().GetProperty("InnerQuery", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (property != null)
                    {
                        return property.GetValue(query, null) is IQueryable innerQuery && SupportsLinqToObjects(innerQuery, provider);
                    }

                    return SupportsLinqToObjects(query);
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