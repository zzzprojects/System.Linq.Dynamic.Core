#if EFCORE
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
#else
using System.Data.Entity.Infrastructure;
#endif
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

#if EFCORE
namespace Microsoft.EntityFrameworkCore.DynamicLinq
#else
namespace EntityFramework.DynamicLinq
#endif
{
    /// <summary>
    /// Provides a set of static Async methods for querying data structures that implement <see cref="IQueryable"/>.
    /// It allows dynamic string based querying. Very handy when, at compile time, you don't know the type of queries that will be generated,
    /// or when downstream components only return column names to sort and filter by.
    /// </summary>
    public static class EntityFrameworkDynamicQueryableExtensions
    {
        private static readonly TraceSource TraceSource = new TraceSource(typeof(EntityFrameworkDynamicQueryableExtensions).Name);

        #region AllAsync
        private static readonly MethodInfo _AllPredicate = GetMethod(nameof(Queryable.All), 1);

        /// <summary>
        ///     Asynchronously determines whether all the elements of a sequence satisfy a condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that All asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to calculate the All of.
        /// </param>
        /// <param name="predicate">A projection function to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains true if every element of the source sequence passes the test in the specified predicate; otherwise, false.
        /// </returns>
        [PublicAPI]
        public static Task<bool> AllAsync([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return AllAsync(source, predicate, default(CancellationToken), args);
        }

        /// <summary>
        ///     Asynchronously determines whether all the elements of a sequence satisfy a condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that All asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to calculate the All of.
        /// </param>
        /// <param name="predicate">A projection function to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains true if every element of the source sequence passes the test in the specified predicate; otherwise, false.
        /// </returns>
        [PublicAPI]
        public static Task<bool> AllAsync([NotNull] this IQueryable source, [NotNull] string predicate, CancellationToken cancellationToken = default(CancellationToken), [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(false, source.ElementType, null, predicate, args);

            return ExecuteAsync<bool>(_AllPredicate, source, Expression.Quote(lambda), cancellationToken);
        }
#endregion AllAsync

#region AnyAsync
        private static readonly MethodInfo _any = GetMethod(nameof(Queryable.Any));

        /// <summary>
        ///     Asynchronously determines whether a sequence contains any elements.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to check for being empty.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains true if the source sequence contains any elements; otherwise, false.
        /// </returns>
        [PublicAPI]
        public static Task<bool> AnyAsync([NotNull] this IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            return ExecuteAsync<bool>(_any, source, cancellationToken);
        }

        private static readonly MethodInfo _anyPredicate = GetMethod(nameof(Queryable.Any), 1);

        /// <summary>
        ///     Asynchronously determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> whose elements to test for a condition.
        /// </param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains true if the source sequence contains any elements; otherwise, false.
        /// </returns>
        [PublicAPI]
        public static Task<bool> AnyAsync([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return AnyAsync(source, predicate, default(CancellationToken), args);
        }

        /// <summary>
        ///     Asynchronously determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> whose elements to test for a condition.
        /// </param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains true if the source sequence contains any elements; otherwise, false.
        /// </returns>
        [PublicAPI]
        public static Task<bool> AnyAsync([NotNull] this IQueryable source, [NotNull] string predicate, CancellationToken cancellationToken = default(CancellationToken), [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(false, source.ElementType, null, predicate, args);

            return ExecuteAsync<bool>(_anyPredicate, source, Expression.Quote(lambda), cancellationToken);
        }
#endregion AnyAsync

#region AverageAsync
        private static readonly MethodInfo _average = GetMethod(nameof(Queryable.Average));

        /// <summary>
        ///     Asynchronously computes the average of a sequence of values.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported. Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to calculate the average of.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the average of the sequence of values.
        /// </returns>
        [PublicAPI]
        public static Task<double> AverageAsync([NotNull] this IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            return ExecuteAsync<double>(_average, source, cancellationToken);
        }

        private static readonly MethodInfo _averagePredicate = GetMethod(nameof(Queryable.Average), 1);

        /// <summary>
        ///     Asynchronously computes the average of a sequence of values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that Average asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to calculate the average of.
        /// </param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains <c>true</c> if Average elements in the source sequence pass the test in the specified
        ///     predicate; otherwise, <c>false</c>.
        /// </returns>
        [PublicAPI]
        public static Task<double> AverageAsync([NotNull] this IQueryable source, [NotNull] string selector, [CanBeNull] params object[] args)
        {
            return AverageAsync(source, selector, default(CancellationToken), args);
        }

        /// <summary>
        ///     Asynchronously computes the average of a sequence of values that is obtained by invoking a projection function on each element of the input sequence.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that Average asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to calculate the average of.
        /// </param>
        /// <param name="selector">A projection function to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains <c>true</c> if Average elements in the source sequence pass the test in the specified
        ///     predicate; otherwise, <c>false</c>.
        /// </returns>
        [PublicAPI]
        public static Task<double> AverageAsync([NotNull] this IQueryable source, [NotNull] string selector, CancellationToken cancellationToken = default(CancellationToken), [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(selector, nameof(selector));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(false, source.ElementType, null, selector, args);

            return ExecuteAsync<double>(_averagePredicate, source, Expression.Quote(lambda), cancellationToken);
        }
#endregion AverageAsync

#region Count
        private static readonly MethodInfo _count = GetMethod(nameof(Queryable.Count));

        /// <summary>
        ///     Asynchronously returns the number of elements in a sequence.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> that contains the elements to be counted.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the number of elements in the input sequence.
        /// </returns>
        [PublicAPI]
        public static Task<int> CountAsync([NotNull] this IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            return ExecuteAsync<int>(_count, source, cancellationToken);
        }

        private static readonly MethodInfo _countPredicate = GetMethod(nameof(Queryable.Count), 1);

        /// <summary>
        ///     Asynchronously returns the number of elements in a sequence that satisfy a condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> that contains the elements to be counted.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the number of elements in the sequence that satisfy the condition in the predicate
        ///     function.
        /// </returns>
        [PublicAPI]
        public static Task<int> CountAsync([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return CountAsync(source, default(CancellationToken), predicate, args);
        }

        /// <summary>
        ///     Asynchronously returns the number of elements in a sequence that satisfy a condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> that contains the elements to be counted.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the number of elements in the sequence that satisfy the condition in the predicate
        ///     function.
        /// </returns>
        [PublicAPI]
        public static Task<int> CountAsync([NotNull] this IQueryable source, CancellationToken cancellationToken, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(false, source.ElementType, null, predicate, args);

            return ExecuteAsync<int>(_countPredicate, source, Expression.Quote(lambda), cancellationToken);
        }
#endregion Count

#region FirstAsync
        private static readonly MethodInfo _first = GetMethod(nameof(Queryable.First));

        /// <summary>
        ///     Asynchronously returns the first element of a sequence.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the first element of.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the first element in <paramref name="source" />.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> FirstAsync([NotNull] this IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<dynamic>(_first, source, cancellationToken);
        }

        private static readonly MethodInfo _firstPredicate = GetMethod(nameof(Queryable.First), 1);

        /// <summary>
        ///     Asynchronously returns the first element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the first element of.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the first element in <paramref name="source" /> that passes the test in
        ///     <paramref name="predicate" />.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> FirstAsync([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return FirstAsync(source, default(CancellationToken), predicate, args);
        }

        /// <summary>
        ///     Asynchronously returns the first element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the first element of.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the first element in <paramref name="source" /> that passes the test in
        ///     <paramref name="predicate" />.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> FirstAsync([NotNull] this IQueryable source, CancellationToken cancellationToken, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(false, source.ElementType, null, predicate, args);

            return ExecuteAsync<dynamic>(_firstPredicate, source, Expression.Quote(lambda), cancellationToken);
        }
#endregion FirstAsync

#region FirstOrDefaultAsync
        private static readonly MethodInfo _firstOrDefault = GetMethod(nameof(Queryable.FirstOrDefault));

        /// <summary>
        ///     Asynchronously returns the first element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the first element of.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains <c>default</c> if
        ///     <paramref name="source" /> is empty; otherwise, the first element in <paramref name="source" />.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> FirstOrDefaultAsync([NotNull] this IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            return ExecuteAsync<dynamic>(_firstOrDefault, source, cancellationToken);
        }

        private static readonly MethodInfo _firstOrDefaultPredicate = GetMethod(nameof(Queryable.FirstOrDefault), 1);

        /// <summary>
        ///     Asynchronously returns the first element of a sequence that satisfies a specified condition
        ///     or a default value if no such element is found.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the first element of.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains <c>default</c> if <paramref name="source" />
        ///     is empty or if no element passes the test specified by <paramref name="predicate" /> ; otherwise, the first
        ///     element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> FirstOrDefaultAsync([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return FirstOrDefaultAsync(source, default(CancellationToken), predicate, args);
        }

        /// <summary>
        ///     Asynchronously returns the first element of a sequence that satisfies a specified condition
        ///     or a default value if no such element is found.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the first element of.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains <c>default</c> if <paramref name="source" />
        ///     is empty or if no element passes the test specified by <paramref name="predicate" /> ; otherwise, the first
        ///     element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> FirstOrDefaultAsync([NotNull] this IQueryable source, CancellationToken cancellationToken, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(false, source.ElementType, null, predicate, args);

            return ExecuteAsync<dynamic>(_firstOrDefaultPredicate, source, Expression.Quote(lambda), cancellationToken);
        }
#endregion FirstOrDefault

#region LastAsync
        private static readonly MethodInfo _last = GetMethod(nameof(Queryable.Last));

        /// <summary>
        ///     Asynchronously returns the last element of a sequence. [Maybe not supported : https://msdn.microsoft.com/en-us/library/bb738550.aspx]
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the last element of.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the last element in <paramref name="source" />.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> LastAsync([NotNull] this IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));

            return ExecuteAsync<dynamic>(_last, source, cancellationToken);
        }

        private static readonly MethodInfo _lastPredicate = GetMethod(nameof(Queryable.Last), 1);

        /// <summary>
        ///     Asynchronously returns the last element of a sequence that satisfies a specified condition. [Maybe not supported : https://msdn.microsoft.com/en-us/library/bb738550.aspx]
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the last element of.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the last element in <paramref name="source" /> that passes the test in
        ///     <paramref name="predicate" />.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> LastAsync([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return LastAsync(source, default(CancellationToken), predicate, args);
        }

        /// <summary>
        ///     Asynchronously returns the last element of a sequence that satisfies a specified condition. [Maybe not supported : https://msdn.microsoft.com/en-us/library/bb738550.aspx]
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the last element of.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the last element in <paramref name="source" /> that passes the test in
        ///     <paramref name="predicate" />.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> LastAsync([NotNull] this IQueryable source, CancellationToken cancellationToken, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(false, source.ElementType, null, predicate, args);

            return ExecuteAsync<dynamic>(_lastPredicate, source, Expression.Quote(lambda), cancellationToken);
        }
