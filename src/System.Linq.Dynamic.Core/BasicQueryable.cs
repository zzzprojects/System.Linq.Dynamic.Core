using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using JetBrains.Annotations;
using System.Linq.Dynamic.Core.Extensions;
#if WINDOWS_APP
using System;
using System.Linq;
#endif

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Provides a set of static (Shared in Visual Basic) methods for querying data 
    /// structures that implement <see cref="IQueryable"/>. It adds basic methods to <see cref="IQueryable"/> that would
    /// normally require <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class BasicQueryable
    {
        /// <summary>
        /// Returns the elements as paged.
        /// </summary>
        /// <param name="source">The IQueryable to return elements from.</param>
        /// <param name="page">The page to return.</param>
        /// <param name="pageSize">The number of elements per page.</param>
        /// <returns>A <see cref="IQueryable"/> that contains the paged elements.</returns>
        public static IQueryable Page([NotNull] this IQueryable source, int page, int pageSize)
        {
            Check.NotNull(source, nameof(source));
            Check.Condition(page, p => p > 0, nameof(page));
            Check.Condition(pageSize, ps => ps > 0, nameof(pageSize));

            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Returns the elements as paged.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The IQueryable to return elements from.</param>
        /// <param name="page">The page to return.</param>
        /// <param name="pageSize">The number of elements per page.</param>
        /// <returns>A <see cref="IQueryable{TSource}"/> that contains the paged elements.</returns>
        public static IQueryable<TSource> Page<TSource>([NotNull] this IQueryable<TSource> source, int page, int pageSize)
        {
            Check.NotNull(source, nameof(source));
            Check.Condition(page, p => p > 0, nameof(page));
            Check.Condition(pageSize, ps => ps > 0, nameof(pageSize));

            return Queryable.Take(Queryable.Skip(source, (page - 1) * pageSize), pageSize);
        }

        /// <summary>
        /// Returns the elements as paged and include the CurrentPage, PageCount, PageSize and RowCount.
        /// </summary>
        /// <param name="source">The IQueryable to return elements from.</param>
        /// <param name="page">The page to return.</param>
        /// <param name="pageSize">The number of elements per page.</param>
        /// <returns>PagedResult</returns>
        public static PagedResult PageResult([NotNull] this IQueryable source, int page, int pageSize)
        {
            Check.NotNull(source, nameof(source));
            Check.Condition(page, p => p > 0, nameof(page));
            Check.Condition(pageSize, ps => ps > 0, nameof(pageSize));

            var result = new PagedResult
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = source.Count()
            };

            result.PageCount = (int)Math.Ceiling((double)result.RowCount / pageSize);
            result.Queryable = Page(source, page, pageSize);

            return result;
        }

        /// <summary>
        /// Returns the elements as paged and include the CurrentPage, PageCount, PageSize and RowCount.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The IQueryable to return elements from.</param>
        /// <param name="page">The page to return.</param>
        /// <param name="pageSize">The number of elements per page.</param>
        /// <returns>PagedResult{TSource}</returns>
        public static PagedResult<TSource> PageResult<TSource>([NotNull] this IQueryable<TSource> source, int page, int pageSize)
        {
            Check.NotNull(source, nameof(source));
            Check.Condition(page, p => p > 0, nameof(page));
            Check.Condition(pageSize, ps => ps > 0, nameof(pageSize));

            var result = new PagedResult<TSource>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = Queryable.Count(source)
            };

            result.PageCount = (int)Math.Ceiling((double)result.RowCount / pageSize);
            result.Queryable = Page(source, page, pageSize);

            return result;
        }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A <see cref="IQueryable"/> that contains the specified number of elements from the start of source.</returns>
        public static IQueryable Take([NotNull] this IQueryable source, int count)
        {
            Check.NotNull(source, nameof(source));
            Check.Condition(count, x => x > 0, nameof(count));

            return Queryable.Take((IQueryable<object>)source, count);
        }

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> to return elements from.</param>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns>A <see cref="IQueryable"/> that contains elements that occur after the specified index in the input sequence.</returns>
        public static IQueryable Skip([NotNull] this IQueryable source, int count)
        {
            Check.NotNull(source, nameof(source));
            Check.Condition(count, x => x >= 0, nameof(count));

            //no need to skip if count is zero
            if (count == 0)
                return source;

            return Queryable.Skip((IQueryable<object>)source, count);
        }

        /// <summary>
        /// Inverts the order of the elements in a sequence.
        /// </summary>
        /// <param name="source">A sequence of values to reverse.</param>
        /// <returns>A <see cref="IQueryable"/> whose elements correspond to those of the input sequence in reverse order.</returns>
        public static IQueryable Reverse([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            return Queryable.Reverse((IQueryable<object>)source);
        }

        /// <summary>
        /// Returns distinct elements from a sequence by using the default equality comparer to compare values.
        /// </summary>
        /// <param name="source">The sequence to remove duplicate elements from.</param>
        /// <returns>An <see cref="IQueryable"/> that contains distinct elements from the source sequence.</returns>
        public static IQueryable Distinct([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Distinct",
                    new Type[] { source.ElementType },
                    source.Expression));
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        public static bool Any([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            return Queryable.Any((IQueryable<object>)source);
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        public static bool Any([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpression.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return source.Provider.Execute<bool>(
                Expression.Call(
                    typeof(Queryable), "Any",
                    new[] { source.ElementType },
                    source.Expression,
                    Expression.Quote(lambda)
                )
            );
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> that contains the elements to be counted.</param>
        /// <returns>The number of elements in the input sequence.</returns>
        public static int Count([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            return Queryable.Count((IQueryable<object>)source);
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> that contains the elements to be counted.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
        public static int Count([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpression.ParseLambda(createParameterCtor, source.ElementType, typeof(bool), predicate, args);

            return source.Provider.Execute<int>(
                Expression.Call(
                    typeof(Queryable), "Count",
                    new[] { source.ElementType },
                    source.Expression,
                    Expression.Quote(lambda)
                )
            );
        }

        /// <summary>
        /// Computes the sum of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the sum of.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        public static object Sum([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            return source.Provider.Execute(
                Expression.Call(
                typeof(Queryable), "Sum",
                null,
                source.Expression));
        }

        /// <summary>
        /// Returns the only element of a sequence, and throws an exception if there
        /// is not exactly one element in the sequence.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> to return the single element of.</param>
        /// <returns>The single element of the input sequence.</returns>
#if NET35
        public static object Single([NotNull] this IQueryable source)
#else
        public static dynamic Single([NotNull] this IQueryable source)
#endif
        {
            Check.NotNull(source, nameof(source));

            return source.Provider.Execute(Expression.Call(
                typeof(Queryable), "Single",
                new[] { source.ElementType }, source.Expression));
        }

        /// <summary>
        /// Returns the only element of a sequence, or a default value if the sequence
        /// is empty; this method throws an exception if there is more than one element
        /// in the sequence.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> to return the single element of.</param>
        /// <returns>The single element of the input sequence, or default(TSource) if the sequence contains no elements.</returns>
#if NET35
        public static object SingleOrDefault([NotNull] this IQueryable source)
#else
        public static dynamic SingleOrDefault([NotNull] this IQueryable source)
#endif
        {
            Check.NotNull(source, nameof(source));

            return source.Provider.Execute(Expression.Call(
                typeof(Queryable), "SingleOrDefault",
                new[] { source.ElementType }, source.Expression));
        }

        /// <summary>
        /// Returns the first element of a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the first element of.</param>
        /// <returns>The first element in source.</returns>
#if NET35
        public static object First([NotNull] this IQueryable source)
#else
        public static dynamic First([NotNull] this IQueryable source)
#endif
        {
            Check.NotNull(source, nameof(source));

            return source.Provider.Execute(Expression.Call(
                typeof(Queryable), "First",
                new[] { source.ElementType }, source.Expression));
        }

        /// <summary>
        /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the first element of.</param>
        /// <returns>default(TSource) if source is empty; otherwise, the first element in source.</returns>
#if NET35
        public static object FirstOrDefault([NotNull] this IQueryable source)
#else
        public static dynamic FirstOrDefault([NotNull] this IQueryable source)
#endif
        {
            Check.NotNull(source, nameof(source));

            return source.Provider.Execute(Expression.Call(
                typeof(Queryable), "FirstOrDefault",
                new[] { source.ElementType }, source.Expression));
        }

        /// <summary>
        /// Returns the last element of a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <returns>The last element in source.</returns>
#if NET35
        public static object Last([NotNull] this IQueryable source)
#else
        public static dynamic Last([NotNull] this IQueryable source)
#endif
        {
            Check.NotNull(source, nameof(source));

            return source.Provider.Execute(Expression.Call(
                typeof(Queryable), "Last",
                new[] { source.ElementType }, source.Expression));
        }

        /// <summary>
        /// Returns the last element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <returns>default(TSource) if source is empty; otherwise, the last element in source.</returns>
#if NET35
        public static object LastOrDefault([NotNull] this IQueryable source)
#else
        public static dynamic LastOrDefault([NotNull] this IQueryable source)
#endif
        {
            Check.NotNull(source, nameof(source));

            return source.Provider.Execute(Expression.Call(
                typeof(Queryable), "LastOrDefault",
                new[] { source.ElementType }, source.Expression));
        }

#if NET35
        /// <summary>
        /// Returns the input typed as <see cref="IEnumerable{T}"/> of <see cref="object"/>./>
        /// </summary>
        /// <param name="source">The sequence to type as <see cref="IEnumerable{T}"/> of <see cref="object"/>.</param>
        /// <returns>The input typed as <see cref="IEnumerable{T}"/> of <see cref="object"/>.</returns>
        public static IEnumerable<object> AsEnumerable([NotNull] this IQueryable source)
#else
        /// <summary>
        /// Returns the input typed as <see cref="IEnumerable{T}"/> of dynamic.
        /// </summary>
        /// <param name="source">The sequence to type as <see cref="IEnumerable{T}"/> of dynamic.</param>
        /// <returns>The input typed as <see cref="IEnumerable{T}"/> of dynamic.</returns>
        public static IEnumerable<dynamic> AsEnumerable([NotNull] this IQueryable source)
#endif
        {
            foreach (var obj in source)
            {
                yield return obj;
            }
        }

        /// <summary>
        /// Creates an array of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <returns>An array that contains the elements from the input sequence.</returns>
#if NET35
        public static object[] ToDynamicArray([NotNull] this IEnumerable source)
#else
        public static dynamic[] ToDynamicArray([NotNull] this IEnumerable source)
#endif
        {
            Check.NotNull(source, nameof(source));
            return CastToArray<object>(source);
        }

        /// <summary>
        /// Creates an array of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <returns>An Array{T} that contains the elements from the input sequence.</returns>
        public static T[] ToDynamicArray<T>([NotNull] this IEnumerable source)
        {
            Check.NotNull(source, nameof(source));
            return CastToArray<T>(source);
        }

        /// <summary>
        /// Creates a list of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <returns>A List that contains the elements from the input sequence.</returns>
#if NET35
        public static List<object> ToDynamicList([NotNull] this IEnumerable source)
#else
        public static List<dynamic> ToDynamicList([NotNull] this IEnumerable source)
#endif
        {
            Check.NotNull(source, nameof(source));
#if NET35
            return CastToList<object>(source);
#else
            return CastToList<dynamic>(source);
#endif
        }

        /// <summary>
        /// Creates a list of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <returns>A List{T} that contains the elements from the input sequence.</returns>
        public static List<T> ToDynamicList<T>([NotNull] this IEnumerable source)
        {
            Check.NotNull(source, nameof(source));
            return CastToList<T>(source);
        }

        private static T[] CastToArray<T>(IEnumerable source)
        {
            return source.Cast<T>().ToArray();
        }

        private static List<T> CastToList<T>(IEnumerable source)
        {
            return source.Cast<T>().ToList();
        }
    }
}