using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Linq.Dynamic.Core.Exceptions;
#if !(WINDOWS_APP45x || SILVERLIGHT)
using System.Diagnostics;
#endif
using System.Linq.Dynamic.Core.Extensions;
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
        private static readonly TraceSource TraceSource = new TraceSource(typeof(DynamicQueryableExtensions).Name);
#endif
        private static readonly Func<MethodInfo, bool> PredicateParameterHas2 = mi => mi.GetParameters()[1].ToString().Contains("Func`2");

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
            ParameterExpression parameter = Expression.Parameter(source.ElementType, "s");
            Expression selector = Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter);
            // We've tried to find an expression of the type Expression<Func<TSource, TAcc>>,
            // which is expressed as ( (TSource s) => s.Price );

            var methods = typeof(Queryable).GetMethods().Where(x => x.Name == function && x.IsGenericMethod);

            // Method
            MethodInfo aggregateMethod = methods.SingleOrDefault(m =>
            {
                ParameterInfo lastParameter = m.GetParameters().LastOrDefault();

                return lastParameter != null ? TypeHelper.GetUnderlyingType(lastParameter.ParameterType) == property.PropertyType : false;
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
        public static bool Any([NotNull] this IQueryable source, [CanBeNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute<bool>(_anyPredicate, source, lambda);
        }

        /// <inheritdoc cref="Any(IQueryable, ParsingConfig, string, object[])"/>
        public static bool Any([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
        {
            return Any(source, null, predicate, args);
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
        public static int Count([NotNull] this IQueryable source, [CanBeNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            return Execute<int>(_countPredicate, source, lambda);
        }

        /// <inheritdoc cref="Count(IQueryable, ParsingConfig, string, object[])"/>
        public static int Count([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
        {
            return Count(source, null, predicate, args);
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
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object First([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#else
        public static dynamic First([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_firstPredicate, source, lambda);
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
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>default if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate.</returns>
#if NET35
        public static object FirstOrDefault([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#else
        public static dynamic FirstOrDefault([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_firstOrDefaultPredicate, source, lambda);
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
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] string keySelector, [NotNull] string resultSelector, object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(keySelector, nameof(keySelector));
            Check.NotEmpty(resultSelector, nameof(resultSelector));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression keyLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, keySelector, args);
            LambdaExpression elementLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, resultSelector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), "GroupBy",
                new[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
                source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)));

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function 
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> whose elements to group.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <param name="resultSelector">A string expression to specify a result value from each group.</param>
        /// <returns>A <see cref="IQueryable"/> where each element represents a projection over a group and its key.</returns>
        /// <example>
        /// <code>
        /// var groupResult1 = queryable.GroupBy("NumberPropertyAsKey", "StringProperty");
        /// var groupResult2 = queryable.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)", "new (StringProperty1, StringProperty2)");
        /// </code>
        /// </example>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] string keySelector, [NotNull] string resultSelector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(keySelector, nameof(keySelector));
            Check.NotEmpty(resultSelector, nameof(resultSelector));

            return GroupBy(source, keySelector, resultSelector, null);
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function 
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> whose elements to group.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> where each element represents a projection over a group and its key.</returns>
        /// <example>
        /// <code>
        /// var groupResult1 = queryable.GroupBy("NumberPropertyAsKey");
        /// var groupResult2 = queryable.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)");
        /// </code>
        /// </example>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] string keySelector, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(keySelector, nameof(keySelector));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression keyLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, keySelector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.GroupBy),
                new[] { source.ElementType, keyLambda.Body.Type }, source.Expression, Expression.Quote(keyLambda)));

            return source.Provider.CreateQuery(optimized);
        }
        #endregion GroupBy

        #region GroupByMany
        /// <summary>
        /// Groups the elements of a sequence according to multiple specified key string functions 
        /// and creates a result value from each group (and subgroups) and its key.
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source">A <see cref="IEnumerable{T}"/> whose elements to group.</param>
        /// <param name="keySelectors"><see cref="string"/> expressions to specify the keys for each element.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of type <see cref="GroupResult"/> where each element represents a projection over a group, its key, and its subgroups.</returns>
        public static IEnumerable<GroupResult> GroupByMany<TElement>([NotNull] this IEnumerable<TElement> source, params string[] keySelectors)
        {
            Check.NotNull(source, nameof(source));
            Check.HasNoNulls(keySelectors, nameof(keySelectors));

            var selectors = new List<Func<TElement, object>>(keySelectors.Length);

            bool createParameterCtor = true;
            foreach (var selector in keySelectors)
            {
                LambdaExpression l = DynamicExpressionParser.ParseLambda(createParameterCtor, typeof(TElement), typeof(object), selector);
                selectors.Add((Func<TElement, object>)l.Compile());
            }

            return GroupByManyInternal(source, selectors.ToArray(), 0);
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

        static IEnumerable<GroupResult> GroupByManyInternal<TElement>(IEnumerable<TElement> source, Func<TElement, object>[] keySelectors, int currentSelector)
        {
            if (currentSelector >= keySelectors.Length) return null;

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
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A dynamic function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A dynamic function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A dynamic function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicates as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> obtained by performing a grouped join on two sequences.</returns>
        public static IQueryable GroupJoin([NotNull] this IQueryable outer, [NotNull] IEnumerable inner, [NotNull] string outerKeySelector, [NotNull] string innerKeySelector, [NotNull] string resultSelector, params object[] args)
        {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotEmpty(outerKeySelector, nameof(outerKeySelector));
            Check.NotEmpty(innerKeySelector, nameof(innerKeySelector));
            Check.NotEmpty(resultSelector, nameof(resultSelector));

            Type outerType = outer.ElementType;
            Type innerType = inner.AsQueryable().ElementType;

            bool createParameterCtor = outer.IsLinqToObjects();
            LambdaExpression outerSelectorLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, outerType, null, outerKeySelector, args);
            LambdaExpression innerSelectorLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, innerType, null, innerKeySelector, args);

            CheckOuterAndInnerTypes(createParameterCtor, outerType, innerType, outerKeySelector, innerKeySelector, ref outerSelectorLambda, ref innerSelectorLambda, args);

            ParameterExpression[] parameters =
            {
                Expression.Parameter(outerType, "outer"),
                Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(innerType), "inner")
            };

            LambdaExpression resultSelectorLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, parameters, null, resultSelector, args);

            return outer.Provider.CreateQuery(Expression.Call(
                typeof(Queryable), "GroupJoin",
                new[] { outer.ElementType, innerType, outerSelectorLambda.Body.Type, resultSelectorLambda.Body.Type },
                outer.Expression,
                Expression.Constant(inner),
                Expression.Quote(outerSelectorLambda),
                Expression.Quote(innerSelectorLambda),
                Expression.Quote(resultSelectorLambda)));
        }
        #endregion

        #region Join
        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
        /// </summary>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A dynamic function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A dynamic function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A dynamic function to create a result element from two matching elements.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicates as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> obtained by performing an inner join on two sequences.</returns>
        public static IQueryable Join([NotNull] this IQueryable outer, [NotNull] IEnumerable inner, [NotNull] string outerKeySelector, [NotNull] string innerKeySelector, [NotNull] string resultSelector, params object[] args)
        {
            //http://stackoverflow.com/questions/389094/how-to-create-a-dynamic-linq-join-extension-method

            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotEmpty(outerKeySelector, nameof(outerKeySelector));
            Check.NotEmpty(innerKeySelector, nameof(innerKeySelector));
            Check.NotEmpty(resultSelector, nameof(resultSelector));

            Type outerType = outer.ElementType;
            Type innerType = inner.AsQueryable().ElementType;

            bool createParameterCtor = outer.IsLinqToObjects();
            LambdaExpression outerSelectorLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, outerType, null, outerKeySelector, args);
            LambdaExpression innerSelectorLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, innerType, null, innerKeySelector, args);

            CheckOuterAndInnerTypes(createParameterCtor, outerType, innerType, outerKeySelector, innerKeySelector, ref outerSelectorLambda, ref innerSelectorLambda, args);

            ParameterExpression[] parameters =
            {
                Expression.Parameter(outerType, "outer"), Expression.Parameter(innerType, "inner")
            };

            LambdaExpression resultSelectorLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, parameters, null, resultSelector, args);

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

        /// <summary>
        /// Correlates the elements of two sequences based on matching keys. The default equality comparer is used to compare keys.
        /// </summary>
        /// <typeparam name="TElement">The type of the elements of both sequences, and the result.</typeparam>
        /// <param name="outer">The first sequence to join.</param>
        /// <param name="inner">The sequence to join to the first sequence.</param>
        /// <param name="outerKeySelector">A dynamic function to extract the join key from each element of the first sequence.</param>
        /// <param name="innerKeySelector">A dynamic function to extract the join key from each element of the second sequence.</param>
        /// <param name="resultSelector">A dynamic function to create a result element from two matching elements.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicates as parameters.  Similar to the way String.Format formats strings.</param>
        /// <remarks>This overload only works on elements where both sequences and the resulting element match.</remarks>
        /// <returns>An <see cref="IQueryable{T}"/> that has elements of type TResult obtained by performing an inner join on two sequences.</returns>
        public static IQueryable<TElement> Join<TElement>([NotNull] this IQueryable<TElement> outer, [NotNull] IEnumerable<TElement> inner, [NotNull] string outerKeySelector, [NotNull] string innerKeySelector, string resultSelector, params object[] args)
        {
            return (IQueryable<TElement>)Join((IQueryable)outer, (IEnumerable)inner, outerKeySelector, innerKeySelector, resultSelector, args);
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
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object Last([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#else
        public static dynamic Last([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_lastPredicate, source, lambda);
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
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object LastOrDefault([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#else
        public static dynamic LastOrDefault([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_lastDefaultPredicate, source, lambda);
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

        #region OrderBy
        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
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
        public static IOrderedQueryable<TSource> OrderBy<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] string ordering, params object[] args)
        {
            return (IOrderedQueryable<TSource>)OrderBy((IQueryable)source, ordering, args);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
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
        public static IOrderedQueryable OrderBy([NotNull] this IQueryable source, [NotNull] string ordering, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(ordering, nameof(ordering));

            ParameterExpression[] parameters = { Expression.Parameter(source.ElementType, "") };
            ExpressionParser parser = new ExpressionParser(parameters, ordering, args, null);
            IList<DynamicOrdering> dynamicOrderings = parser.ParseOrdering();

            Expression queryExpr = source.Expression;

            foreach (DynamicOrdering dynamicOrdering in dynamicOrderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable), dynamicOrdering.MethodName,
                    new[] { source.ElementType, dynamicOrdering.Selector.Type },
                    queryExpr, Expression.Quote(Expression.Lambda(dynamicOrdering.Selector, parameters)));
            }

            var optimized = OptimizeExpression(queryExpr);
            return (IOrderedQueryable)source.Provider.CreateQuery(optimized);
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
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> whose elements are the result of invoking a projection string on each element of source.</returns>
        /// <example>
        /// <code>
        /// var singleField = queryable.Select("StringProperty");
        /// var dynamicObject = queryable.Select("new (StringProperty1, StringProperty2 as OtherStringPropertyName)");
        /// </code>
        /// </example>
        public static IQueryable Select([NotNull] this IQueryable source, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(selector, nameof(selector));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, selector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.Select),
                new[] { source.ElementType, lambda.Body.Type },
                source.Expression, Expression.Quote(lambda))
            );

            return source.Provider.CreateQuery(optimized);
        }

        /// <summary>
        /// Projects each element of a sequence into a new class of type TResult.
        /// Details see <see href="http://solutionizing.net/category/linq/"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
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
        public static IQueryable<TResult> Select<TResult>([NotNull] this IQueryable source, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(selector, nameof(selector));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, typeof(TResult), selector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.Select),
                new[] { source.ElementType, typeof(TResult) },
                source.Expression, Expression.Quote(lambda)));

            return source.Provider.CreateQuery<TResult>(optimized);
        }

        /// <summary>
        /// Projects each element of a sequence into a new class of type TResult.
        /// Details see http://solutionizing.net/category/linq/ 
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="resultType">The result type.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
        /// <returns>An <see cref="IQueryable{TResult}"/> whose elements are the result of invoking a projection string on each element of source.</returns>
        /// <example>
        /// <code>
        /// var users = queryable.Select(typeof(User), "new (Username, Pwd as Password)");
        /// </code>
        /// </example>
        public static IQueryable Select([NotNull] this IQueryable source, [NotNull] Type resultType, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(resultType, nameof(resultType));
            Check.NotEmpty(selector, nameof(selector));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, resultType, selector, args);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), nameof(Queryable.Select),
                new[] { source.ElementType, resultType },
                source.Expression, Expression.Quote(lambda)));

            return source.Provider.CreateQuery(optimized);
        }
        #endregion Select

        #region SelectMany
        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable"/> and combines the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> whose elements are the result of invoking a one-to-many projection function on each element of the input sequence.</returns>
        /// <example>
        /// <code>
        /// var roles = users.SelectMany("Roles");
        /// </code>
        /// </example>
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(selector, nameof(selector));

            return SelectManyInternal(source, null, selector, args);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable"/> and combines the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="resultType">The result type.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> whose elements are the result of invoking a one-to-many projection function on each element of the input sequence.</returns>
        /// <example>
        /// <code>
        /// var permissions = users.SelectMany(typeof(Permission), "Roles.SelectMany(Permissions)");
        /// </code>
        /// </example>
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] Type resultType, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(resultType, nameof(resultType));
            Check.NotEmpty(selector, nameof(selector));

            return SelectManyInternal(source, resultType, selector, args);
        }

        private static IQueryable SelectManyInternal(IQueryable source, Type resultType, string selector, params object[] args)
        {
            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, selector, args);

            //Extra help to get SelectMany to work from StackOverflow Answer
            //http://stackoverflow.com/a/3001674/2465182

            // if resultType is not specified, create one based on the lambda.Body.Type
            if (resultType == null)
            {
                // SelectMany assumes that lambda.Body.Type is a generic type and throws an exception on
                // lambda.Body.Type.GetGenericArguments()[0] when used over an array as GetGenericArguments() returns an empty array.
                if (lambda.Body.Type.IsArray)
                    resultType = lambda.Body.Type.GetElementType();
                else
                    resultType = lambda.Body.Type.GetGenericArguments()[0];
            }

            //we have to adjust to lambda to return an IEnumerable<T> instead of whatever the actual property is.
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(resultType);
            Type inputType = source.Expression.Type.GetTypeInfo().GetGenericTypeArguments()[0];
            Type delegateType = typeof(Func<,>).MakeGenericType(inputType, enumerableType);
            lambda = Expression.Lambda(delegateType, lambda.Body, lambda.Parameters);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), "SelectMany",
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
        public static IQueryable<TResult> SelectMany<TResult>([NotNull] this IQueryable source, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(selector, nameof(selector));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, selector, args);

            //we have to adjust to lambda to return an IEnumerable<T> instead of whatever the actual property is.
            Type inputType = source.Expression.Type.GetTypeInfo().GetGenericTypeArguments()[0];
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(TResult));
            Type delegateType = typeof(Func<,>).MakeGenericType(inputType, enumerableType);
            lambda = Expression.Lambda(delegateType, lambda.Body, lambda.Parameters);

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), "SelectMany",
                new[] { source.ElementType, typeof(TResult) },
                source.Expression, Expression.Quote(lambda))
            );

            return source.Provider.CreateQuery<TResult>(optimized);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable"/>
        /// and invokes a result selector function on each element therein. The resulting
        /// values from each intermediate sequence are combined into a single, one-dimensional
        /// sequence and returned.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
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
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] string collectionSelector, [NotNull] string resultSelector, [CanBeNull] object[] collectionSelectorArgs = null, [CanBeNull] params object[] resultSelectorArgs)
        {
            return SelectMany(source, collectionSelector, resultSelector, "x", "y", collectionSelectorArgs, resultSelectorArgs);
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable"/>
        /// and invokes a result selector function on each element therein. The resulting
        /// values from each intermediate sequence are combined into a single, one-dimensional
        /// sequence and returned.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
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
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] string collectionSelector, [NotNull] string resultSelector,
            [NotNull] string collectionParameterName, [NotNull] string resultParameterName, [CanBeNull] object[] collectionSelectorArgs = null, [CanBeNull] params object[] resultSelectorArgs)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(collectionSelector, nameof(collectionSelector));
            Check.NotEmpty(collectionParameterName, nameof(collectionParameterName));
            Check.NotEmpty(resultSelector, nameof(resultSelector));
            Check.NotEmpty(resultParameterName, nameof(resultParameterName));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression sourceSelectLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, collectionSelector, collectionSelectorArgs);

            //we have to adjust to lambda to return an IEnumerable<T> instead of whatever the actual property is.
            Type sourceLambdaInputType = source.Expression.Type.GetGenericArguments()[0];
            Type sourceLambdaResultType = sourceSelectLambda.Body.Type.GetGenericArguments()[0];
            Type sourceLambdaEnumerableType = typeof(IEnumerable<>).MakeGenericType(sourceLambdaResultType);
            Type sourceLambdaDelegateType = typeof(Func<,>).MakeGenericType(sourceLambdaInputType, sourceLambdaEnumerableType);

            sourceSelectLambda = Expression.Lambda(sourceLambdaDelegateType, sourceSelectLambda.Body, sourceSelectLambda.Parameters);

            //we have to create additional lambda for result selection
            ParameterExpression xParameter = Expression.Parameter(source.ElementType, collectionParameterName);
            ParameterExpression yParameter = Expression.Parameter(sourceLambdaResultType, resultParameterName);

            LambdaExpression resultSelectLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, new[] { xParameter, yParameter }, null, resultSelector, resultSelectorArgs);
            Type resultLambdaResultType = resultSelectLambda.Body.Type;

            var optimized = OptimizeExpression(Expression.Call(
                typeof(Queryable), "SelectMany",
                new[] { source.ElementType, sourceLambdaResultType, resultLambdaResultType },
                source.Expression, Expression.Quote(sourceSelectLambda), Expression.Quote(resultSelectLambda))
            );

            return source.Provider.CreateQuery(optimized);
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

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), "Single", new[] { source.ElementType }, source.Expression));
            return source.Provider.Execute(optimized);
        }

        private static readonly MethodInfo _singlePredicate = GetMethod(nameof(Queryable.Single), 1);

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if there
        /// is not exactly one element in the sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object Single([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#else
        public static dynamic Single([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_singlePredicate, source, lambda);
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

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), "SingleOrDefault", new[] { source.ElementType }, source.Expression));
            return source.Provider.Execute(optimized);
        }

        private static readonly MethodInfo _singleDefaultPredicate = GetMethod(nameof(Queryable.SingleOrDefault), 1);

        /// <summary>
        /// Returns the only element of a sequence that satisfies a specified condition or a default value if the sequence
        /// is empty; and throws an exception if there is not exactly one element in the sequence.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>The first element in source that passes the test in predicate.</returns>
