#if EFCORE
using Microsoft.EntityFrameworkCore.Query.Internal;
#else
using System.Data.Entity.Infrastructure;
#endif
using System.Collections.Generic;
using System.Collections;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Extensions;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;

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
        ///     A task that represents the asynchronous operation.
        ///     The task result contains <c>true</c> if the source sequence contains any elements; otherwise, <c>false</c>.
        /// </returns>
        public static Task<bool> AnyAsync([NotNull] this IQueryable source, [NotNull] CancellationToken cancellationToken = default(CancellationToken))
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
        /// <param name="predicate"> A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains <c>true</c> if any elements in the source sequence pass the test in the specified
        ///     predicate; otherwise, <c>false</c>.
        /// </returns>
        public static Task<bool> AnyAsync([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] object[] args = null, [NotNull] CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return ExecuteAsync<bool>(_anyPredicate, source, Expression.Quote(lambda));
        }
        #endregion AnyAsync

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
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the first element in <paramref name="source" /> that passes the test in
        ///     <paramref name="predicate" />.
        /// </returns>
        public static Task<dynamic> FirstAsync([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] object[] args = null, [NotNull] CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

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
        public static Task<dynamic> FirstOrDefaultAsync([NotNull] this IQueryable source, [NotNull] CancellationToken cancellationToken = default(CancellationToken))
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
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains <c>default</c> if <paramref name="source" />
        ///     is empty or if no element passes the test specified by <paramref name="predicate" /> ; otherwise, the first
        ///     element in <paramref name="source" /> that passes the test specified by <paramref name="predicate" />.
        /// </returns>
        public static Task<dynamic> FirstOrDefaultAsync([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] object[] args = null, [NotNull] CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));
            Check.NotNull(cancellationToken, nameof(cancellationToken));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return ExecuteAsync<dynamic>(_firstOrDefaultPredicate, source, Expression.Quote(lambda), cancellationToken);
        }

        #endregion FirstOrDefault

        #region Private Helpers
        // Copied from https://github.com/aspnet/EntityFramework/blob/9186d0b78a3176587eeb0f557c331f635760fe92/src/Microsoft.EntityFrameworkCore/EntityFrameworkQueryableExtensions.cs
        //private static Task<dynamic> ExecuteAsync(MethodInfo operatorMethodInfo, IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    var provider = source.Provider as IAsyncQueryProvider;

        //    if (provider != null)
        //    {
        //        if (operatorMethodInfo.IsGenericMethod)
        //        {
        //            operatorMethodInfo = operatorMethodInfo.MakeGenericMethod(source.ElementType);
        //        }

        //        return provider.ExecuteAsync<dynamic>(
        //            Expression.Call(null, operatorMethodInfo, source.Expression),
        //            cancellationToken);
        //    }

        //    throw new InvalidOperationException(Res.IQueryableProviderNotAsync);
        //}

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

                return provider.ExecuteAsync<TResult>(
                    Expression.Call(null, operatorMethodInfo, source.Expression),
                    cancellationToken);
            }

            throw new InvalidOperationException(Res.IQueryableProviderNotAsync);
        }

        private static Task<TResult> ExecuteAsync<TResult>(MethodInfo operatorMethodInfo, IQueryable source, LambdaExpression expression, CancellationToken cancellationToken = default(CancellationToken))
            => ExecuteAsync<TResult>(operatorMethodInfo, source, Expression.Quote(expression), cancellationToken);

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

                return provider.ExecuteAsync<TResult>(
                    Expression.Call(
                        null,
                        operatorMethodInfo,
                        new[] { source.Expression, expression }),
                    cancellationToken);
            }

            throw new InvalidOperationException(Res.IQueryableProviderNotAsync);
        }

        private static MethodInfo GetMethod<TResult>(string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null) =>
            GetMethod(name, parameterCount, mi => (mi.ReturnType == typeof(TResult)) && ((predicate == null) || predicate(mi)));

        private static MethodInfo GetMethod(string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null) =>
            typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name).Single(mi => (mi.GetParameters().Length == parameterCount + 1) && ((predicate == null) || predicate(mi)));
        #endregion Private Helpers
    }
}