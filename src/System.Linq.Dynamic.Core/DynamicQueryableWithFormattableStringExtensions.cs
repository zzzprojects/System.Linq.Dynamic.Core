#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace System.Linq.Dynamic.Core
{
#if NET46_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP2_1_OR_GREATER  || NETSTANDARD1_3_OR_GREATER || UAP10_0
    /// <summary>
    /// Provides a set of static extension methods for querying data structures that implement <see cref="IQueryable"/>.
    /// It supports a FormattableString string as predicate.
    /// <seealso cref="DynamicQueryableExtensions"/>
    /// </summary>
    public static class DynamicQueryableWithFormattableStringExtensions
    {
        private static readonly Regex ReplaceArgumentsRegex = new(@"{(\d+)}", RegexOptions.Compiled);

        public static IQueryable WhereInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Where(source, config, predicateStr, args);
        }

        public static IQueryable WhereInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Where(source, predicateStr, args);
        }

        public static IQueryable<TSource> WhereInterpolated<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Where(source, config, predicateStr, args);
        }

        public static IQueryable<TSource> WhereInterpolated<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Where(source, predicateStr, args);
        }

        [PublicAPI]
        public static bool AllInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.All(source, predicateStr, args);
        }

        [PublicAPI]
        public static bool AllInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.All(source, config, predicateStr, args);
        }

        [PublicAPI]
        public static bool AnyInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Any(source, config, predicateStr, args);
        }

        public static bool AnyInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Any(source, predicateStr, args);
        }

        [PublicAPI]
        public static double AverageInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Average(source, config, predicateStr, args);
        }

        [PublicAPI]
        public static double AverageInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Average(source, predicateStr, args);
        }

        public static dynamic SingleInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Single(source, config, predicateStr, args);
        }

        public static dynamic SingleInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Single(source, predicateStr, args);
        }

        public static dynamic SingleOrDefaultInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.SingleOrDefault(source, config, predicateStr, args);
        }

        public static dynamic SingleOrDefaultInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.SingleOrDefault(source, predicateStr, args);
        }

        public static IQueryable SkipWhileInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.SkipWhile(source, config, predicateStr, args);
        }

        public static IQueryable SkipWhileInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.SkipWhile(source, predicateStr, args);
        }

        public static IQueryable TakeWhileInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.TakeWhile(source, config, predicateStr, args);
        }

        public static IQueryable TakeWhileInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.TakeWhile(source, predicateStr, args);
        }

        [PublicAPI]
        public static object SumInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Sum(source, config, predicateStr, args);
        }

        [PublicAPI]
        public static object SumInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Sum(source, predicateStr, args);
        }

        [PublicAPI]
        public static int CountInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Count(source, config, predicateStr, args);
        }

        public static int CountInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Count(source, predicateStr, args);
        }

        public static dynamic FirstInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.First(source, config, predicateStr, args);
        }

        public static dynamic FirstInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.First(source, predicateStr, args);
        }

        public static dynamic FirstOrDefaultInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.FirstOrDefault(source, config, predicateStr, args);
        }

        public static dynamic FirstOrDefaultInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.FirstOrDefault(source, predicateStr, args);
        }

        public static dynamic LastInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Last(source, config, predicateStr, args);
        }

        public static dynamic LastInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Last(source, predicateStr, args);
        }

        public static dynamic LastOrDefaultInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.LastOrDefault(source, config, predicateStr, args);
        }

        public static dynamic LastOrDefaultInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.LastOrDefault(source, predicateStr, args);
        }

        [PublicAPI]
        public static long LongCountInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.LongCount(source, config, predicateStr, args);
        }

        public static long LongCountInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.LongCount(source, predicateStr, args);
        }

        [PublicAPI]
        public static object MaxInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Max(source, config, predicateStr, args);
        }

        [PublicAPI]
        public static object MaxInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Max(source, predicateStr, args);
        }

        [PublicAPI]
        public static object MinInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Min(source, config, predicateStr, args);
        }

        [PublicAPI]
        public static object MinInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString predicate)
        {
            var predicateStr = ParseFormattableString(predicate, out var args);
            return DynamicQueryableExtensions.Min(source, predicateStr, args);
        }

        public static IQueryable SelectInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString selector)
        {
            var selectorStr = ParseFormattableString(selector, out var args);
            return DynamicQueryableExtensions.Select(source, config, selectorStr, args);
        }

        public static IQueryable SelectInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] Type resultType, [NotNull] FormattableString selector)
        {
            var selectorStr = ParseFormattableString(selector, out var args);
            return DynamicQueryableExtensions.Select(source, config, resultType, selectorStr, args);
        }

        public static IQueryable SelectInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString selector)
        {
            var selectorStr = ParseFormattableString(selector, out var args);
            return DynamicQueryableExtensions.Select(source, selectorStr, args);
        }

        public static IQueryable<TResult> SelectInterpolated<TResult>([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString selector)
        {
            var selectorStr = ParseFormattableString(selector, out var args);
            return DynamicQueryableExtensions.Select<TResult>(source, config, selectorStr, args);
        }

        public static IQueryable<TResult> SelectInterpolated<TResult>([NotNull] this IQueryable source, [NotNull] FormattableString selector)
        {
            var selectorStr = ParseFormattableString(selector, out var args);
            return DynamicQueryableExtensions.Select<TResult>(source, selectorStr, args);
        }

        public static IQueryable SelectManyInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString collectionSelector, [NotNull] FormattableString resultSelector)
        {
            var collectionSelectorStr = ParseFormattableString(collectionSelector, out var collectionSelectorArgs);
            var resultSelectorStr = ParseFormattableString(resultSelector, out var resultSelectorArgs);
            return DynamicQueryableExtensions.SelectMany(source, config, collectionSelectorStr, resultSelectorStr, collectionSelectorArgs, resultSelectorArgs);
        }

        public static IQueryable SelectManyInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString selector)
        {
            var selectorStr = ParseFormattableString(selector, out var args);
            return DynamicQueryableExtensions.SelectMany(source, config, selectorStr, args);
        }

        public static IQueryable SelectManyInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] Type resultType, [NotNull] FormattableString selector)
        {
            var selectorStr = ParseFormattableString(selector, out var args);
            return DynamicQueryableExtensions.SelectMany(source, config, resultType, selectorStr, args);
        }

        public static IQueryable SelectManyInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString collectionSelector, [NotNull] FormattableString resultSelector)
        {
            var collectionSelectorStr = ParseFormattableString(collectionSelector, out var collectionSelectorArgs);
            var resultSelectorStr = ParseFormattableString(resultSelector, out var resultSelectorArgs);
            return DynamicQueryableExtensions.SelectMany(source, collectionSelectorStr, collectionSelectorArgs, resultSelectorStr, resultSelectorArgs);
        }

        public static IQueryable SelectManyInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString selector)
        {
            var selectorStr = ParseFormattableString(selector, out var args);
            return DynamicQueryableExtensions.SelectMany(source, selectorStr, args);
        }

        public static IQueryable SelectManyInterpolated([NotNull] this IQueryable source, [NotNull] Type resultType, [NotNull] FormattableString selector)
        {
            var selectorStr = ParseFormattableString(selector, out var args);
            return DynamicQueryableExtensions.SelectMany(source, resultType, selectorStr, args);
        }

        public static IQueryable<TResult> SelectManyInterpolated<TResult>([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString selector)
        {
            var selectorStr = ParseFormattableString(selector, out var args);
            return DynamicQueryableExtensions.SelectMany<TResult>(source, config, selectorStr, args);
        }

        public static IQueryable<TResult> SelectManyInterpolated<TResult>([NotNull] this IQueryable source, [NotNull] FormattableString selector)
        {
            var selectorStr = ParseFormattableString(selector, out var args);
            return DynamicQueryableExtensions.SelectMany<TResult>(source, selectorStr, args);
        }

        public static IOrderedQueryable OrderByInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString ordering, IComparer comparer)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.OrderBy(source, config, orderingStr, comparer, args);
        }

        public static IOrderedQueryable OrderByInterpolated([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString ordering)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.OrderBy(source, config, orderingStr, args);
        }

        public static IOrderedQueryable OrderByInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString ordering, IComparer comparer)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.OrderBy(source, orderingStr, comparer, args);
        }

        public static IOrderedQueryable OrderByInterpolated([NotNull] this IQueryable source, [NotNull] FormattableString ordering)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.OrderBy(source, orderingStr, args);
        }

        public static IOrderedQueryable<TSource> OrderByInterpolated<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] ParsingConfig config, [NotNull] FormattableString ordering, IComparer comparer)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.OrderBy(source, config, orderingStr, comparer, args);
        }

        public static IOrderedQueryable<TSource> OrderByInterpolated<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] ParsingConfig config, [NotNull] FormattableString ordering)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.OrderBy(source, config, orderingStr, args);
        }

        public static IOrderedQueryable<TSource> OrderByInterpolated<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] FormattableString ordering, IComparer comparer)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.OrderBy(source, orderingStr, comparer, args);
        }

        public static IOrderedQueryable<TSource> OrderByInterpolated<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] FormattableString ordering)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.OrderBy(source, orderingStr, args);
        }

        public static IOrderedQueryable ThenByInterpolated([NotNull] this IOrderedQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString ordering, IComparer comparer)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.ThenBy(source, config, orderingStr, comparer, args);
        }

        public static IOrderedQueryable ThenByInterpolated([NotNull] this IOrderedQueryable source, [NotNull] ParsingConfig config, [NotNull] FormattableString ordering)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.ThenBy(source, config, orderingStr, args);
        }

        public static IOrderedQueryable ThenByInterpolated([NotNull] this IOrderedQueryable source, [NotNull] FormattableString ordering, IComparer comparer)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.ThenBy(source, orderingStr, comparer, args);
        }

        public static IOrderedQueryable ThenByInterpolated([NotNull] this IOrderedQueryable source, [NotNull] FormattableString ordering)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.ThenBy(source, orderingStr, args);
        }

        public static IOrderedQueryable<TSource> ThenByInterpolated<TSource>([NotNull] this IOrderedQueryable<TSource> source, [NotNull] ParsingConfig config, [NotNull] FormattableString ordering, IComparer comparer)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.ThenBy(source, config, orderingStr, comparer, args);
        }

        public static IOrderedQueryable<TSource> ThenByInterpolated<TSource>([NotNull] this IOrderedQueryable<TSource> source, [NotNull] ParsingConfig config, [NotNull] FormattableString ordering)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.ThenBy(source, config, orderingStr, args);
        }

        public static IOrderedQueryable<TSource> ThenByInterpolated<TSource>([NotNull] this IOrderedQueryable<TSource> source, [NotNull] FormattableString ordering, IComparer comparer)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.ThenBy(source, orderingStr, comparer, args);
        }

        public static IOrderedQueryable<TSource> ThenByInterpolated<TSource>([NotNull] this IOrderedQueryable<TSource> source, [NotNull] FormattableString ordering)
        {
            var orderingStr = ParseFormattableString(ordering, out var args);
            return DynamicQueryableExtensions.ThenBy(source, orderingStr, args);
        }

        private static string ParseFormattableString(FormattableString predicate, out object[] args)
        {
            args = predicate.GetArguments();
            return ReplaceArgumentsRegex.Replace(predicate.Format, "@$1"); // replace {0} with @0
        }
    }
#endif
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member