#if NET35
        public static object SingleOrDefault([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#else
        public static dynamic SingleOrDefault([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return Execute(_singleDefaultPredicate, source, lambda);
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
        private static readonly MethodInfo _skipWhilePredicate = GetMethod(nameof(Queryable.SkipWhile), 1, PredicateParameterHas2);

        /// <summary>
        /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
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
        public static IQueryable SkipWhile([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return CreateQuery(_skipWhilePredicate, source, lambda);
        }
        #endregion SkipWhile

        #region Sum
        /// <summary>
        /// Computes the sum of a sequence of numeric values.
        /// </summary>
        /// <param name="source">A sequence of numeric values to calculate the sum of.</param>
        /// <returns>The sum of the values in the sequence.</returns>
        public static object Sum([NotNull] this IQueryable source)
        {
            Check.NotNull(source, nameof(source));

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), "Sum", null, source.Expression));
            return source.Provider.Execute(optimized);
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
        private static readonly MethodInfo _takeWhilePredicate = GetMethod(nameof(Queryable.TakeWhile), 1, PredicateParameterHas2);

        /// <summary>
        /// Returns elements from a sequence as long as a specified condition is true.
        /// </summary>
        /// <param name="source">A sequence to check for being empty.</param>
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
        public static IQueryable TakeWhile([NotNull] this IQueryable source, [NotNull] string predicate, [CanBeNull] params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(createParameterCtor, source.ElementType, null, predicate, args);

            return CreateQuery(_takeWhilePredicate, source, lambda);
        }
        #endregion TakeWhile

        #region ThenBy
        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
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
        public static IOrderedQueryable<TSource> ThenBy<TSource>([NotNull] this IOrderedQueryable<TSource> source, [NotNull] string ordering, params object[] args)
        {
            return (IOrderedQueryable<TSource>)ThenBy((IOrderedQueryable)source, ordering, args);
        }

        /// <summary>
        /// Performs a subsequent ordering of the elements in a sequence in ascending order according to a key.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
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
        public static IOrderedQueryable ThenBy([NotNull] this IOrderedQueryable source, [NotNull] string ordering, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(ordering, nameof(ordering));

            ParameterExpression[] parameters = { Expression.Parameter(source.ElementType, "") };
            ExpressionParser parser = new ExpressionParser(parameters, ordering, args, null);
            IList<DynamicOrdering> dynamicOrderings = parser.ParseOrdering(forceThenBy: true);

            Expression queryExpr = source.Expression;

            foreach (DynamicOrdering dynamicOrdering in dynamicOrderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable), dynamicOrdering.MethodName,
                    new[] { source.ElementType, dynamicOrdering.Selector.Type },
                    queryExpr, Expression.Quote(Expression.Lambda(dynamicOrdering.Selector, parameters)));
            }

            var optimized = OptimizeExpression(queryExpr);
            return (IOrderedQueryable)source.Provider.CreateQuery(optimized);
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
        public static IQueryable<TSource> Where<TSource>([NotNull] this IQueryable<TSource> source, [CanBeNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            return (IQueryable<TSource>)Where((IQueryable)source, config, predicate, args);
        }

        /// <inheritdoc cref="DynamicQueryableExtensions.Where{TSource}(IQueryable{TSource}, ParsingConfig, string, object[])"/>
        public static IQueryable<TSource> Where<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] string predicate, params object[] args)
        {
            return Where(source, null, predicate, args);
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
        public static IQueryable Where([NotNull] this IQueryable source, [CanBeNull] ParsingConfig config, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpressionParser.ParseLambda(config, createParameterCtor, source.ElementType, null, predicate, args);

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), nameof(Queryable.Where), new[] { source.ElementType }, source.Expression, Expression.Quote(lambda)));
            return source.Provider.CreateQuery(optimized);
        }

        /// <inheritdoc cref="DynamicQueryableExtensions.Where(IQueryable, ParsingConfig, string, object[])"/>
        public static IQueryable Where([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
        {
            return Where(source, null, predicate, args);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> to filter.</param>
        /// <param name="lambda">A cached Lambda Expression.</param>
        public static IQueryable Where([NotNull] this IQueryable source, [NotNull] LambdaExpression lambda)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(lambda, nameof(lambda));

            var optimized = OptimizeExpression(Expression.Call(typeof(Queryable), "Where", new[] { source.ElementType }, source.Expression, Expression.Quote(lambda)));
            return source.Provider.CreateQuery(optimized);
        }
        #endregion

        #region Private Helpers
        private static void CheckOuterAndInnerTypes(bool createParameterCtor, Type outerType, Type innerType, string outerKeySelector, string innerKeySelector, ref LambdaExpression outerSelectorLambda, ref LambdaExpression innerSelectorLambda, params object[] args)
        {
            Type outerSelectorReturnType = outerSelectorLambda.Body.Type;
            Type innerSelectorReturnType = innerSelectorLambda.Body.Type;

            // If types are not the same, try to convert to Nullable and generate new LambdaExpression
            if (outerSelectorReturnType != innerSelectorReturnType)
            {
                //var outerSelectorReturnTypeInfo = outerSelectorReturnType.GetTypeInfo();
                //var innerSelectorReturnTypeInfo = innerSelectorReturnType.GetTypeInfo();
                //if (outerSelectorReturnTypeInfo.BaseType == typeof(DynamicClass) && innerSelectorReturnTypeInfo.BaseType == typeof(DynamicClass))
                //{

                //}

                if (TypeHelper.IsNullableType(outerSelectorReturnType) && !TypeHelper.IsNullableType(innerSelectorReturnType))
                {
                    innerSelectorReturnType = ExpressionParser.ToNullableType(innerSelectorReturnType);
                    innerSelectorLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, innerType, innerSelectorReturnType, innerKeySelector, args);
                }
                else if (!TypeHelper.IsNullableType(outerSelectorReturnType) && TypeHelper.IsNullableType(innerSelectorReturnType))
                {
                    outerSelectorReturnType = ExpressionParser.ToNullableType(outerSelectorReturnType);
                    outerSelectorLambda = DynamicExpressionParser.ParseLambda(createParameterCtor, outerType, outerSelectorReturnType, outerKeySelector, args);
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
            return source.Provider.Execute<TResult>(optimized);
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
            return source.Provider.Execute<TResult>(optimized);
        }

        private static MethodInfo GetMethod<TResult>(string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null) =>
            GetMethod(name, parameterCount, mi => mi.ReturnType == typeof(TResult) && (predicate == null || predicate(mi)));

        private static MethodInfo GetMethod(string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null) =>
            typeof(Queryable).GetTypeInfo().GetDeclaredMethods(name).Single(mi => mi.GetParameters().Length == parameterCount + 1 && (predicate == null || predicate(mi)));
        #endregion Private Helpers
    }
}
