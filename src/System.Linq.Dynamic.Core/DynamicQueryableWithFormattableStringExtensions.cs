#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace System.Linq.Dynamic.Core;

#if NET46_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP2_1_OR_GREATER  || NETSTANDARD1_3_OR_GREATER || UAP10_0
/// <summary>
/// Provides a set of static extension methods for querying data structures that implement <see cref="IQueryable"/>.
/// It supports a FormattableString string as predicate.
/// <seealso cref="DynamicQueryableExtensions"/>
/// </summary>
public static class DynamicQueryableWithFormattableStringExtensions
{
    private static readonly Regex ReplaceArgumentsRegex = new(@"{(\d+)}", RegexOptions.Compiled);

    public static IQueryable WhereInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Where(config, predicateStr, args);
    }

    public static IQueryable WhereInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Where(predicateStr, args);
    }

    public static IQueryable<TSource> WhereInterpolated<TSource>(this IQueryable<TSource> source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Where(config, predicateStr, args);
    }

    public static IQueryable<TSource> WhereInterpolated<TSource>(this IQueryable<TSource> source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Where(predicateStr, args);
    }

    [PublicAPI]
    public static bool AllInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.All(predicateStr, args);
    }

    [PublicAPI]
    public static bool AllInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.All(config, predicateStr, args);
    }

    [PublicAPI]
    public static bool AnyInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Any(config, predicateStr, args);
    }

    public static bool AnyInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Any(predicateStr, args);
    }

    [PublicAPI]
    public static double AverageInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Average(config, predicateStr, args);
    }

    [PublicAPI]
    public static double AverageInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Average(predicateStr, args);
    }

    public static dynamic SingleInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Single(config, predicateStr, args);
    }

    public static dynamic SingleInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Single(predicateStr, args);
    }

    public static dynamic SingleOrDefaultInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.SingleOrDefault(config, predicateStr, args);
    }

    public static dynamic SingleOrDefaultInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.SingleOrDefault(predicateStr, args);
    }

    public static IQueryable SkipWhileInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.SkipWhile(config, predicateStr, args);
    }

    public static IQueryable SkipWhileInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.SkipWhile(predicateStr, args);
    }

    public static IQueryable TakeWhileInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.TakeWhile(config, predicateStr, args);
    }

    public static IQueryable TakeWhileInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.TakeWhile(predicateStr, args);
    }

    [PublicAPI]
    public static object SumInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Sum(config, predicateStr, args);
    }

    [PublicAPI]
    public static object SumInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Sum(predicateStr, args);
    }

    [PublicAPI]
    public static int CountInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Count(config, predicateStr, args);
    }

    public static int CountInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Count(predicateStr, args);
    }

    public static dynamic FirstInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.First(config, predicateStr, args);
    }

    public static dynamic FirstInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.First(predicateStr, args);
    }

    public static dynamic FirstOrDefaultInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.FirstOrDefault(config, predicateStr, args);
    }

    public static dynamic FirstOrDefaultInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.FirstOrDefault(predicateStr, args);
    }

    public static dynamic LastInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Last(config, predicateStr, args);
    }

    public static dynamic LastInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Last(predicateStr, args);
    }

    public static dynamic LastOrDefaultInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.LastOrDefault(config, predicateStr, args);
    }

    public static dynamic LastOrDefaultInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.LastOrDefault(predicateStr, args);
    }

    [PublicAPI]
    public static long LongCountInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.LongCount(config, predicateStr, args);
    }

    public static long LongCountInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.LongCount(predicateStr, args);
    }

    [PublicAPI]
    public static object MaxInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Max(config, predicateStr, args);
    }

    [PublicAPI]
    public static object MaxInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Max(predicateStr, args);
    }

    [PublicAPI]
    public static object MinInterpolated(this IQueryable source, ParsingConfig config, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Min(config, predicateStr, args);
    }

    [PublicAPI]
    public static object MinInterpolated(this IQueryable source, FormattableString predicate)
    {
        var predicateStr = ParseFormattableString(predicate, out var args);
        return source.Min(predicateStr, args);
    }

    public static IQueryable SelectInterpolated(this IQueryable source, ParsingConfig config, FormattableString selector)
    {
        var selectorStr = ParseFormattableString(selector, out var args);
        return source.Select(config, selectorStr, args);
    }

    public static IQueryable SelectInterpolated(this IQueryable source, ParsingConfig config, Type resultType, FormattableString selector)
    {
        var selectorStr = ParseFormattableString(selector, out var args);
        return source.Select(config, resultType, selectorStr, args);
    }

    public static IQueryable SelectInterpolated(this IQueryable source, FormattableString selector)
    {
        var selectorStr = ParseFormattableString(selector, out var args);
        return source.Select(selectorStr, args);
    }

    public static IQueryable<TResult> SelectInterpolated<TResult>(this IQueryable source, ParsingConfig config, FormattableString selector)
    {
        var selectorStr = ParseFormattableString(selector, out var args);
        return source.Select<TResult>(config, selectorStr, args);
    }

    public static IQueryable<TResult> SelectInterpolated<TResult>(this IQueryable source, FormattableString selector)
    {
        var selectorStr = ParseFormattableString(selector, out var args);
        return source.Select<TResult>(selectorStr, args);
    }

    public static IQueryable SelectManyInterpolated(this IQueryable source, ParsingConfig config, FormattableString collectionSelector, FormattableString resultSelector)
    {
        var collectionSelectorStr = ParseFormattableString(collectionSelector, out var collectionSelectorArgs);
        var resultSelectorStr = ParseFormattableString(resultSelector, out var resultSelectorArgs);
        return source.SelectMany(config, collectionSelectorStr, resultSelectorStr, collectionSelectorArgs, resultSelectorArgs);
    }

    public static IQueryable SelectManyInterpolated(this IQueryable source, ParsingConfig config, FormattableString selector)
    {
        var selectorStr = ParseFormattableString(selector, out var args);
        return source.SelectMany(config, selectorStr, args);
    }

    public static IQueryable SelectManyInterpolated(this IQueryable source, ParsingConfig config, Type resultType, FormattableString selector)
    {
        var selectorStr = ParseFormattableString(selector, out var args);
        return source.SelectMany(config, resultType, selectorStr, args);
    }

    public static IQueryable SelectManyInterpolated(this IQueryable source, FormattableString collectionSelector, FormattableString resultSelector)
    {
        var collectionSelectorStr = ParseFormattableString(collectionSelector, out var collectionSelectorArgs);
        var resultSelectorStr = ParseFormattableString(resultSelector, out var resultSelectorArgs);
        return source.SelectMany(collectionSelectorStr, collectionSelectorArgs, resultSelectorStr, resultSelectorArgs);
    }

    public static IQueryable SelectManyInterpolated(this IQueryable source, FormattableString selector)
    {
        var selectorStr = ParseFormattableString(selector, out var args);
        return source.SelectMany(selectorStr, args);
    }

    public static IQueryable SelectManyInterpolated(this IQueryable source, Type resultType, FormattableString selector)
    {
        var selectorStr = ParseFormattableString(selector, out var args);
        return source.SelectMany(resultType, selectorStr, args);
    }

    public static IQueryable<TResult> SelectManyInterpolated<TResult>(this IQueryable source, ParsingConfig config, FormattableString selector)
    {
        var selectorStr = ParseFormattableString(selector, out var args);
        return source.SelectMany<TResult>(config, selectorStr, args);
    }

    public static IQueryable<TResult> SelectManyInterpolated<TResult>(this IQueryable source, FormattableString selector)
    {
        var selectorStr = ParseFormattableString(selector, out var args);
        return source.SelectMany<TResult>(selectorStr, args);
    }

    public static IOrderedQueryable OrderByInterpolated(this IQueryable source, ParsingConfig config, FormattableString ordering, IComparer comparer)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.OrderBy(config, orderingStr, comparer, args);
    }

    public static IOrderedQueryable OrderByInterpolated(this IQueryable source, ParsingConfig config, FormattableString ordering)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.OrderBy(config, orderingStr, args);
    }

    public static IOrderedQueryable OrderByInterpolated(this IQueryable source, FormattableString ordering, IComparer comparer)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.OrderBy(orderingStr, comparer, args);
    }

    public static IOrderedQueryable OrderByInterpolated(this IQueryable source, FormattableString ordering)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.OrderBy(orderingStr, args);
    }

    public static IOrderedQueryable<TSource> OrderByInterpolated<TSource>(this IQueryable<TSource> source, ParsingConfig config, FormattableString ordering, IComparer comparer)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.OrderBy(config, orderingStr, comparer, args);
    }

    public static IOrderedQueryable<TSource> OrderByInterpolated<TSource>(this IQueryable<TSource> source, ParsingConfig config, FormattableString ordering)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.OrderBy(config, orderingStr, args);
    }

    public static IOrderedQueryable<TSource> OrderByInterpolated<TSource>(this IQueryable<TSource> source, FormattableString ordering, IComparer comparer)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.OrderBy(orderingStr, comparer, args);
    }

    public static IOrderedQueryable<TSource> OrderByInterpolated<TSource>(this IQueryable<TSource> source, FormattableString ordering)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.OrderBy(orderingStr, args);
    }

    public static IOrderedQueryable ThenByInterpolated(this IOrderedQueryable source, ParsingConfig config, FormattableString ordering, IComparer comparer)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.ThenBy(config, orderingStr, comparer, args);
    }

    public static IOrderedQueryable ThenByInterpolated(this IOrderedQueryable source, ParsingConfig config, FormattableString ordering)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.ThenBy(config, orderingStr, args);
    }

    public static IOrderedQueryable ThenByInterpolated(this IOrderedQueryable source, FormattableString ordering, IComparer comparer)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.ThenBy(orderingStr, comparer, args);
    }

    public static IOrderedQueryable ThenByInterpolated(this IOrderedQueryable source, FormattableString ordering)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.ThenBy(orderingStr, args);
    }

    public static IOrderedQueryable<TSource> ThenByInterpolated<TSource>(this IOrderedQueryable<TSource> source, ParsingConfig config, FormattableString ordering, IComparer comparer)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.ThenBy(config, orderingStr, comparer, args);
    }

    public static IOrderedQueryable<TSource> ThenByInterpolated<TSource>(this IOrderedQueryable<TSource> source, ParsingConfig config, FormattableString ordering)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.ThenBy(config, orderingStr, args);
    }

    public static IOrderedQueryable<TSource> ThenByInterpolated<TSource>(this IOrderedQueryable<TSource> source, FormattableString ordering, IComparer comparer)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.ThenBy(orderingStr, comparer, args);
    }

    public static IOrderedQueryable<TSource> ThenByInterpolated<TSource>(this IOrderedQueryable<TSource> source, FormattableString ordering)
    {
        var orderingStr = ParseFormattableString(ordering, out var args);
        return source.ThenBy(orderingStr, args);
    }

    private static string ParseFormattableString(FormattableString predicate, out object?[] args)
    {
        args = predicate.GetArguments();
        return ReplaceArgumentsRegex.Replace(predicate.Format, "@$1"); // replace {0} with @0
    }
}
#endif

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member