#endregion LastAsync

#region LastOrDefaultAsync
        private static readonly MethodInfo _lastOrDefault = GetMethod(nameof(Queryable.LastOrDefault));

        /// <summary>
        ///     Asynchronously returns the last element of a sequence, or a default value if the sequence contains no elements. [Maybe not supported : https://msdn.microsoft.com/en-us/library/bb738550.aspx]
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the last element of.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains <c>default</c> if
        ///     <paramref name="source" /> is empty; otherwise, the last element in <paramref name="source" />.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> LastOrDefaultAsync([NotNull] this IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            return ExecuteAsync<dynamic>(_lastOrDefault, source, cancellationToken);
        }

        private static readonly MethodInfo _lastOrDefaultPredicate = GetMethod(nameof(Queryable.LastOrDefault), 1);

        /// <summary>
        ///     Asynchronously returns the last element of a sequence that satisfies a specified condition
        ///     or a default value if no such element is found. [Maybe not supported : https://msdn.microsoft.com/en-us/library/bb738550.aspx]
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the last element of.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains <c>default</c> if <paramref name="source" />
        ///     is empty or if no element passes the test specified by <paramref name="predicate" /> ; otherwise, the last
        ///     element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> LastOrDefaultAsync([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return LastOrDefaultAsync(source, default(CancellationToken), predicate, args);
        }

        /// <summary>
        ///     Asynchronously returns the last element of a sequence that satisfies a specified condition
        ///     or a default value if no such element is found. [Maybe not supported : https://msdn.microsoft.com/en-us/library/bb738550.aspx]
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the last element of.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains <c>default</c> if <paramref name="source" />
        ///     is empty or if no element passes the test specified by <paramref name="predicate" /> ; otherwise, the last
        ///     element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> LastOrDefaultAsync([NotNull] this IQueryable source, CancellationToken cancellationToken, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(false, source.ElementType, null, predicate, args);

            return ExecuteAsync<dynamic>(_lastOrDefaultPredicate, source, Expression.Quote(lambda), cancellationToken);
        }
