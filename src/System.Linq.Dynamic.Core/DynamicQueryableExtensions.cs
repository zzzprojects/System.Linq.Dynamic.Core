using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Linq.Dynamic.Core.Exceptions;
#if !(WINDOWS_APP45x || SILVERLIGHT)
using System.Diagnostics;
#endif
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using System.Linq.Dynamic.Core.Parser;

#if WINDOWS_APP
using System;
using System.Linq;
#endif

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Provides a set of static (Shared in Visual Basic) methods for querying data structures that implement <see cref="IQueryable"/>.
    /// It allows dynamic string based querying. Very handy when, at compile time, you don't know the type of queries that will be generated,
    /// or when downstream components only return column names to sort and filter by.
    /// </summary>
    public static class DynamicQueryableExtensions
    {
#if !(WINDOWS_APP45x || SILVERLIGHT)
        private static readonly TraceSource TraceSource = new TraceSource(nameof(DynamicQueryableExtensions));
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

        #region Aggregate
        /// <summary>
        /// Dynamically runs an aggregate function on the IQueryable.
        /// </summary>
        /// <param name="source">The IQueryable data source.</param>
        /// <param name="function">The name of the function to run. Can be Sum, Average, Min or Max.</param>
        /// <param name="member">The name of the property to aggregate over.</param>
        /// <returns>The value of the aggregate function run over the specified property.</returns>
        public static object Aggregate([NotNull] this IQueryable source, [NotNull] string function, [NotNull] string member)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(function, nameof(function));
            Check.NotEmpty(member, nameof(member));

            // Properties
            PropertyInfo property = source.ElementType.GetProperty(member);
            ParameterExpression parameter = ParameterExpressionHelper.CreateParameterExpression(source.ElementType, "s");
            Expression selector = Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter);
            // We've tried to find an expression of the type Expression<Func<TSource, TAcc>>,
            // which is expressed as ( (TSource s) => s.Price );

            var methods = typeof(Queryable).GetMethods().Where(x => x.Name == function && x.IsGenericMethod);

            // Method
            MethodInfo aggregateMethod = methods.SingleOrDefault(m =>
            {
                ParameterInfo lastParameter = m.GetParameters().LastOrDefault();

                return lastParameter != null && TypeHelper.GetUnderlyingType(lastParameter.ParameterType) == property.PropertyType;
            });

            // Sum, Average
            if (aggregateMethod != null)
            {
                return source.Provider.Execute(
                    Expression.Call(
                        null,
                        aggregateMethod.MakeGenericMethod(source.ElementType),
                        new[] { source.Expression, Expression.Quote(selector) }));
            }

            // Min, Max
            aggregateMethod = methods.SingleOrDefault(m => m.Name == function && m.GetGenericArguments().Length == 2);

            return source.Provider.Execute(
                Expression.Call(
                    null,
                    aggregateMethod.MakeGenericMethod(source.ElementType, property.PropertyType),
                    new[] { source.Expression, Expression.Quote(selector) }));
        }
        #endregion Aggregate

        #region All
        private static readonly MethodInfo _AllPredicate = GetMethod(nameof(Queryable.All), 1);

        /// <summary>
        ///     Determines whether all the elements of a sequence satisfy a condition.
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
        ///     true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.
        /// </returns>
        [PublicAPI]
        public static bool All([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return All(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        ///     Determines whether all the elements of a sequence satisfy a condition.
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that All asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="source">
        ///     An <see cref="IQueryable" /> to calculate the All of.
        /// </param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A projection function to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        ///     true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.
        /// </returns>
        [PublicAPI]
        public static bool All([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return Execute<bool>(_AllPredicate, source, Expression.Quote(lambda));
        }
        #endregion AllAsync

        #region Any
        private static readonly MethodInfo _any = GetMethod(nameof(Queryable.Any));

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result = queryable.Any();
        /// </code>
        /// </example>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        public static bool Any([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            return Execute<bool>(_any, source);
        }

        private static readonly MethodInfo _anyPredicate = GetMethod(nameof(Queryable.Any), 1);

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.Any("Income > 50");
        /// var result2 = queryable.Any("Income > @0", 50);
        /// var result3 = queryable.Select("Roles.Any()");
        /// </code>
        /// </example>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        [PublicAPI]
        public static bool Any([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute<bool>(_anyPredicate, source, lambda);
        }

        /// <inheritdoc cref="Any(IQueryable, ParsingConfig, string, object[])"/>
        public static bool Any([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
        {
            return Any(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
        public static bool Any([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(lambda, nameof(lambda));

            return Execute<bool>(_anyPredicate, source, lambda);
        }
        #endregion Any

        #region Average
        /// <summary>
        /// Computes the average of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the average of.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.Average();
        /// var result2 = queryable.Select("Roles.Average()");
        /// </code>
        /// </example>
        /// <returns>The average of the values in the sequence.</returns>
        [PublicAPI]
        public static double Average([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            var average = GetMethod(nameof(Queryable.Average), source.ElementType, typeof(double));
            return Execute<double>(average, source);
        }

        /// <summary>
        /// Computes the average of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the average of.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result = queryable.Average("Income");
        /// </code>
        /// </example>
        /// <returns>The average of the values in the sequence.</returns>
        [PublicAPI]
        public static double Average([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Average(source, lambda);
        }

        /// <inheritdoc cref="Average(IQueryable, ParsingConfig, string, object[])"/>
        [PublicAPI]
        public static double Average([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return Average(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Computes the average of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the average of.</param>
        /// <param name="lambda">A Lambda Expression.</param>
        /// <returns>The average of the values in the sequence.</returns>
        [PublicAPI]
        public static double Average([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(lambda, nameof(lambda));

            var averageSelector = GetMethod(nameof(Queryable.Average), lambda.GetReturnType(), typeof(double), 1);
            return Execute<double>(averageSelector, source, lambda);
        }
        #endregion Average

        #region AsEnumerable
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
        #endregion AsEnumerable

        #region Cast
        private static readonly MethodInfo _cast = GetGenericMethod(nameof(Queryable.Cast));

        /// <summary>
        /// Converts the elements of an <see cref="IQueryable"/> to the specified type.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> that contains the elements to be converted.</param>
        /// <param name="type">The type to convert the elements of source to.</param>
        /// <returns>An <see cref="IQueryable"/> that contains each element of the source sequence converted to the specified type.</returns>
        public static IQueryable Cast([NotNull] this IQueryable source, [NotNull] Type type)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(type, nameof(type));

            var optimized = OptimizeExpression(Expression.Call(null, _cast.MakeGenericMethod(new Type[] { type }), new Expression[] { source.Expression }));

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Converts the elements of an <see cref="IQueryable"/> to the specified type.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> that contains the elements to be converted.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="typeName">The type to convert the elements of source to.</param>
        /// <returns>An <see cref="IQueryable"/> that contains each element of the source sequence converted to the specified type.</returns>
        public static IQueryable Cast([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string typeName)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(typeName, nameof(typeName));

            var finder = new TypeFinder(config, new KeywordsHelper(config));
            Type type = finder.FindTypeByName(typeName, null, true);

            return Cast(source, type);
        }

        /// <summary>
        /// Converts the elements of an <see cref="IQueryable"/> to the specified type.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> that contains the elements to be converted.</param>
        /// <param name="typeName">The type to convert the elements of source to.</param>
        /// <returns>An <see cref="IQueryable"/> that contains each element of the source sequence converted to the specified type.</returns>
        public static IQueryable Cast([NotNull] this IQueryable source, [NotNull] string typeName)
        {
            return Cast(source, ParsingConfig.Default, typeName);
        }
        #endregion Cast

        #region Count
        private static readonly MethodInfo _count = GetMethod(nameof(Queryable.Count));

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> that contains the elements to be counted.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result = queryable.Count();
        /// </code>
        /// </example>
        /// <returns>The number of elements in the input sequence.</returns>
        public static int Count([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            return Execute<int>(_count, source);
        }

        private static readonly MethodInfo _countPredicate = GetMethod(nameof(Queryable.Count), 1);

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> that contains the elements to be counted.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.Count("Income > 50");
        /// var result2 = queryable.Count("Income > @0", 50);
        /// var result3 = queryable.Select("Roles.Count()");
        /// </code>
        /// </example>
        /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
        [PublicAPI]
        public static int Count([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute<int>(_countPredicate, source, lambda);
        }

        /// <inheritdoc cref="Count(IQueryable, ParsingConfig, string, object[])"/>
        public static int Count([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
        {
            return Count(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> that contains the elements to be counted.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
        public static int Count([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(lambda, nameof(lambda));

            return Execute<int>(_countPredicate, source, lambda);
        }
        #endregion Count

        #region DefaultIfEmpty
        private static readonly MethodInfo _defaultIfEmpty = GetMethod(nameof(Queryable.DefaultIfEmpty));
        private static readonly MethodInfo _defaultIfEmptyWithParam = GetMethod(nameof(Queryable.DefaultIfEmpty), 1);

        /// <summary>
        /// Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return a default value for if empty.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.DefaultIfEmpty();
        /// </code>
        /// </example>
        /// <returns>An <see cref="IQueryable"/> that contains default if source is empty; otherwise, source.</returns>
        public static IQueryable DefaultIfEmpty([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            return CreateQuery(_defaultIfEmpty, source);
        }

        /// <summary>
        /// Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return a default value for if empty.</param>
        /// <param name="defaultValue">The value to return if the sequence is empty.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.DefaultIfEmpty(new Employee());
        /// </code>
        /// </example>
        /// <returns>An <see cref="IQueryable"/> that contains defaultValue if source is empty; otherwise, source.</returns>
        public static IQueryable DefaultIfEmpty([NotNull] this IQueryable source, [CanBeNull] object defaultValue)
        {
            Check.NotNull(source, nameof(source));

            return CreateQuery(_defaultIfEmptyWithParam, source, Expression.Constant(defaultValue));
        }
        #endregion

        #region Distinct
        private static readonly MethodInfo _distinct = GetMethod(nameof(Queryable.Distinct));

        /// <summary>
        /// Returns distinct elements from a sequence by using the default equality comparer to compare values.
        /// </summary>
        /// <param name="source">The sequence to remove duplicate elements from.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.Distinct();
        /// var result2 = queryable.Select("Roles.Distinct()");
        /// </code>
        /// </example>
        /// <returns>An <see cref="IQueryable"/> that contains distinct elements from the source sequence.</returns>
        public static IQueryable Distinct([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            return CreateQuery(_distinct, source);
        }
        #endregion Distinct

        #region First
        private static readonly MethodInfo _first = GetMethod(nameof(Queryable.First));

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

            return Execute(_first, source);
        }

        private static readonly MethodInfo _firstPredicate = GetMethod(nameof(Queryable.First), 1);

        /// <summary>
        /// Returns the first element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the first element of.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
        [PublicAPI]
#if NET35
        public static object First([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
#else
        public static dynamic First([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_firstPredicate, source, lambda);
        }

        /// <inheritdoc cref="First(IQueryable, ParsingConfig, string, object[])"/>
#if NET35
        public static object First([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#else
        public static dynamic First([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#endif
        {
            return First(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the first element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the first element of.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object First([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
#else
        public static dynamic First([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
#endif
        {
            Check.NotNull(source, nameof(source));
            return Execute(_firstPredicate, source, lambda);
        }
        #endregion First

        #region FirstOrDefault
        private static readonly MethodInfo _firstOrDefault = GetMethod(nameof(Queryable.FirstOrDefault));

        /// <summary>
        /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the first element of.</param>
        /// <returns>default if source is empty; otherwise, the first element in source.</returns>
#if NET35
        public static object FirstOrDefault([NotNull] this IQueryable source)
#else
        public static dynamic FirstOrDefault([NotNull] this IQueryable source)
#endif
        {
            Check.NotNull(source, nameof(source));

            return Execute(_firstOrDefault, source);
        }

        /// <summary>
        /// Returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the first element of.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>default if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate.</returns>
        [PublicAPI]
#if NET35
        public static object FirstOrDefault([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
#else
        public static dynamic FirstOrDefault([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_firstOrDefaultPredicate, source, lambda);
        }

        /// <inheritdoc cref="FirstOrDefault(IQueryable, ParsingConfig, string, object[])"/>
#if NET35
        public static object FirstOrDefault([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#else
        public static dynamic FirstOrDefault([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#endif
        {
            return FirstOrDefault(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the first element of.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>default if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate.</returns>
#if NET35
        public static object FirstOrDefault([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
#else
        public static dynamic FirstOrDefault([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
#endif
        {
            Check.NotNull(source, nameof(source));

            return Execute(_firstOrDefaultPredicate, source, lambda);
        }
        private static readonly MethodInfo _firstOrDefaultPredicate = GetMethod(nameof(Queryable.FirstOrDefault), 1);
        #endregion FirstOrDefault

        #region GroupBy
        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function 
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> whose elements to group.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <param name="resultSelector">A string expression to specify a result value from each group.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> where each element represents a projection over a group and its key.</returns>
        /// <example>
        /// <code>
        /// var groupResult1 = queryable.GroupBy("NumberPropertyAsKey", "StringProperty");
        /// var groupResult2 = queryable.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)", "new (StringProperty1, StringProperty2)");
        /// </code>
        /// </example>
        [PublicAPI]
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string keySelector, [NotNull] string resultSelector, object[] args)
        {
            return InternalGroupBy(source, config, keySelector, resultSelector, null, args);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function 
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> whose elements to group.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <param name="resultSelector">A string expression to specify a result value from each group.</param>
        /// <param name="equalityComparer">The comparer to use.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> where each element represents a projection over a group and its key.</returns>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string keySelector, [NotNull] string resultSelector, IEqualityComparer equalityComparer, object[] args)
        {
            return InternalGroupBy(source, config, keySelector, resultSelector, equalityComparer, args);
        }

        internal static IQueryable InternalGroupBy([NotNull] IQueryable source, [NotNull] ParsingConfig config, [NotNull] string keySelector, [NotNull] string resultSelector, IEqualityComparer equalityComparer, object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(keySelector, nameof(keySelector));
            Check.NotEmpty(resultSelector, nameof(resultSelector));

            bool createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            LambdaExpression keyLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, keySelector, args);
            LambdaExpression elementLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, resultSelector, args);

            Expression optimized = null;
            if (equalityComparer == null)
            {
                optimized = OptimizeExpression(Expression.Call(
                    typeof(Queryable), nameof(Queryable.GroupBy),
                    new[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
                    source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)));
            }
            else
            {
                var equalityComparerGenericType = typeof(IEqualityComparer<>).MakeGenericType(keyLambda.Body.Type);
                optimized = OptimizeExpression(Expression.Call(
                    typeof(Queryable), nameof(Queryable.GroupBy),
                    new[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
                    source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda),
                    Expression.Constant(equalityComparer, equalityComparerGenericType)));
            }

            return source.Provider.CreateQuery(optimized);
        }

        /// <inheritdoc cref="GroupBy(IQueryable, ParsingConfig, string, string, object[])"/>
        [PublicAPI]
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] string keySelector, [NotNull] string resultSelector, object[] args)
        {
            return GroupBy(source, ParsingConfig.Default, keySelector, resultSelector, args);
        }

        /// <inheritdoc cref="GroupBy(IQueryable, ParsingConfig, string, string, IEqualityComparer, object[])"/>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] string keySelector, [NotNull] string resultSelector, IEqualityComparer equalityComparer, object[] args)
        {
            return GroupBy(source, ParsingConfig.Default, keySelector, resultSelector, equalityComparer, args);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function 
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> whose elements to group.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <param name="resultSelector">A string expression to specify a result value from each group.</param>
        /// <returns>A <see cref="IQueryable"/> where each element represents a projection over a group and its key.</returns>
        /// <example>
        /// <code>
        /// var groupResult1 = queryable.GroupBy("NumberPropertyAsKey", "StringProperty");
        /// var groupResult2 = queryable.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)", "new (StringProperty1, StringProperty2)");
        /// </code>
        /// </example>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string keySelector, [NotNull] string resultSelector)
        {
            return GroupBy(source, config, keySelector, resultSelector, null, null);
        }

        /// <inheritdoc cref="GroupBy(IQueryable, ParsingConfig, string, string)"/>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] string keySelector, [NotNull] string resultSelector)
        {
            return GroupBy(source, ParsingConfig.Default, keySelector, resultSelector);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function 
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> whose elements to group.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <param name="resultSelector">A string expression to specify a result value from each group.</param>
        /// <param name="equalityComparer">The comparer to use.</param>
        /// <returns>A <see cref="IQueryable"/> where each element represents a projection over a group and its key.</returns>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string keySelector, [NotNull] string resultSelector, IEqualityComparer equalityComparer)
        {
            return InternalGroupBy(source, config, keySelector, resultSelector, equalityComparer, null);
        }

        /// <inheritdoc cref="GroupBy(IQueryable, ParsingConfig, string, string, IEqualityComparer)"/>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] string keySelector, [NotNull] string resultSelector, IEqualityComparer equalityComparer)
        {
            return GroupBy(source, ParsingConfig.Default, keySelector, resultSelector, equalityComparer);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function 
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> whose elements to group.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> where each element represents a projection over a group and its key.</returns>
        /// <example>
        /// <code>
        /// var groupResult1 = queryable.GroupBy("NumberPropertyAsKey");
        /// var groupResult2 = queryable.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)");
        /// </code>
        /// </example>
        [PublicAPI]
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string keySelector, [CanBeNull] params object[] args)
        {
            return InternalGroupBy(source, config, keySelector, null, args);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function 
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> whose elements to group.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="equalityComparer">The comparer to use.</param>
        /// <returns>A <see cref="IQueryable"/> where each element represents a projection over a group and its key.</returns>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string keySelector, IEqualityComparer equalityComparer, [CanBeNull] params object[] args)
        {
            return InternalGroupBy(source, config, keySelector, equalityComparer, args);
        }

        internal static IQueryable InternalGroupBy([NotNull] IQueryable source, [NotNull] ParsingConfig config, [NotNull] string keySelector, IEqualityComparer equalityComparer, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(keySelector, nameof(keySelector));

            bool createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            LambdaExpression keyLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, keySelector, args);

            Expression optimized = null;

            if (equalityComparer == null)
            {
                optimized = OptimizeExpression(Expression.Call(
                    typeof(Queryable), nameof(Queryable.GroupBy),
                    new[] { source.ElementType, keyLambda.Body.Type }, source.Expression, Expression.Quote(keyLambda)));
            }
            else
            {
                var equalityComparerGenericType = typeof(IEqualityComparer<>).MakeGenericType(keyLambda.Body.Type);
                optimized = OptimizeExpression(Expression.Call(
                    typeof(Queryable), nameof(Queryable.GroupBy),
                    new[] { source.ElementType, keyLambda.Body.Type }, source.Expression, Expression.Quote(keyLambda),
                    Expression.Constant(equalityComparer, equalityComparerGenericType)));
            }

            return source.Provider.CreateQuery(optimized);
        }

        /// <inheritdoc cref="GroupBy(IQueryable, ParsingConfig, string, object[])"/>
        [PublicAPI]
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] string keySelector, [CanBeNull] params object[] args)
        {
            return GroupBy(source, ParsingConfig.Default, keySelector, args);
        }

        /// <inheritdoc cref="GroupBy(IQueryable, ParsingConfig, string, IEqualityComparer, object[])"/>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] string keySelector, IEqualityComparer equalityComparer, [CanBeNull] params object[] args)
        {
            return GroupBy(source, ParsingConfig.Default, keySelector, equalityComparer, args);
        }

        #endregion GroupBy

        #region GroupByMany
        /// <summary>
        /// Groups the elements of a sequence according to multiple specified key string functions 
        /// and creates a result value from each group (and subgroups) and its key.
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source">A <see cref="IEnumerable{T}"/> whose elements to group.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="keySelectors"><see cref="string"/> expressions to specify the keys for each element.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of type <see cref="GroupResult"/> where each element represents a projection over a group, its key, and its subgroups.</returns>
        public static IEnumerable<GroupResult> GroupByMany<TElement>([NotNull] this IEnumerable<TElement> source, [NotNull] ParsingConfig config, params string[] keySelectors)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.HasNoNulls(keySelectors, nameof(keySelectors));

            var selectors = new List<Func<TElement, object>>(keySelectors.Length);

            bool createParameterCtor = true;
            foreach (var selector in keySelectors)
            {
                LambdaExpression l = DynamicExpressionParser.ParseLambda(config, createParameterCtor, typeof(TElement), typeof(object), selector);
                selectors.Add((Func<TElement, object>)l.Compile());
            }

            return GroupByManyInternal(source, selectors.ToArray(), 0);
        }

        /// <inheritdoc cref="GroupByMany{TElement}(IEnumerable{TElement}, ParsingConfig, string[])"/>
        public static IEnumerable<GroupResult> GroupByMany<TElement>([NotNull] this IEnumerable<TElement> source, params string[] keySelectors)
        {
            return GroupByMany(source, ParsingConfig.Default, keySelectors);
        }

        /// <summary>
        /// Groups the elements of a sequence according to multiple specified key functions 
        /// and creates a result value from each group (and subgroups) and its key.
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source">A <see cref="IEnumerable{T}"/> whose elements to group.</param>
        /// <param name="keySelectors">Lambda expressions to specify the keys for each element.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of type <see cref="GroupResult"/> where each element represents a projection over a group, its key, and its subgroups.</returns>
        public static IEnumerable<GroupResult> GroupByMany<TElement>([NotNull] this IEnumerable<TElement> source, params Func<TElement, object>[] keySelectors)
        {
            Check.NotNull(source, nameof(source));
            Check.HasNoNulls(keySelectors, nameof(keySelectors));

            return GroupByManyInternal(source, keySelectors, 0);
        }

        private static IEnumerable<GroupResult> GroupByManyInternal<TElement>(IEnumerable<TElement> source, Func<TElement, object>[] keySelectors, int currentSelector)
        {
            if (currentSelector >= keySelectors.Length)
            {
                return null;
            }

            var selector = keySelectors[currentSelector];

            var result = source.GroupBy(selector).Select(
                g => new GroupResult
                {
                    Key = g.Key,
                    Count = g.Count(),
                    Items = g,
                    Subgroups = GroupByManyInternal(g, keySelectors, currentSelector + 1)
                });

            return result;
        }
        #endregion GroupByMany

        #region GroupJoin
        /// <summary>
        /// Correlates the elements of two sequences based on equality of keys and groups the results. The default equality comparer is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A dynamic function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A dynamic function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A dynamic function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicates as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> obtained by performing a grouped join on two sequences.</returns>
        public static IQueryable GroupJoin([NotNull] this IQueryable outer, [NotNull] ParsingConfig config, [NotNull] IEnumerable inner, [NotNull] string outerKeySelector, [NotNull] string innerKeySelector, [NotNull] string resultSelector, params object[] args)
        {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(config, nameof(config));
            Check.NotNull(inner, nameof(inner));
            Check.NotEmpty(outerKeySelector, nameof(outerKeySelector));
            Check.NotEmpty(innerKeySelector, nameof(innerKeySelector));
            Check.NotEmpty(resultSelector, nameof(resultSelector));

            Type outerType = outer.ElementType;
            Type innerType = inner.AsQueryable().ElementType;

            bool createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, outer);
            LambdaExpression outerSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, outerType, null, outerKeySelector, args);
            LambdaExpression innerSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, innerType, null, innerKeySelector, args);

            CheckOuterAndInnerTypes(config, createParameterCtor, outerType, innerType, outerKeySelector, innerKeySelector, ref outerSelectorLambda, ref innerSelectorLambda, args);

            ParameterExpression[] parameters =
            {
                ParameterExpressionHelper.CreateParameterExpression(outerType, "outer"),
                ParameterExpressionHelper.CreateParameterExpression(typeof(IEnumerable<>).MakeGenericType(innerType), "inner")
            };

            LambdaExpression resultSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, parameters, null, resultSelector, args);

            return outer.Provider.CreateQuery(Expression.Call(
                typeof(Queryable), nameof(Queryable.GroupJoin),
                new[] { outer.ElementType, innerType, outerSelectorLambda.Body.Type, resultSelectorLambda.Body.Type },
                outer.Expression,
                inner.AsQueryable().Expression,
                Expression.Quote(outerSelectorLambda),
                Expression.Quote(innerSelectorLambda),
                Expression.Quote(resultSelectorLambda)));
        }

        /// <inheritdoc cref="GroupJoin(IQueryable, ParsingConfig, IEnumerable, string, string, string, object[])"/>
        public static IQueryable GroupJoin([NotNull] this IQueryable outer, [NotNull] IEnumerable inner, [NotNull] string outerKeySelector, [NotNull] string innerKeySelector, [NotNull] string resultSelector, params object[] args)
        {
            return GroupJoin(outer, ParsingConfig.Default, inner, outerKeySelector, innerKeySelector, resultSelector, args);
        }
        #endregion

        #region Join
        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A dynamic function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A dynamic function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A dynamic function to create a result element from two matching elements.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicates as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> obtained by performing an inner join on two sequences.</returns>
        public static IQueryable Join([NotNull] this IQueryable outer, [NotNull] ParsingConfig config, [NotNull] IEnumerable inner, [NotNull] string outerKeySelector, [NotNull] string innerKeySelector, [NotNull] string resultSelector, params object[] args)
        {
            //http://stackoverflow.com/questions/389094/how-to-create-a-dynamic-linq-join-extension-method

            Check.NotNull(outer, nameof(outer));
            Check.NotNull(config, nameof(config));
            Check.NotNull(inner, nameof(inner));
            Check.NotEmpty(outerKeySelector, nameof(outerKeySelector));
            Check.NotEmpty(innerKeySelector, nameof(innerKeySelector));
            Check.NotEmpty(resultSelector, nameof(resultSelector));

            Type outerType = outer.ElementType;
            Type innerType = inner.AsQueryable().ElementType;

            bool createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, outer);
            LambdaExpression outerSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, outerType, null, outerKeySelector, args);
            LambdaExpression innerSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, innerType, null, innerKeySelector, args);

            CheckOuterAndInnerTypes(config, createParameterCtor, outerType, innerType, outerKeySelector, innerKeySelector, ref outerSelectorLambda, ref innerSelectorLambda, args);

            ParameterExpression[] parameters =
            {
                ParameterExpressionHelper.CreateParameterExpression(outerType, "outer"),
                ParameterExpressionHelper.CreateParameterExpression(innerType, "inner")
            };

            LambdaExpression resultSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, parameters, null, resultSelector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), "Join",
                new[] { outerType, innerType, outerSelectorLambda.Body.Type, resultSelectorLambda.Body.Type },
                outer.Expression, // outer: The first sequence to join.
                inner.AsQueryable().Expression, // inner: The sequence to join to the first sequence.
                Expression.Quote(outerSelectorLambda), // outerKeySelector: A function to extract the join key from each element of the first sequence.
                Expression.Quote(innerSelectorLambda), // innerKeySelector: A function to extract the join key from each element of the second sequence.
                Expression.Quote(resultSelectorLambda) // resultSelector: A function to create a result element from two matching elements.
            ));

            return outer.Provider.CreateQuery(optimized);
        }

        /// <inheritdoc cref="Join(IQueryable, ParsingConfig, IEnumerable, string, string, string, object[])"/>
        public static IQueryable Join([NotNull] this IQueryable outer, [NotNull] IEnumerable inner, [NotNull] string outerKeySelector, [NotNull] string innerKeySelector, [NotNull] string resultSelector, params object[] args)
        {
            return Join(outer, ParsingConfig.Default, inner, outerKeySelector, innerKeySelector, resultSelector, args);
        }

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TElement">The type of the elements of both sequences, and the result.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A dynamic function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A dynamic function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A dynamic function to create a result element from two matching elements.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicates as parameters.  Similar to the way String.Format formats strings.</param>
        /// <remarks>This overload only works on elements where both sequences and the resulting element match.</remarks>
        /// <returns>An <see cref="IQueryable{T}"/> that has elements of type TResult obtained by performing an inner join on two sequences.</returns>
        public static IQueryable<TElement> Join<TElement>([NotNull] this IQueryable<TElement> outer, [NotNull] ParsingConfig config, [NotNull] IEnumerable<TElement> inner, [NotNull] string outerKeySelector, [NotNull] string innerKeySelector, string resultSelector, params object[] args)
        {
            return (IQueryable<TElement>)Join(outer, config, (IEnumerable)inner, outerKeySelector, innerKeySelector, resultSelector, args);
        }

        /// <inheritdoc cref="Join{TElement}(IQueryable{TElement}, ParsingConfig, IEnumerable{TElement}, string, string, string, object[])"/>
        public static IQueryable<TElement> Join<TElement>([NotNull] this IQueryable<TElement> outer, [NotNull] IEnumerable<TElement> inner, [NotNull] string outerKeySelector, [NotNull] string innerKeySelector, string resultSelector, params object[] args)
        {
            return Join(outer, ParsingConfig.Default, inner, outerKeySelector, innerKeySelector, resultSelector, args);
        }
        #endregion Join

        #region Last
        private static readonly MethodInfo _last = GetMethod(nameof(Queryable.Last));
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

            return Execute(_last, source);
        }

        private static readonly MethodInfo _lastPredicate = GetMethod(nameof(Queryable.Last), 1);

        /// <summary>
        /// Returns the last element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object Last([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
#else
        public static dynamic Last([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_lastPredicate, source, lambda);
        }

        /// <inheritdoc cref="Last(IQueryable, ParsingConfig, string, object[])"/>
#if NET35
        public static object Last([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#else
        public static dynamic Last([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#endif
        {
            return Last(source, ParsingConfig.Default, predicate, args);
        }


        /// <summary>
        /// Returns the last element of a sequence that satisfies a specified condition.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object Last([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
#else
        public static dynamic Last([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
#endif
        {
            Check.NotNull(source, nameof(source));
            return Execute(_lastPredicate, source, lambda);
        }
        #endregion Last

        #region LastOrDefault
        private static readonly MethodInfo _lastDefault = GetMethod(nameof(Queryable.LastOrDefault));
        /// <summary>
        /// Returns the last element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <returns>default if source is empty; otherwise, the last element in source.</returns>
#if NET35
        public static object LastOrDefault([NotNull] this IQueryable source)
#else
        public static dynamic LastOrDefault([NotNull] this IQueryable source)
#endif
        {
            Check.NotNull(source, nameof(source));

            return Execute(_lastDefault, source);
        }

        private static readonly MethodInfo _lastDefaultPredicate = GetMethod(nameof(Queryable.LastOrDefault), 1);

        /// <summary>
        /// Returns the last element of a sequence that satisfies a specified condition, or a default value if the sequence contains no elements.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object LastOrDefault([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
#else
        public static dynamic LastOrDefault([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_lastDefaultPredicate, source, lambda);
        }

        /// <inheritdoc cref="LastOrDefault(IQueryable, ParsingConfig, string, object[])"/>
#if NET35
        public static object LastOrDefault([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#else
        public static dynamic LastOrDefault([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#endif
        {
            return LastOrDefault(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the last element of a sequence that satisfies a specified condition, or a default value if the sequence contains no elements.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object LastOrDefault([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
#else
        public static dynamic LastOrDefault([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
#endif
        {
            Check.NotNull(source, nameof(source));
            return Execute(_lastDefaultPredicate, source, lambda);
        }
        #endregion LastOrDefault

        #region LongCount
        private static readonly MethodInfo _longCount = GetMethod(nameof(Queryable.LongCount));

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> that contains the elements to be counted.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result = queryable.LongCount();
        /// </code>
        /// </example>
        /// <returns>The number of elements in the input sequence.</returns>
        public static long LongCount([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            return Execute<long>(_longCount, source);
        }

        private static readonly MethodInfo _longCountPredicate = GetMethod(nameof(Queryable.LongCount), 1);

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> that contains the elements to be counted.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.LongCount("Income > 50");
        /// var result2 = queryable.LongCount("Income > @0", 50);
        /// var result3 = queryable.Select("Roles.LongCount()");
        /// </code>
        /// </example>
        /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
        [PublicAPI]
        public static long LongCount([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute<long>(_longCountPredicate, source, lambda);
        }

        /// <inheritdoc cref="LongCount(IQueryable, ParsingConfig, string, object[])"/>
        public static long LongCount([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
        {
            return LongCount(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> that contains the elements to be counted.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
        public static long LongCount([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(lambda, nameof(lambda));

            return Execute<long>(_longCountPredicate, source, lambda);
        }
        #endregion LongCount

        #region Max
        private static readonly MethodInfo _max = GetMethod(nameof(Queryable.Max));

        /// <summary>
        /// Computes the max element of a sequence.
        /// </summary>
        /// <param name="source">A sequence of values to calculate find the max for.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.Max();
        /// var result2 = queryable.Select("Roles.Max()");
        /// </code>
        /// </example>
        /// <returns>The max element in the sequence.</returns>
        [PublicAPI]
        public static object Max([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            return Execute(_max, source);
        }

        private static readonly MethodInfo _maxPredicate = GetMethod(nameof(Queryable.Max), 1);

        /// <summary>
        /// Computes the max element of a sequence.
        /// </summary>
        /// <param name="source">A sequence of values to calculate find the max for.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result = queryable.Max("Income");
        /// </code>
        /// </example>
        /// <returns>The max element in the sequence.</returns>
        [PublicAPI]
        public static object Max([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, typeof(object), predicate, args);

            return Execute(_maxPredicate, source, lambda);
        }

        /// <inheritdoc cref="Max(IQueryable, ParsingConfig, string, object[])"/>
        [PublicAPI]
        public static object Max([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return Max(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Computes the max element of a sequence.
        /// </summary>
        /// <param name="source">A sequence of values to calculate find the max for.</param>
        /// <param name="lambda">A Lambda Expression.</param>
        /// <returns>The max element in the sequence.</returns>
        [PublicAPI]
        public static object Max([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
        {
            Check.NotNull(source, nameof(source));
            return Execute(_maxPredicate, source, lambda);
        }
        #endregion Max

        #region Min
        private static readonly MethodInfo _min = GetMethod(nameof(Queryable.Min));

        /// <summary>
        /// Computes the min element of a sequence.
        /// </summary>
        /// <param name="source">A sequence of values to calculate find the min for.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.Min();
        /// var result2 = queryable.Select("Roles.Min()");
        /// </code>
        /// </example>
        /// <returns>The min element in the sequence.</returns>
        [PublicAPI]
        public static object Min([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            return Execute(_min, source);
        }

        private static readonly MethodInfo _minPredicate = GetMethod(nameof(Queryable.Min), 1);

        /// <summary>
        /// Computes the min element of a sequence.
        /// </summary>
        /// <param name="source">A sequence of values to calculate find the min for.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result = queryable.Min("Income");
        /// </code>
        /// </example>
        /// <returns>The min element in the sequence.</returns>
        [PublicAPI]
        public static object Min([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, typeof(object), predicate, args);

            return Execute(_minPredicate, source, lambda);
        }

        /// <inheritdoc cref="Min(IQueryable, ParsingConfig, string, object[])"/>
        [PublicAPI]
        public static object Min([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return Min(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Computes the min element of a sequence.
        /// </summary>
        /// <param name="source">A sequence of values to calculate find the min for.</param>
        /// <param name="lambda">A Lambda Expression.</param>
        /// <returns>The min element in the sequence.</returns>
        [PublicAPI]
        public static object Min([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
        {
            Check.NotNull(source, nameof(source));
            return Execute(_minPredicate, source, lambda);
        }
        #endregion Min

        #region OfType
        private static readonly MethodInfo _ofType = GetGenericMethod(nameof(Queryable.OfType));

        /// <summary>
        /// Filters the elements of an <see cref="IQueryable"/> based on a specified type.
        /// </summary>
        /// <param name="source">An <see cref="IQueryable"/> whose elements to filter.</param>
        /// <param name="type">The type to filter the elements of the sequence on.</param>
        /// <returns>A collection that contains the elements from source that have the type.</returns>
        public static IQueryable OfType([NotNull] this IQueryable source, [NotNull] Type type)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(type, nameof(type));

            var optimized = OptimizeExpression(Expression.Call(null, _ofType.MakeGenericMethod(new Type[] { type }), new Expression[] { source.Expression }));

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Filters the elements of an <see cref="IQueryable"/> based on a specified type.
        /// </summary>
        /// <param name="source">An <see cref="IQueryable"/> whose elements to filter.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="typeName">The type to filter the elements of the sequence on.</param>
        /// <returns>A collection that contains the elements from source that have the type.</returns>
        public static IQueryable OfType([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string typeName)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(typeName, nameof(typeName));

            var finder = new TypeFinder(config, new KeywordsHelper(config));
            Type type = finder.FindTypeByName(typeName, null, true);

            return OfType(source, type);
        }

        /// <summary>
        /// Filters the elements of an <see cref="IQueryable"/> based on a specified type.
        /// </summary>
        /// <param name="source">An <see cref="IQueryable"/> whose elements to filter.</param>
        /// <param name="typeName">The type to filter the elements of the sequence on.</param>
        /// <returns>A collection that contains the elements from source that have the type.</returns>
        public static IQueryable OfType([NotNull] this IQueryable source, [NotNull] string typeName)
        {
            return OfType(source, ParsingConfig.Default, typeName);
        }
        #endregion OfType

        #region OrderBy
        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable{T}"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var resultSingle = queryable.OrderBy<User>("NumberProperty");
        /// var resultSingleDescending = queryable.OrderBy<User>("NumberProperty DESC");
        /// var resultMultiple = queryable.OrderBy<User>("NumberProperty, StringProperty");
        /// ]]>
        /// </code>
        /// </example>
        public static IOrderedQueryable<TSource> OrderBy<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] ParsingConfig config, [NotNull] string ordering, params object[] args)
        {
            return (IOrderedQueryable<TSource>)OrderBy((IQueryable)source, config, ordering, args);
        }

        /// <inheritdoc cref="OrderBy{TSource}(IQueryable{TSource}, ParsingConfig, string, object[])"/>
        public static IOrderedQueryable<TSource> OrderBy<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] string ordering, params object[] args)
        {
            return OrderBy(source, ParsingConfig.Default, ordering, args);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable{T}"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] ParsingConfig config, [NotNull] string ordering, IComparer comparer, params object[] args)
        {
            return (IOrderedQueryable<TSource>)InternalOrderBy((IQueryable)source, config, ordering, comparer, args);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable{T}"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        public static IOrderedQueryable<TSource> OrderBy<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] string ordering, IComparer comparer, params object[] args)
        {
            return OrderBy(source, ParsingConfig.Default, ordering, comparer, args);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        /// <example>
        /// <code>
        /// var resultSingle = queryable.OrderBy("NumberProperty");
        /// var resultSingleDescending = queryable.OrderBy("NumberProperty DESC");
        /// var resultMultiple = queryable.OrderBy("NumberProperty, StringProperty DESC");
        /// </code>
        /// </example>
        public static IOrderedQueryable OrderBy([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string ordering, params object[] args)
        {
            return InternalOrderBy(source, config, ordering, null, args);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        public static IOrderedQueryable OrderBy([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string ordering, IComparer comparer, params object[] args)
        {
            return InternalOrderBy(source, config, ordering, comparer, args);
        }

        internal static IOrderedQueryable InternalOrderBy([NotNull] IQueryable source, [NotNull] ParsingConfig config, [NotNull] string ordering, IComparer comparer, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(ordering, nameof(ordering));

            ParameterExpression[] parameters = { ParameterExpressionHelper.CreateParameterExpression(source.ElementType, string.Empty, config.RenameEmptyParameterExpressionNames) };
            ExpressionParser parser = new ExpressionParser(parameters, ordering, args, config);
            IList<DynamicOrdering> dynamicOrderings = parser.ParseOrdering();

            Expression queryExpr = source.Expression;

            foreach (DynamicOrdering dynamicOrdering in dynamicOrderings)
            {
                if (comparer == null)
                {
                    queryExpr = Expression.Call(
                        typeof(Queryable), dynamicOrdering.MethodName,
                        new[] { source.ElementType, dynamicOrdering.Selector.Type },
                        queryExpr, Expression.Quote(Expression.Lambda(dynamicOrdering.Selector, parameters)));
                }
                else
                {
                    var comparerGenericType = typeof(IComparer<>).MakeGenericType(dynamicOrdering.Selector.Type);
                    queryExpr = Expression.Call(
                        typeof(Queryable), dynamicOrdering.MethodName,
                        new[] { source.ElementType, dynamicOrdering.Selector.Type },
                        queryExpr, Expression.Quote(Expression.Lambda(dynamicOrdering.Selector, parameters)),
                        Expression.Constant(comparer, comparerGenericType));
                }
            }

            var optimized = OptimizeExpression(queryExpr);
            return (IOrderedQueryable)source.Provider.CreateQuery(optimized);
        }

        /// <inheritdoc cref="OrderBy(IQueryable, ParsingConfig, string, object[])"/>
        public static IOrderedQueryable OrderBy([NotNull] this IQueryable source, [NotNull] string ordering, params object[] args)
        {
            return OrderBy(source, ParsingConfig.Default, ordering, args);
        }

        /// <inheritdoc cref="OrderBy(IQueryable, ParsingConfig, string, IComparer, object[])"/>
        public static IOrderedQueryable OrderBy([NotNull] this IQueryable source, [NotNull] string ordering, IComparer comparer, params object[] args)
        {
            return OrderBy(source, ParsingConfig.Default, ordering, comparer, args);
        }

        #endregion OrderBy

        #region Page/PageResult
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
        /// <param name="rowCount">If this optional parameter has been defined, this value is used as the RowCount instead of executing a Linq `Count()`.</param>
        /// <returns>PagedResult</returns>
        public static PagedResult PageResult([NotNull] this IQueryable source, int page, int pageSize, int? rowCount = null)
        {
            Check.NotNull(source, nameof(source));
            Check.Condition(page, p => p > 0, nameof(page));
            Check.Condition(pageSize, ps => ps > 0, nameof(pageSize));
            Check.Condition(rowCount, rc => rc == null || rc >= 0, nameof(rowCount));

            var result = new PagedResult
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = rowCount ?? source.Count()
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
        /// <param name="rowCount">If this optional parameter has been defined, this value is used as the RowCount instead of executing a Linq `Count()`.</param>
        /// <returns>PagedResult{TSource}</returns>
        public static PagedResult<TSource> PageResult<TSource>([NotNull] this IQueryable<TSource> source, int page, int pageSize, int? rowCount = null)
        {
            Check.NotNull(source, nameof(source));
            Check.Condition(page, p => p > 0, nameof(page));
            Check.Condition(pageSize, ps => ps > 0, nameof(pageSize));
            Check.Condition(rowCount, rc => rc == null || rc >= 0, nameof(rowCount));

            var result = new PagedResult<TSource>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = rowCount ?? Queryable.Count(source)
            };

            result.PageCount = (int)Math.Ceiling((double)result.RowCount / pageSize);
            result.Queryable = Page(source, page, pageSize);

            return result;
        }
        #endregion Page/PageResult

        #region Reverse
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
        #endregion Reverse

        #region Select
        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> whose elements are the result of invoking a projection string on each element of source.</returns>
        /// <example>
        /// <code>
        /// var singleField = queryable.Select("StringProperty");
        /// var dynamicObject = queryable.Select("new (StringProperty1, StringProperty2 as OtherStringPropertyName)");
        /// </code>
        /// </example>
        public static IQueryable Select([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(selector, nameof(selector));

            bool createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, selector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.Select),
                new[] { source.ElementType, lambda.Body.Type },
                source.Expression, Expression.Quote(lambda))
            );

            return source.Provider.CreateQuery(optimized);
        }

        /// <inheritdoc cref="Select(IQueryable, ParsingConfig, string, object[])"/>
        public static IQueryable Select([NotNull] this IQueryable source, [NotNull] string selector, params object[] args)
        {
            return Select(source, ParsingConfig.Default, selector, args);
        }

        /// <summary>
        /// Projects each element of a sequence into a new class of type TResult.
        /// Details see <see href="http://solutionizing.net/category/linq/"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
        /// <returns>An <see cref="IQueryable{TResult}"/> whose elements are the result of invoking a projection string on each element of source.</returns>
        /// <example>
        /// <code language="cs">
        /// <![CDATA[
        /// var users = queryable.Select<User>("new (Username, Pwd as Password)");
        /// ]]>
        /// </code>
        /// </example>
        public static IQueryable<TResult> Select<TResult>([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(selector, nameof(selector));

            bool createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, typeof(TResult), selector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.Select),
                new[] { source.ElementType, typeof(TResult) },
                source.Expression, Expression.Quote(lambda)));

            return source.Provider.CreateQuery<TResult>(optimized);
        }

        /// <inheritdoc cref="Select{TResult}(IQueryable, ParsingConfig, string, object[])"/>
        public static IQueryable<TResult> Select<TResult>([NotNull] this IQueryable source, [NotNull] string selector, params object[] args)
        {
            return Select<TResult>(source, ParsingConfig.Default, selector, args);
        }

        /// <summary>
        /// Projects each element of a sequence into a new class of type TResult.
        /// Details see http://solutionizing.net/category/linq/ 
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="resultType">The result type.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
        /// <returns>An <see cref="IQueryable"/> whose elements are the result of invoking a projection string on each element of source.</returns>
        /// <example>
        /// <code>
        /// var users = queryable.Select(typeof(User), "new (Username, Pwd as Password)");
        /// </code>
        /// </example>
        public static IQueryable Select([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] Type resultType, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotNull(resultType, nameof(resultType));
            Check.NotEmpty(selector, nameof(selector));

            bool createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, resultType, selector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.Select),
                new[] { source.ElementType, resultType },
                source.Expression, Expression.Quote(lambda)));

            return source.Provider.CreateQuery(optimized);
        }

        /// <inheritdoc cref="Select(IQueryable, ParsingConfig, Type, string, object[])"/>
        public static IQueryable Select([NotNull] this IQueryable source, [NotNull] Type resultType, [NotNull] string selector, params object[] args)
        {
            return Select(source, ParsingConfig.Default, resultType, selector, args);
        }

        #endregion Select

        #region SelectMany
        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable"/> and combines the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> whose elements are the result of invoking a one-to-many projection function on each element of the input sequence.</returns>
        /// <example>
        /// <code>
        /// var roles = users.SelectMany("Roles");
        /// </code>
        /// </example>
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string selector, params object[] args)
        {
            return SelectManyInternal(source, config, null, selector, args);
        }

        /// <inheritdoc cref="SelectMany(IQueryable, ParsingConfig, string, object[])"/>
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] string selector, params object[] args)
        {
            return SelectMany(source, ParsingConfig.Default, selector, args);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable"/> and combines the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="resultType">The result type.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> whose elements are the result of invoking a one-to-many projection function on each element of the input sequence.</returns>
        /// <example>
        /// <code>
        /// var permissions = users.SelectMany(typeof(Permission), "Roles.SelectMany(Permissions)");
        /// </code>
        /// </example>
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] Type resultType, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotNull(resultType, nameof(resultType));
            Check.NotEmpty(selector, nameof(selector));

            return SelectManyInternal(source, config, resultType, selector, args);
        }

        /// <inheritdoc cref="SelectMany(IQueryable, ParsingConfig, Type, string, object[])"/>
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] Type resultType, [NotNull] string selector, params object[] args)
        {
            return SelectMany(source, ParsingConfig.Default, resultType, selector, args);
        }

        private static IQueryable SelectManyInternal(IQueryable source, ParsingConfig config, Type resultType, string selector, params object[] args)
        {
            bool createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, selector, args);

            //Extra help to get SelectMany to work from StackOverflow Answer
            //http://stackoverflow.com/a/3001674/2465182

            // if resultType is not specified, create one based on the lambda.Body.Type
            if (resultType == null)
            {
                // SelectMany assumes that lambda.Body.Type is a generic type and throws an exception on
                // lambda.Body.Type.GetGenericArguments()[0] when used over an array as GetGenericArguments() returns an empty array.
                if (lambda.Body.Type.IsArray)
                {
                    resultType = lambda.Body.Type.GetElementType();
                }
                else
                {
                    resultType = lambda.Body.Type.GetGenericArguments()[0];
                }
            }

            //we have to adjust to lambda to return an IEnumerable<T> instead of whatever the actual property is.
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(resultType);
            Type inputType = source.Expression.Type.GetTypeInfo().GetGenericTypeArguments()[0];
            Type delegateType = typeof(Func<,>).MakeGenericType(inputType, enumerableType);
            lambda = Expression.Lambda(delegateType, lambda.Body, lambda.Parameters);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.SelectMany),
                new[] { source.ElementType, resultType },
                source.Expression, Expression.Quote(lambda))
            );

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable{TResult}"/> and combines the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable{TResult}"/> whose elements are the result of invoking a one-to-many projection function on each element of the input sequence.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var permissions = users.SelectMany<Permission>("Roles.SelectMany(Permissions)");
        /// ]]>
        /// </code>
        /// </example>
        public static IQueryable<TResult> SelectMany<TResult>([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(selector, nameof(selector));

            bool createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, selector, args);

            //we have to adjust to lambda to return an IEnumerable<T> instead of whatever the actual property is.
            Type inputType = source.Expression.Type.GetTypeInfo().GetGenericTypeArguments()[0];
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(TResult));
            Type delegateType = typeof(Func<,>).MakeGenericType(inputType, enumerableType);
            lambda = Expression.Lambda(delegateType, lambda.Body, lambda.Parameters);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.SelectMany),
                new[] { source.ElementType, typeof(TResult) },
                source.Expression, Expression.Quote(lambda))
            );

            return source.Provider.CreateQuery<TResult>(optimized);
        }

        /// <inheritdoc cref="SelectMany{TResult}(IQueryable, ParsingConfig, string, object[])"/>
        public static IQueryable<TResult> SelectMany<TResult>([NotNull] this IQueryable source, [NotNull] string selector, params object[] args)
        {
            return SelectMany<TResult>(source, ParsingConfig.Default, selector, args);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable"/>
        /// and invokes a result selector function on each element therein. The resulting
        /// values from each intermediate sequence are combined into a single, one-dimensional
        /// sequence and returned.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="collectionSelector">A projection function to apply to each element of the input sequence.</param>
        /// <param name="resultSelector">A projection function to apply to each element of each intermediate sequence. Should only use x and y as parameter names.</param>
        /// <param name="collectionSelectorArgs">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="resultSelectorArgs">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        /// An <see cref="IQueryable"/> whose elements are the result of invoking the one-to-many 
        /// projection function <paramref name="collectionSelector"/> on each element of source and then mapping
        /// each of those sequence elements and their corresponding source element to a result element.
        /// </returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// // TODO
        /// ]]>
        /// </code>
        /// </example>
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string collectionSelector, [NotNull] string resultSelector, [CanBeNull] object[] collectionSelectorArgs = null, [CanBeNull] params object[] resultSelectorArgs)
        {
            return SelectMany(source, collectionSelector, resultSelector, "x", "y", collectionSelectorArgs, resultSelectorArgs);
        }

        /// <inheritdoc cref="SelectMany(IQueryable, ParsingConfig, string, string, string, string, object[], object[])"/>
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] string collectionSelector, [NotNull] string resultSelector, [CanBeNull] object[] collectionSelectorArgs = null, [CanBeNull] params object[] resultSelectorArgs)
        {
            return SelectMany(source, ParsingConfig.Default, collectionSelector, resultSelector, "x", "y", collectionSelectorArgs, resultSelectorArgs);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable"/>
        /// and invokes a result selector function on each element therein. The resulting
        /// values from each intermediate sequence are combined into a single, one-dimensional
        /// sequence and returned.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="collectionSelector">A projection function to apply to each element of the input sequence.</param>
        /// <param name="collectionParameterName">The name from collectionParameter to use. Default is x.</param>
        /// <param name="resultSelector">A projection function to apply to each element of each intermediate sequence.</param>
        /// <param name="resultParameterName">The name from resultParameterName to use. Default is y.</param>
        /// <param name="collectionSelectorArgs">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <param name="resultSelectorArgs">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>
        /// An <see cref="IQueryable"/> whose elements are the result of invoking the one-to-many 
        /// projection function <paramref name="collectionSelector"/> on each element of source and then mapping
        /// each of those sequence elements and their corresponding source element to a result element.
        /// </returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// // TODO
        /// ]]>
        /// </code>
        /// </example>
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string collectionSelector, [NotNull] string resultSelector, [NotNull] string collectionParameterName, [NotNull] string resultParameterName, [CanBeNull] object[] collectionSelectorArgs = null, [CanBeNull] params object[] resultSelectorArgs)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(collectionSelector, nameof(collectionSelector));
            Check.NotEmpty(collectionParameterName, nameof(collectionParameterName));
            Check.NotEmpty(resultSelector, nameof(resultSelector));
            Check.NotEmpty(resultParameterName, nameof(resultParameterName));

            bool createParameterCtor = config?.EvaluateGroupByAtDatabase ?? SupportsLinqToObjects(config, source);
            LambdaExpression sourceSelectLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, collectionSelector, collectionSelectorArgs);

            //we have to adjust to lambda to return an IEnumerable<T> instead of whatever the actual property is.
            Type sourceLambdaInputType = source.Expression.Type.GetGenericArguments()[0];
            Type sourceLambdaResultType = sourceSelectLambda.Body.Type.GetGenericArguments()[0];
            Type sourceLambdaEnumerableType = typeof(IEnumerable<>).MakeGenericType(sourceLambdaResultType);
            Type sourceLambdaDelegateType = typeof(Func<,>).MakeGenericType(sourceLambdaInputType, sourceLambdaEnumerableType);

            sourceSelectLambda = Expression.Lambda(sourceLambdaDelegateType, sourceSelectLambda.Body, sourceSelectLambda.Parameters);

            //we have to create additional lambda for result selection
            ParameterExpression xParameter = ParameterExpressionHelper.CreateParameterExpression(source.ElementType, collectionParameterName, config.RenameEmptyParameterExpressionNames);
            ParameterExpression yParameter = ParameterExpressionHelper.CreateParameterExpression(sourceLambdaResultType, resultParameterName, config.RenameEmptyParameterExpressionNames);

            LambdaExpression resultSelectLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, new[] { xParameter, yParameter }, null, resultSelector, resultSelectorArgs);
            Type resultLambdaResultType = resultSelectLambda.Body.Type;

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.SelectMany),
                new[] { source.ElementType, sourceLambdaResultType, resultLambdaResultType },
                source.Expression, Expression.Quote(sourceSelectLambda), Expression.Quote(resultSelectLambda))
            );

            return source.Provider.CreateQuery(optimized);
        }

        /// <inheritdoc cref="SelectMany(IQueryable, ParsingConfig, string, string, string, string, object[], object[])"/>
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] string collectionSelector, [NotNull] string resultSelector, [NotNull] string collectionParameterName, [NotNull] string resultParameterName, [CanBeNull] object[] collectionSelectorArgs = null, [CanBeNull] params object[] resultSelectorArgs)
        {
            return SelectMany(source, ParsingConfig.Default, collectionSelector, resultSelector, collectionParameterName, resultParameterName, collectionSelectorArgs, resultSelectorArgs);
        }

        #endregion SelectMany

        #region Single/SingleOrDefault
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

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), nameof(Queryable.Single), new[] { source.ElementType }, source.Expression));
            return source.Provider.Execute(optimized);
        }

        private static readonly MethodInfo _singlePredicate = GetMethod(nameof(Queryable.Single), 1);

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if there
        /// is not exactly one element in the sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object Single([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
#else
        public static dynamic Single([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_singlePredicate, source, lambda);
        }

        /// <inheritdoc cref="Single(IQueryable, ParsingConfig, string, object[])"/>
#if NET35
        public static object Single([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#else
        public static dynamic Single([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#endif
        {
            return Single(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if there
        /// is not exactly one element in the sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object Single([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
#else
        public static dynamic Single([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
#endif
        {
            Check.NotNull(source, nameof(source));
            return Execute(_singlePredicate, source, lambda);
        }

        /// <summary>
        /// Returns the only element of a sequence, or a default value if the sequence
        /// is empty; this method throws an exception if there is more than one element
        /// in the sequence.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> to return the single element of.</param>
        /// <returns>The single element of the input sequence, or default if the sequence contains no elements.</returns>
#if NET35
        public static object SingleOrDefault([NotNull] this IQueryable source)
#else
        public static dynamic SingleOrDefault([NotNull] this IQueryable source)
#endif
        {
            Check.NotNull(source, nameof(source));

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), nameof(Queryable.SingleOrDefault), new[] { source.ElementType }, source.Expression));
            return source.Provider.Execute(optimized);
        }

        private static readonly MethodInfo _singleDefaultPredicate = GetMethod(nameof(Queryable.SingleOrDefault), 1);

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition or a default value if the sequence
        /// is empty; and throws an exception if there is not exactly one element in the sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object SingleOrDefault([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
#else
        public static dynamic SingleOrDefault([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_singleDefaultPredicate, source, lambda);
        }

        /// <inheritdoc cref="SingleOrDefault(IQueryable, ParsingConfig, string, object[])"/>
#if NET35
        public static object SingleOrDefault([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#else
        public static dynamic SingleOrDefault([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#endif
        {
            return SingleOrDefault(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition or a default value if the sequence
        /// is empty; and throws an exception if there is not exactly one element in the sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object SingleOrDefault([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
#else
        public static dynamic SingleOrDefault([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(lambda, nameof(lambda));

            return Execute(_singleDefaultPredicate, source, lambda);
        }
        #endregion Single/SingleOrDefault

        #region Skip
        private static readonly MethodInfo _skip = GetMethod(nameof(Queryable.Skip), 1);

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

            return CreateQuery(_skip, source, Expression.Constant(count));
        }
        #endregion Skip

        #region SkipWhile

        private static readonly MethodInfo _skipWhilePredicate = GetMethod(nameof(Queryable.SkipWhile), 1, mi =>
        {
            return mi.GetParameters().Length == 2 &&
                   mi.GetParameters()[1].ParameterType.GetTypeInfo().IsGenericType &&
                   mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>) &&
                   mi.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetTypeInfo().IsGenericType &&
                   mi.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>);
        });

        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.SkipWhile("Income > 50");
        /// var result2 = queryable.SkipWhile("Income > @0", 50);
        /// </code>
        /// </example>
        /// <returns>An <see cref="IQueryable"/> that contains elements from source starting at the first element in the linear series that does not pass the test specified by predicate.</returns>
        public static IQueryable SkipWhile([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotNull(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return CreateQuery(_skipWhilePredicate, source, lambda);
        }

        /// <inheritdoc cref="SkipWhile(IQueryable, ParsingConfig, string, object[])"/>
        public static IQueryable SkipWhile([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return SkipWhile(source, ParsingConfig.Default, predicate, args);
        }

        #endregion SkipWhile

        #region Sum
        /// <summary>
        /// Computes the sum of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the sum of.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.Sum();
        /// var result2 = queryable.Select("Roles.Sum()");
        /// </code>
        /// </example>
        /// <returns>The sum of the values in the sequence.</returns>
        [PublicAPI]
        public static object Sum([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            var sum = GetMethod(nameof(Queryable.Sum), source.ElementType);
            return Execute<object>(sum, source);
        }

        /// <summary>
        /// Computes the sum of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the sum of.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result = queryable.Sum("Income");
        /// </code>
        /// </example>
        /// <returns>The sum of the values in the sequence.</returns>
        [PublicAPI]
        public static object Sum([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            var sumSelector = GetMethod(nameof(Queryable.Sum), lambda.GetReturnType(), 1);

            return Execute<object>(sumSelector, source, lambda);
        }

        /// <inheritdoc cref="Sum(IQueryable, ParsingConfig, string, object[])"/>
        [PublicAPI]
        public static object Sum([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return Sum(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Computes the sum of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the sum of.</param>
        /// <param name="lambda">A Lambda Expression.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        [PublicAPI]
        public static object Sum([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(lambda, nameof(lambda));

            var sumSelector = GetMethod(nameof(Queryable.Sum), lambda.GetReturnType(), 1);

            return Execute<object>(sumSelector, source, lambda);
        }
        #endregion Sum

        #region Take
        private static readonly MethodInfo _take = GetMethod(nameof(Queryable.Take), 1);
        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>A <see cref="IQueryable"/> that contains the specified number of elements from the start of source.</returns>
        public static IQueryable Take([NotNull] this IQueryable source, int count)
        {
            Check.NotNull(source, nameof(source));
            Check.Condition(count, x => x >= 0, nameof(count));

            return CreateQuery(_take, source, Expression.Constant(count));
        }
        #endregion Take

        #region TakeWhile

        private static readonly MethodInfo _takeWhilePredicate = GetMethod(nameof(Queryable.TakeWhile), 1, mi =>
        {
            return mi.GetParameters().Length == 2 &&
                   mi.GetParameters()[1].ParameterType.GetTypeInfo().IsGenericType &&
                   mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>) &&
                   mi.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetTypeInfo().IsGenericType &&
                   mi.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericTypeDefinition() == typeof(Func<,>);
        });

        /// <summary>
        /// Returns elements from a sequence as long as a specified condition is true.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <example>
        /// <code language="cs">
        /// IQueryable queryable = employees.AsQueryable();
        /// var result1 = queryable.TakeWhile("Income > 50");
        /// var result2 = queryable.TakeWhile("Income > @0", 50);
        /// </code>
        /// </example>
        /// <returns>An <see cref="IQueryable"/> that contains elements from the input sequence occurring before the element at which the test specified by predicate no longer passes.</returns>
        public static IQueryable TakeWhile([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotNull(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return CreateQuery(_takeWhilePredicate, source, lambda);
        }

        /// <inheritdoc cref="TakeWhile(IQueryable, ParsingConfig, string, object[])"/>
        public static IQueryable TakeWhile([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            return TakeWhile(source, ParsingConfig.Default, predicate, args);
        }

        #endregion TakeWhile

        #region ThenBy
        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// var result = queryable.OrderBy<User>("LastName");
        /// var resultSingle = result.ThenBy<User>("NumberProperty");
        /// var resultSingleDescending = result.ThenBy<User>("NumberProperty DESC");
        /// var resultMultiple = result.ThenBy<User>("NumberProperty, StringProperty");
        /// ]]>
        /// </code>
        /// </example>
        public static IOrderedQueryable<TSource> ThenBy<TSource>([NotNull] this IOrderedQueryable<TSource> source, [NotNull] ParsingConfig config, [NotNull] string ordering, params object[] args)
        {
            return (IOrderedQueryable<TSource>)ThenBy((IOrderedQueryable)source, config, ordering, args);
        }

        /// <inheritdoc cref="ThenBy{TSource}(IOrderedQueryable{TSource}, ParsingConfig, string, object[])"/>
        public static IOrderedQueryable<TSource> ThenBy<TSource>([NotNull] this IOrderedQueryable<TSource> source, [NotNull] string ordering, params object[] args)
        {
            return ThenBy(source, ParsingConfig.Default, ordering, args);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        public static IOrderedQueryable<TSource> ThenBy<TSource>([NotNull] this IOrderedQueryable<TSource> source, [NotNull] ParsingConfig config, [NotNull] string ordering, IComparer comparer, params object[] args)
        {
            return (IOrderedQueryable<TSource>)InternalThenBy((IOrderedQueryable)source, config, ordering, comparer, args);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        public static IOrderedQueryable<TSource> ThenBy<TSource>([NotNull] this IOrderedQueryable<TSource> source, [NotNull] string ordering, IComparer comparer, params object[] args)
        {
            return ThenBy(source, ParsingConfig.Default, ordering, comparer, args);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        /// <example>
        /// <code>
        /// var result = queryable.OrderBy("LastName");
        /// var resultSingle = result.OrderBy("NumberProperty");
        /// var resultSingleDescending = result.OrderBy("NumberProperty DESC");
        /// var resultMultiple = result.OrderBy("NumberProperty, StringProperty DESC");
        /// </code>
        /// </example>
        public static IOrderedQueryable ThenBy([NotNull] this IOrderedQueryable source, [NotNull] ParsingConfig config, [NotNull] string ordering, params object[] args)
        {
            return InternalThenBy(source, config, ordering, null, args);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        public static IOrderedQueryable ThenBy([NotNull] this IOrderedQueryable source, [NotNull] ParsingConfig config, [NotNull] string ordering, IComparer comparer, params object[] args)
        {
            return InternalThenBy(source, config, ordering, comparer, args);
        }

        internal static IOrderedQueryable InternalThenBy([NotNull] IOrderedQueryable source, [NotNull] ParsingConfig config, [NotNull] string ordering, IComparer comparer, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(ordering, nameof(ordering));

            ParameterExpression[] parameters = { ParameterExpressionHelper.CreateParameterExpression(source.ElementType, string.Empty, config.RenameEmptyParameterExpressionNames) };
            ExpressionParser parser = new ExpressionParser(parameters, ordering, args, config);
            IList<DynamicOrdering> dynamicOrderings = parser.ParseOrdering(forceThenBy: true);

            Expression queryExpr = source.Expression;

            foreach (DynamicOrdering dynamicOrdering in dynamicOrderings)
            {
                if (comparer == null)
                {
                    queryExpr = Expression.Call(
                        typeof(Queryable), dynamicOrdering.MethodName,
                        new[] { source.ElementType, dynamicOrdering.Selector.Type },
                        queryExpr, Expression.Quote(Expression.Lambda(dynamicOrdering.Selector, parameters)));
                }
                else
                {
                    var comparerGenericType = typeof(IComparer<>).MakeGenericType(dynamicOrdering.Selector.Type);
                    queryExpr = Expression.Call(
                        typeof(Queryable), dynamicOrdering.MethodName,
                        new[] { source.ElementType, dynamicOrdering.Selector.Type },
                        queryExpr, Expression.Quote(Expression.Lambda(dynamicOrdering.Selector, parameters)),
                        Expression.Constant(comparer, comparerGenericType));
                }
            }

            var optimized = OptimizeExpression(queryExpr);
            return (IOrderedQueryable)source.Provider.CreateQuery(optimized);
        }

        /// <inheritdoc cref="ThenBy(IOrderedQueryable, ParsingConfig, string, object[])"/>
        public static IOrderedQueryable ThenBy([NotNull] this IOrderedQueryable source, [NotNull] string ordering, params object[] args)
        {
            return ThenBy(source, ParsingConfig.Default, ordering, args);
        }

        /// <inheritdoc cref="ThenBy(IOrderedQueryable, ParsingConfig, string, IComparer, object[])"/>
        public static IOrderedQueryable ThenBy([NotNull] this IOrderedQueryable source, [NotNull] string ordering, IComparer comparer, params object[] args)
        {
            return ThenBy(source, ParsingConfig.Default, ordering, comparer, args);
        }

        #endregion OrderBy

        #region Where
        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A <see cref="IQueryable{TSource}"/> to filter.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">An expression string to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable{TSource}"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
        /// <example>
        /// <code language="cs">
        /// var result1 = queryable.Where("NumberProperty = 1");
        /// var result2 = queryable.Where("NumberProperty = @0", 1);
        /// var result3 = queryable.Where("StringProperty = null");
        /// var result4 = queryable.Where("StringProperty = \"abc\"");
        /// var result5 = queryable.Where("StringProperty = @0", "abc");
        /// </code>
        /// </example>
        public static IQueryable<TSource> Where<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            return (IQueryable<TSource>)Where((IQueryable)source, config, predicate, args);
        }

        /// <inheritdoc cref="DynamicQueryableExtensions.Where{TSource}(IQueryable{TSource}, ParsingConfig, string, object[])"/>
        public static IQueryable<TSource> Where<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] string predicate, params object[] args)
        {
            return Where(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> to filter.</param>
        /// <param name="config">The <see cref="ParsingConfig"/>.</param>
        /// <param name="predicate">An expression string to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
        /// <example>
        /// <code>
        /// var result1 = queryable.Where("NumberProperty = 1");
        /// var result2 = queryable.Where("NumberProperty = @0", 1);
        /// var result3 = queryable.Where("StringProperty = null");
        /// var result4 = queryable.Where("StringProperty = \"abc\"");
        /// var result5 = queryable.Where("StringProperty = @0", "abc");
        /// </code>
        /// </example>
        public static IQueryable Where([NotNull] this IQueryable source, [NotNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(config, nameof(config));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = SupportsLinqToObjects(config, source);
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), nameof(Queryable.Where), new[] { source.ElementType }, source.Expression, Expression.Quote(lambda)));
            return source.Provider.CreateQuery(optimized);
        }

        /// <inheritdoc cref="DynamicQueryableExtensions.Where(IQueryable, ParsingConfig, string, object[])"/>
        public static IQueryable Where([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
        {
            return Where(source, ParsingConfig.Default, predicate, args);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> to filter.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        /// <returns>A <see cref="IQueryable"/> that contains elements from the input sequence that satisfy the condition specified by LambdaExpression.</returns>
        public static IQueryable Where([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(lambda, nameof(lambda));

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), nameof(Queryable.Where), new[] { source.ElementType }, source.Expression, Expression.Quote(lambda)));
            return source.Provider.CreateQuery(optimized);
        }

        /// <inheritdoc cref="DynamicQueryableExtensions.Where(IQueryable, LambdaExpression)"/>
        public static IQueryable<TSource> Where<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] LambdaExpression lambda)
        {
            return (IQueryable<TSource>)Where((IQueryable)source, lambda);
        }
        #endregion

        #region Private Helpers

        private static bool SupportsLinqToObjects(ParsingConfig config, IQueryable query)
        {
            return config.QueryableAnalyzer.SupportsLinqToObjects(query);
        }

        private static void CheckOuterAndInnerTypes(ParsingConfig config, bool createParameterCtor, Type outerType, Type innerType, string outerKeySelector, string innerKeySelector, ref LambdaExpression outerSelectorLambda, ref LambdaExpression innerSelectorLambda, params object[] args)
        {
            Type outerSelectorReturnType = outerSelectorLambda.Body.Type;
            Type innerSelectorReturnType = innerSelectorLambda.Body.Type;

            // If types are not the same, try to convert to Nullable and generate new LambdaExpression
            if (outerSelectorReturnType != innerSelectorReturnType)
            {
                if (TypeHelper.IsNullableType(outerSelectorReturnType) && !TypeHelper.IsNullableType(innerSelectorReturnType))
                {
                    innerSelectorReturnType = ExpressionParser.ToNullableType(innerSelectorReturnType);
                    innerSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, innerType, innerSelectorReturnType, innerKeySelector, args);
                }
                else if (!TypeHelper.IsNullableType(outerSelectorReturnType) && TypeHelper.IsNullableType(innerSelectorReturnType))
                {
                    outerSelectorReturnType = ExpressionParser.ToNullableType(outerSelectorReturnType);
                    outerSelectorLambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, outerType, outerSelectorReturnType, outerKeySelector, args);
                }

                // If types are still not the same, throw an Exception
                if (outerSelectorReturnType != innerSelectorReturnType)
                {
                    throw new ParseException(string.Format(CultureInfo.CurrentCulture, Res.IncompatibleTypes, outerSelectorReturnType, innerSelectorReturnType), -1);
                }
            }
        }

        // Code below is based on https://github.com/aspnet/EntityFramework/blob/9186d0b78a3176587eeb0f557c331f635760fe92/src/Microsoft.EntityFrameworkCore/EntityFrameworkQueryableExtensions.cs
        private static IQueryable CreateQuery(MethodInfo operatorMethodInfo, IQueryable source)
        {
            if (operatorMethodInfo.IsGenericMethod)
            {
                operatorMethodInfo = operatorMethodInfo.MakeGenericMethod(source.ElementType);
            }

            var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, source.Expression));
            return source.Provider.CreateQuery(optimized);
        }

        private static IQueryable CreateQuery(MethodInfo operatorMethodInfo, IQueryable source, LambdaExpression expression)
            => CreateQuery(operatorMethodInfo, source, Expression.Quote(expression));

        private static IQueryable CreateQuery(MethodInfo operatorMethodInfo, IQueryable source, Expression expression)
        {
            operatorMethodInfo = operatorMethodInfo.GetGenericArguments().Length == 2
                    ? operatorMethodInfo.MakeGenericMethod(source.ElementType, typeof(object))
                    : operatorMethodInfo.MakeGenericMethod(source.ElementType);

            return source.Provider.CreateQuery(Expression.Call(null, operatorMethodInfo, source.Expression, expression));
        }

        private static object Execute(MethodInfo operatorMethodInfo, IQueryable source)
        {
            if (operatorMethodInfo.IsGenericMethod)
            {
                operatorMethodInfo = operatorMethodInfo.MakeGenericMethod(source.ElementType);
            }

            var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, source.Expression));
            return source.Provider.Execute(optimized);
        }

        private static TResult Execute<TResult>(MethodInfo operatorMethodInfo, IQueryable source)
        {
            if (operatorMethodInfo.IsGenericMethod)
            {
                operatorMethodInfo = operatorMethodInfo.MakeGenericMethod(source.ElementType);
            }

            var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, source.Expression));
            var result = source.Provider.Execute(optimized);

            return (TResult)Convert.ChangeType(result, typeof(TResult));
        }

        private static object Execute(MethodInfo operatorMethodInfo, IQueryable source, LambdaExpression expression)
            => Execute(operatorMethodInfo, source, Expression.Quote(expression));

        private static object Execute(MethodInfo operatorMethodInfo, IQueryable source, Expression expression)
        {
            operatorMethodInfo = operatorMethodInfo.GetGenericArguments().Length == 2
                    ? operatorMethodInfo.MakeGenericMethod(source.ElementType, typeof(object))
                    : operatorMethodInfo.MakeGenericMethod(source.ElementType);

            var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, source.Expression, expression));
            return source.Provider.Execute(optimized);
        }

        private static TResult Execute<TResult>(MethodInfo operatorMethodInfo, IQueryable source, LambdaExpression expression)
            => Execute<TResult>(operatorMethodInfo, source, Expression.Quote(expression));

        private static TResult Execute<TResult>(MethodInfo operatorMethodInfo, IQueryable source, Expression expression)
        {
            operatorMethodInfo = operatorMethodInfo.GetGenericArguments().Length == 2
                    ? operatorMethodInfo.MakeGenericMethod(source.ElementType, typeof(TResult))
                    : operatorMethodInfo.MakeGenericMethod(source.ElementType);

            var optimized = OptimizeExpression(Expression.Call(null, operatorMethodInfo, source.Expression, expression));
            var result = source.Provider.Execute(optimized);

            return (TResult)Convert.ChangeType(result, typeof(TResult));
        }

        private static MethodInfo GetGenericMethod(string name)
        {
            return typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name).Single(mi => mi.IsGenericMethod);
        }

        private static MethodInfo GetMethod(string name, Type argumentType, Type returnType, int parameterCount = 0, Func<MethodInfo, bool> predicate = null) =>
            GetMethod(name, returnType, parameterCount, mi => mi.ToString().Contains(argumentType.ToString()) && ((predicate == null) || predicate(mi)));

        private static MethodInfo GetMethod(string name, Type returnType, int parameterCount = 0, Func<MethodInfo, bool> predicate = null) =>
            GetMethod(name, parameterCount, mi => (mi.ReturnType == returnType) && ((predicate == null) || predicate(mi)));

        private static MethodInfo GetMethod(string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null)
        {
            try
            {
                return typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name).Single(mi =>
                    mi.GetParameters().Length == parameterCount + 1 && (predicate == null || predicate(mi)));
            }
            catch (Exception ex)
            {
                throw new Exception("Specific method not found: " + name, ex);
            }
        }
        #endregion Private Helpers
    }
}
