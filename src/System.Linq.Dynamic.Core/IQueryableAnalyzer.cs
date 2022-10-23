namespace System.Linq.Dynamic.Core;

/// <summary>
/// Interface for QueryableAnalyzer.
/// </summary>
public interface IQueryableAnalyzer
{
    /// <summary>
    /// Determines whether the specified query (and provider) supports LinqToObjects.
    /// </summary>
    /// <param name="query">The query to check.</param>
    /// <param name="provider">The provider to check (can be null).</param>
    /// <returns>true/false</returns>
    bool SupportsLinqToObjects(IQueryable query, IQueryProvider? provider = null);
}