#endregion LastOrDefault

#region LongCount
        private static readonly MethodInfo _longCount = GetMethod(nameof(Queryable.LongCount));

        /// <summary>
        ///     Asynchronously returns the number of elements in a sequence.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> that contains the elements to be counted.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the number of elements in the input sequence.
        /// </returns>
        [PublicAPI]
        public static Task<long> LongCountAsync([NotNull] this IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            return ExecuteAsync<long>(_longCount, source, cancellationToken);
        }

        private static readonly MethodInfo _longCountPredicate = GetMethod(nameof(Queryable.LongCount), 1);

        /// <summary>
        ///     Asynchronously returns the number of elements in a sequence that satisfy a condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> that contains the elements to be counted.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the number of elements in the sequence that satisfy the condition in the predicate
        ///     function.
        /// </returns>
        [PublicAPI]
        public static Task<long> LongCountAsync([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return LongCountAsync(source, default(CancellationToken), predicate, args);
        }

        /// <summary>
        ///     Asynchronously returns the number of elements in a sequence that satisfy a condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> that contains the elements to be counted.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the number of elements in the sequence that satisfy the condition in the predicate
        ///     function.
        /// </returns>
        [PublicAPI]
        public static Task<long> LongCountAsync([NotNull] this IQueryable source, CancellationToken cancellationToken, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(false, source.ElementType, null, predicate, args);

            return ExecuteAsync<long>(_longCountPredicate, source, Expression.Quote(lambda), cancellationToken);
        }
#endregion LongCount

#region SingleOrDefaultAsync
        private static readonly MethodInfo _singleOrDefault = GetMethod(nameof(Queryable.SingleOrDefault));

        /// <summary>
        ///     Asynchronously returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists.
        ///     This method throws an exception if more than one element satisfies the condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the single element of.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the single element of the input sequence that satisfies the condition in predicate.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> SingleOrDefaultAsync([NotNull] this IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            return ExecuteAsync<dynamic>(_singleOrDefault, source, cancellationToken);
        }

        private static readonly MethodInfo _singleOrDefaultPredicate = GetMethod(nameof(Queryable.SingleOrDefault), 1);

        /// <summary>
        ///     Asynchronously returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists.
        ///     This method throws an exception if more than one element satisfies the condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the single element of.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the single element of the input sequence that satisfies the condition in <paramref name="predicate" />, or default if no such element is found.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> SingleOrDefaultAsync([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return SingleOrDefaultAsync(source, default(CancellationToken), predicate, args);
        }

        /// <summary>
        ///     Asynchronously returns the only element of a sequence that satisfies a specified condition or a default value if no such element exists.
        ///     This method throws an exception if more than one element satisfies the condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to return the single element of.
        /// </param>
        /// <param name="predicate"> A function to test each element for a condition. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the single element of the input sequence that satisfies the condition in <paramref name="predicate" />, or default if no such element is found.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> SingleOrDefaultAsync([NotNull] this IQueryable source, CancellationToken cancellationToken, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(false, source.ElementType, null, predicate, args);

            return ExecuteAsync<dynamic>(_singleOrDefaultPredicate, source, Expression.Quote(lambda), cancellationToken);
        }
