using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using JetBrains.Annotations;
using System.Linq.Dynamic.Core.Extensions;
using System.Reflection;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Provides a set of static (Shared in Visual Basic) methods for querying data 
    /// structures that implement <see cref="IQueryable"/>. It allows dynamic string based querying. 
    /// Very handy when, at compile time, you don't know the type of queries that will be generated, 
    /// or when downstream components only return column names to sort and filter by.
    /// </summary>
    public static class DynamicQueryable
    {
        #region Where

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A <see cref="IQueryable{T}"/> to filter.</param>
        /// <param name="predicate">An expression string to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable{T}"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
        /// <example>
        /// <code>
        /// var result1 = list.Where("NumberProperty=1");
        /// var result2 = list.Where("NumberProperty=@0", 1);
        /// var result3 = list.Where("NumberProperty=@0", SomeIntValue);
        /// </code>
        /// </example>
        public static IQueryable<TSource> Where<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            return (IQueryable<TSource>)Where((IQueryable)source, predicate, args);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> to filter.</param>
        /// <param name="predicate">An expression string to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
        /// <example>
        /// <code>
        /// var result1 = list.Where("NumberProperty=1");
        /// var result2 = list.Where("NumberProperty=@0", 1);
        /// var result3 = list.Where("NumberProperty=@0", SomeIntValue);
        /// </code>
        /// </example>
        public static IQueryable Where([NotNull] this IQueryable source, [NotNull] string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpression.ParseLambda(createParameterCtor, source.ElementType, typeof(bool), predicate, args);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Where",
                    new[] { source.ElementType },
                    source.Expression, Expression.Quote(lambda)));
        }

        #endregion

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
        /// var singleField = qry.Select("StringProperty");
        /// var dynamicObject = qry.Select("new (StringProperty1, StringProperty2 as OtherStringPropertyName)");
        /// </code>
        /// </example>
        public static IQueryable Select([NotNull] this IQueryable source, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(selector, nameof(selector));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpression.ParseLambda(createParameterCtor, source.ElementType, null, selector, args);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Select",
                    new[] { source.ElementType, lambda.Body.Type },
                    source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// Projects each element of a sequence into a new class of type TResult.
        /// Details see http://solutionizing.net/category/linq/ 
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
        /// <returns>An <see cref="IQueryable{TResult}"/> whose elements are the result of invoking a projection string on each element of source.</returns>
        /// <example>
        /// <code>
        /// var users = qry.Select&lt;User&gt;("new (StringProperty1, StringProperty2 as OtherStringPropertyName)");
        /// </code>
        /// </example>
        public static IQueryable<TResult> Select<TResult>([NotNull] this IQueryable source, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(selector, nameof(selector));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpression.ParseLambda(createParameterCtor, source.ElementType, typeof(TResult), selector, args);

            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    typeof(Queryable), "Select",
                    new[] { source.ElementType, typeof(TResult) },
                    source.Expression, Expression.Quote(lambda)));
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
        /// var users = qry.Select(typeof(User), "new (StringProperty1, StringProperty2 as OtherStringPropertyName)");
        /// </code>
        /// </example>
        public static IQueryable Select([NotNull] this IQueryable source, [NotNull] Type resultType, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(resultType, nameof(resultType));
            Check.NotEmpty(selector, nameof(selector));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpression.ParseLambda(createParameterCtor, source.ElementType, resultType, selector, args);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Select",
                    new[] { source.ElementType, resultType },
                    source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable"/> and combines the 
        /// resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> whose elements are the result of invoking a one-to-many projection function on each element of the input sequence.</returns>
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
            LambdaExpression lambda = DynamicExpression.ParseLambda(createParameterCtor, source.ElementType, null, selector, args);

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

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "SelectMany",
                    new[] { source.ElementType, resultType },
                    source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable{TResult}"/> and combines the resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable{TResult}"/> whose elements are the result of invoking a one-to-many projection function on each element of the input sequence.</returns>
        public static IQueryable<TResult> SelectMany<TResult>([NotNull] this IQueryable source, [NotNull] string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(selector, nameof(selector));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression lambda = DynamicExpression.ParseLambda(createParameterCtor, source.ElementType, null, selector, args);

            //we have to adjust to lambda to return an IEnumerable<T> instead of whatever the actual property is.
            Type inputType = source.Expression.Type.GetTypeInfo().GetGenericTypeArguments()[0];
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(typeof(TResult));
            Type delegateType = typeof(Func<,>).MakeGenericType(inputType, enumerableType);
            lambda = Expression.Lambda(delegateType, lambda.Body, lambda.Parameters);

            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    typeof(Queryable), "SelectMany",
                    new[] { source.ElementType, typeof(TResult) },
                    source.Expression, Expression.Quote(lambda)));
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
        public static IQueryable SelectMany([NotNull] this IQueryable source, [NotNull] string collectionSelector, [NotNull] string resultSelector,
            [NotNull] string collectionParameterName, [NotNull] string resultParameterName, [CanBeNull] object[] collectionSelectorArgs = null, [CanBeNull] params object[] resultSelectorArgs)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(collectionSelector, nameof(collectionSelector));
            Check.NotEmpty(collectionParameterName, nameof(collectionParameterName));
            Check.NotEmpty(resultSelector, nameof(resultSelector));
            Check.NotEmpty(resultParameterName, nameof(resultParameterName));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression sourceSelectLambda = DynamicExpression.ParseLambda(createParameterCtor, source.ElementType, null, collectionSelector, collectionSelectorArgs);

            //we have to adjust to lambda to return an IEnumerable<T> instead of whatever the actual property is.
            Type sourceLambdaInputType = source.Expression.Type.GetGenericArguments()[0];
            Type sourceLambdaResultType = sourceSelectLambda.Body.Type.GetGenericArguments()[0];
            Type sourceLambdaEnumerableType = typeof(IEnumerable<>).MakeGenericType(sourceLambdaResultType);
            Type sourceLambdaDelegateType = typeof(Func<,>).MakeGenericType(sourceLambdaInputType, sourceLambdaEnumerableType);

            sourceSelectLambda = Expression.Lambda(sourceLambdaDelegateType, sourceSelectLambda.Body, sourceSelectLambda.Parameters);

            //we have to create additional lambda for result selection
            ParameterExpression xParameter = Expression.Parameter(source.ElementType, collectionParameterName);
            ParameterExpression yParameter = Expression.Parameter(sourceLambdaResultType, resultParameterName);

            LambdaExpression resultSelectLambda = DynamicExpression.ParseLambda(createParameterCtor, new[] { xParameter, yParameter }, null, resultSelector, resultSelectorArgs);
            Type resultLambdaResultType = resultSelectLambda.Body.Type;

            return source.Provider.CreateQuery(
                Expression.Call(
                typeof(Queryable), "SelectMany",
                new[] { source.ElementType, sourceLambdaResultType, resultLambdaResultType },
                source.Expression, Expression.Quote(sourceSelectLambda), Expression.Quote(resultSelectLambda)));
        }

        #endregion

        #region OrderBy

        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable{T}"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        /// <example>
        /// <code>
        /// var result = list.OrderBy("NumberProperty, StringProperty DESC");
        /// </code>
        /// </example>
        public static IOrderedQueryable<TSource> OrderBy<TSource>([NotNull] this IQueryable<TSource> source, [NotNull] string ordering, params object[] args)
        {
            return (IOrderedQueryable<TSource>)OrderBy((IQueryable)source, ordering, args);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending or decsending order according to a key.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        /// <example>
        /// <code>
        /// var result = list.OrderBy("NumberProperty, StringProperty DESC");
        /// </code>
        /// </example>
        public static IOrderedQueryable OrderBy([NotNull] this IQueryable source, [NotNull] string ordering, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(ordering, nameof(ordering));

            ParameterExpression[] parameters = { Expression.Parameter(source.ElementType, "") };
            ExpressionParser parser = new ExpressionParser(parameters, ordering, args);
            IEnumerable<DynamicOrdering> orderings = parser.ParseOrdering();
            Expression queryExpr = source.Expression;
            string methodAsc = "OrderBy";
            string methodDesc = "OrderByDescending";
            foreach (DynamicOrdering o in orderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable), o.Ascending ? methodAsc : methodDesc,
                    new[] { source.ElementType, o.Selector.Type },
                    queryExpr, Expression.Quote(Expression.Lambda(o.Selector, parameters)));
                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }
            return (IOrderedQueryable)source.Provider.CreateQuery(queryExpr);
        }

        #endregion

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
        /// var groupResult1 = qry.GroupBy("NumberPropertyAsKey", "StringProperty");
        /// var groupResult2 = qry.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)", "new (StringProperty1, StringProperty2)");
        /// </code>
        /// </example>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] string keySelector, [NotNull] string resultSelector, object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(keySelector, nameof(keySelector));
            Check.NotEmpty(resultSelector, nameof(resultSelector));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression keyLambda = DynamicExpression.ParseLambda(createParameterCtor, source.ElementType, null, keySelector, args);
            LambdaExpression elementLambda = DynamicExpression.ParseLambda(createParameterCtor, source.ElementType, null, resultSelector, args);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "GroupBy",
                    new[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
                    source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)));
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
        /// var groupResult1 = qry.GroupBy("NumberPropertyAsKey", "StringProperty");
        /// var groupResult2 = qry.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)", "new (StringProperty1, StringProperty2)");
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
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> where each element represents a projection over a group and its key.</returns>
        /// <example>
        /// <code>
        /// var groupResult1 = qry.GroupBy("NumberPropertyAsKey");
        /// var groupResult2 = qry.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)");
        /// </code>
        /// </example>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] string keySelector, object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(keySelector, nameof(keySelector));

            bool createParameterCtor = source.IsLinqToObjects();
            LambdaExpression keyLambda = DynamicExpression.ParseLambda(createParameterCtor, source.ElementType, null, keySelector, args);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "GroupBy",
                    new[] { source.ElementType, keyLambda.Body.Type },
                    new[] { source.Expression, Expression.Quote(keyLambda) }));
        }

        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function 
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> whose elements to group.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <returns>A <see cref="IQueryable"/> where each element represents a projection over a group and its key.</returns>
        /// <example>
        /// <code>
        /// var groupResult1 = qry.GroupBy("NumberPropertyAsKey");
        /// var groupResult2 = qry.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)");
        /// </code>
        /// </example>
        public static IQueryable GroupBy([NotNull] this IQueryable source, [NotNull] string keySelector)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(keySelector, nameof(keySelector));

            return GroupBy(source, keySelector, (object[])null);
        }
        #endregion

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
                LambdaExpression l = DynamicExpression.ParseLambda(createParameterCtor, typeof(TElement), typeof(object), selector);
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

            bool createParameterCtor = outer.IsLinqToObjects();
            LambdaExpression outerSelectorLambda = DynamicExpression.ParseLambda(createParameterCtor, outer.ElementType, null, outerKeySelector, args);
            LambdaExpression innerSelectorLambda = DynamicExpression.ParseLambda(createParameterCtor, inner.AsQueryable().ElementType, null, innerKeySelector, args);

            ParameterExpression[] parameters = new[]
            {
                Expression.Parameter(outer.ElementType, "outer"), Expression.Parameter(inner.AsQueryable().ElementType, "inner")
            };

            LambdaExpression resultsSelectorLambda = DynamicExpression.ParseLambda(createParameterCtor, parameters, null, resultSelector, args);

            return outer.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Join",
                    new[] { outer.ElementType, inner.AsQueryable().ElementType, outerSelectorLambda.Body.Type, resultsSelectorLambda.Body.Type },
                    outer.Expression, inner.AsQueryable().Expression, Expression.Quote(outerSelectorLambda), Expression.Quote(innerSelectorLambda), Expression.Quote(resultsSelectorLambda)));
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

        #endregion
    }
}