#endregion SingleOrDefault

#region SumAsync
        /// <summary>
        ///     Asynchronously computes the sum of a sequence of values.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> that contains the elements to be summed.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains sum of the values in the sequence.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> SumAsync([NotNull] this IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            var sum = GetMethod(nameof(Queryable.Sum), source.ElementType);

            return ExecuteDynamicAsync(sum, source, cancellationToken);
        }

        /// <summary>
        ///     Asynchronously computes the sum of a sequence of values.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> that contains the elements to be summed.
        /// </param>
        /// <param name="selector"> A projection function to apply to each element. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the number of elements in the sequence that satisfy the condition in the predicate
        ///     function.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> SumAsync([NotNull] this IQueryable source, [NotNull] string selector, [CanBeNull] params object[] args)
        {
            return SumAsync(source, default(CancellationToken), selector, args);
        }

        /// <summary>
        ///     Asynchronously computes the sum of a sequence of values.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> that contains the elements to be summed.
        /// </param>
        /// <param name="selector"> A projection function to apply to each element. </param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the sum of the projected values.
        /// </returns>
        [PublicAPI]
        public static Task<dynamic> SumAsync([NotNull] this IQueryable source, CancellationToken cancellationToken, [NotNull] string selector, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(selector, nameof(selector));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(false, source.ElementType, null, selector, args);

            var sumSelector = GetMethod(nameof(Queryable.Sum), lambda.ReturnType, 1);

            return ExecuteDynamicAsync(sumSelector, source, Expression.Quote(lambda), cancellationToken);
        }
#endregion SumAsync

#region Private Helpers
        private static readonly MethodInfo _executeAsyncMethod =
                typeof(EntityFrameworkDynamicQueryableExtensions)
#if NETSTANDARD || UAP10_0
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Single(m => m.Name == nameof(ExecuteAsync) && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(MethodInfo), typeof(IQueryable), typeof(CancellationToken) }));
#else
                .GetMethod(nameof(ExecuteAsync), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(MethodInfo), typeof(IQueryable), typeof(CancellationToken) }, null);
#endif

        private static Task<dynamic> ExecuteDynamicAsync(MethodInfo operatorMethodInfo, IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
            var executeAsyncMethod = _executeAsyncMethod.MakeGenericMethod(operatorMethodInfo.ReturnType);

            var task = (Task)executeAsyncMethod.Invoke(null, new object[] { operatorMethodInfo, source, cancellationToken });
            var castedTask = task.ContinueWith(t => (dynamic)t.GetType().GetProperty(nameof(Task<object>.Result)).GetValue(t));

            return castedTask;
        }

        private static Task<TResult> ExecuteAsync<TResult>(MethodInfo operatorMethodInfo, IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
#if EFCORE
            var provider = source.Provider as IAsyncQueryProvider;
#else
            var provider = source.Provider as IDbAsyncQueryProvider;
#endif

            if (provider != null)
            {
                if (operatorMethodInfo.IsGenericMethod)
                {
                    operatorMethodInfo = operatorMethodInfo.MakeGenericMethod(source.ElementType);
                }

#if EFCORE_3X
                source = CastSource(source, operatorMethodInfo);
                var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, source.Expression));
                return ExecuteAsync<TResult>(provider, optimized, cancellationToken);
#else
                var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, source.Expression));

                return provider.ExecuteAsync<TResult>(optimized, cancellationToken);
#endif
            }

            throw new InvalidOperationException(Res.IQueryableProviderNotAsync);
        }

        private static Task<TResult> ExecuteAsync<TResult>(MethodInfo operatorMethodInfo, IQueryable source, LambdaExpression expression, CancellationToken cancellationToken = default(CancellationToken))
            => ExecuteAsync<TResult>(operatorMethodInfo, source, Expression.Quote(expression), cancellationToken);

        private static readonly MethodInfo _executeAsyncMethodWithExpression =
                typeof(EntityFrameworkDynamicQueryableExtensions)
#if NETSTANDARD || UAP10_0
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Single(m => m.Name == nameof(ExecuteAsync) && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(MethodInfo), typeof(IQueryable), typeof(Expression), typeof(CancellationToken) }));
#else
                .GetMethod(nameof(ExecuteAsync), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(MethodInfo), typeof(IQueryable), typeof(Expression), typeof(CancellationToken) }, null);
#endif

        private static Task<dynamic> ExecuteDynamicAsync(MethodInfo operatorMethodInfo, IQueryable source, Expression expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            var executeAsyncMethod = _executeAsyncMethodWithExpression.MakeGenericMethod(operatorMethodInfo.ReturnType);

            var task = (Task)executeAsyncMethod.Invoke(null, new object[] { operatorMethodInfo, source, expression, cancellationToken });
            var castedTask = task.ContinueWith(t => (dynamic)t.GetType().GetProperty(nameof(Task<object>.Result)).GetValue(t));

            return castedTask;
        }

        private static Task<TResult> ExecuteAsync<TResult>(MethodInfo operatorMethodInfo, IQueryable source, Expression expression, CancellationToken cancellationToken = default(CancellationToken))
        {
#if EFCORE
            var provider = source.Provider as IAsyncQueryProvider;
#else
            var provider = source.Provider as IDbAsyncQueryProvider;
#endif

            if (provider != null)
            {
                operatorMethodInfo
                    = operatorMethodInfo.GetGenericArguments().Length == 2
                        ? operatorMethodInfo.MakeGenericMethod(source.ElementType, typeof(TResult))
                        : operatorMethodInfo.MakeGenericMethod(source.ElementType);
#if EFCORE_3X
                source = CastSource(source, operatorMethodInfo);
                var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, new[] { source.Expression, expression }));
                return ExecuteAsync<TResult>(provider, optimized, cancellationToken);
#else
                var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, new[] { source.Expression, expression }));

                return provider.ExecuteAsync<TResult>(optimized, cancellationToken);
#endif
            }

            throw new InvalidOperationException(Res.IQueryableProviderNotAsync);
        }

        private static MethodInfo GetMethod<TResult>(string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null) =>
            GetMethod(name, typeof(TResult), parameterCount, predicate);

        private static MethodInfo GetMethod(string name, Type returnType, int parameterCount = 0, Func<MethodInfo, bool> predicate = null) =>
            GetMethod(name, parameterCount, mi => (mi.ReturnType == returnType) && ((predicate == null) || predicate(mi)));

        private static MethodInfo GetMethod(string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null) =>
            typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name).First(mi => (mi.GetParameters().Length == parameterCount + 1) && ((predicate == null) || predicate(mi)));

#if EFCORE_3X
        private static IQueryable CastSource(IQueryable source, MethodInfo operatorMethodInfo)
        {
            if (operatorMethodInfo.GetParameters()[0].ParameterType.IsGenericType &&
                operatorMethodInfo.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
                && operatorMethodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[0] != source.Expression.Type.GetGenericArguments()[0])
            {
                source = source.Cast(operatorMethodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[0]);
            }

            return source;
        }

        private static Task<TResult> ExecuteAsync<TResult>(IAsyncQueryProvider provider, Expression expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (typeof(TResult) == typeof(object) && expression.Type != typeof(object))
            {
                var executeAsync = typeof(IAsyncQueryProvider).GetMethod("ExecuteAsync").MakeGenericMethod(typeof(Task<>).MakeGenericType(expression.Type));
                var task = (Task)executeAsync.Invoke(provider, new object[] { expression, cancellationToken });
                return (Task<TResult>)(object)task.ContinueWith(x => (dynamic)x.GetType().GetProperty(nameof(Task<object>.Result)).GetValue(x), cancellationToken);
            }
            else
            {
                return provider.ExecuteAsync<Task<TResult>>(expression, cancellationToken);
            }
        }
#endif

        private static Expression OptimizeExpression(Expression expression)
        {
            if (ExtensibilityPoint.QueryOptimizer != null)
            {
                var optimized = ExtensibilityPoint.QueryOptimizer(expression);

#if !(WINDOWS_APP45x || SILVERLIGHT)
                if (optimized != expression)
                {
                    TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Expression before : {0}", expression);
                    TraceSource.TraceEvent(TraceEventType.Verbose, 0, "Expression after  : {0}", optimized);
                }
#endif
                return optimized;
            }

            return expression;
        }
#endregion Private Helpers
    }
}
