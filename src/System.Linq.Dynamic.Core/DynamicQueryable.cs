using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using ReflectionBridge.Extensions;

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
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similiar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable{T}"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
        /// <example>
        /// <code>
        /// var result1 = list.Where("NumberProperty=1");
        /// var result2 = list.Where("NumberProperty=@0", 1);
        /// var result3 = list.Where("NumberProperty=@0", SomeIntValue);
        /// </code>
        /// </example>
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, string predicate, params object[] args)
        {
            return (IQueryable<TSource>)Where((IQueryable)source, predicate, args);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> to filter.</param>
        /// <param name="predicate">An expression string to test each element for a condition.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similiar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
        /// <example>
        /// <code>
        /// var result1 = list.Where("NumberProperty=1");
        /// var result2 = list.Where("NumberProperty=@0", 1);
        /// var result3 = list.Where("NumberProperty=@0", SomeIntValue);
        /// </code>
        /// </example>
        public static IQueryable Where(this IQueryable source, string predicate, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(predicate, nameof(predicate));

            LambdaExpression lambda = DynamicExpression.ParseLambda(source.ElementType, typeof(bool), predicate, args);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Where",
                    new Type[] { source.ElementType },
                    source.Expression, Expression.Quote(lambda)));
        }

        #endregion

        #region Select

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similiar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> whose elements are the result of invoking a projection string on each element of source.</returns>
        /// <example>
        /// <code>
        /// var singleField = qry.Select("StringProperty");
        /// var dynamicObject = qry.Select("new (StringProperty1, StringProperty2 as OtherStringPropertyName)");
        /// </code>
        /// </example>
        public static IQueryable Select(this IQueryable source, string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(selector, nameof(selector));

            LambdaExpression lambda = DynamicExpression.ParseLambda(source.ElementType, null, selector, args);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Select",
                    new Type[] { source.ElementType, lambda.Body.Type },
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
        public static IQueryable<TResult> Select<TResult>(this IQueryable source, string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(selector, nameof(selector));

            LambdaExpression lambda = DynamicExpression.ParseLambda(source.ElementType, typeof(TResult), selector, args);
            return source.Provider.CreateQuery<TResult>(
                Expression.Call(
                    typeof(Queryable), "Select",
                    new Type[] { source.ElementType, typeof(TResult) },
                    source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// Projects each element of a sequence into a new class of type TResult.
        /// Details see http://solutionizing.net/category/linq/ 
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="type">The result type</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
        /// <returns>An <see cref="IQueryable{TResult}"/> whose elements are the result of invoking a projection string on each element of source.</returns>
        /// <example>
        /// <code>
        /// var users = qry.Select(typeof(User), "new (StringProperty1, StringProperty2 as OtherStringPropertyName)");
        /// </code>
        /// </example>
        public static IQueryable Select(this IQueryable source, Type type, string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(type, nameof(type));
            Check.NotEmpty(selector, nameof(selector));

            LambdaExpression lambda = DynamicExpression.ParseLambda(source.ElementType, type, selector, args);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Select",
                    new Type[] { source.ElementType, type },
                    source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IQueryable"/> and combines the 
        /// resulting sequences into one sequence.
        /// </summary>
        /// <param name="source">A sequence of values to project.</param>
        /// <param name="selector">A projection string expression to apply to each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similiar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> whose elements are the result of invoking a one-to-many projection function on each element of the input sequence.</returns>
        public static IQueryable SelectMany(this IQueryable source, string selector, params object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(selector, nameof(selector));

            LambdaExpression lambda = DynamicExpression.ParseLambda(source.ElementType, null, selector, args);

            //Extra help to get SelectMany to work from StackOverflow Answer
            //http://stackoverflow.com/a/3001674/2465182

            //we have to adjust to lambda to return an IEnumerable<T> instead of whatever the actual property is.
            Type inputType = source.Expression.Type.GetGenericArguments()[0];
            Type resultType = lambda.Body.Type.GetGenericArguments()[0];
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(resultType);
            Type delegateType = typeof(Func<,>).MakeGenericType(inputType, enumerableType);
            lambda = Expression.Lambda(delegateType, lambda.Body, lambda.Parameters);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "SelectMany",
                    new Type[] { source.ElementType, resultType },
                    source.Expression, Expression.Quote(lambda)));
        }

        #endregion

        #region OrderBy

        /// <summary>
        /// Sorts the elements of a sequence in ascending or descending order according to a key.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similiar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable{T}"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        /// <example>
        /// <code>
        /// var result = list.OrderBy("NumberProperty, StringProperty DESC");
        /// </code>
        /// </example>
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string ordering, params object[] args)
        {
            return (IOrderedQueryable<TSource>)OrderBy((IQueryable)source, ordering, args);
        }

        /// <summary>
        /// Sorts the elements of a sequence in ascending or decsending order according to a key.
        /// </summary>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="ordering">An expression string to indicate values to order by.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similiar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
        /// <example>
        /// <code>
        /// var result = list.OrderBy("NumberProperty, StringProperty DESC");
        /// </code>
        /// </example>
        public static IOrderedQueryable OrderBy(this IQueryable source, string ordering, params object[] args)
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
                    new Type[] { source.ElementType, o.Selector.Type },
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
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similiar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> where each element represents a projection over a group and its key.</returns>
        /// <example>
        /// <code>
        /// var groupResult1 = qry.GroupBy("NumberPropertyAsKey", "StringProperty");
        /// var groupResult2 = qry.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)", "new (StringProperty1, StringProperty2)");
        /// </code>
        /// </example>
        public static IQueryable GroupBy(this IQueryable source, string keySelector, string resultSelector, object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(keySelector, nameof(keySelector));
            Check.NotEmpty(resultSelector, nameof(resultSelector));

            LambdaExpression keyLambda = DynamicExpression.ParseLambda(source.ElementType, null, keySelector, args);
            LambdaExpression elementLambda = DynamicExpression.ParseLambda(source.ElementType, null, resultSelector, args);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "GroupBy",
                    new Type[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
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
        public static IQueryable GroupBy(this IQueryable source, string keySelector, string resultSelector)
        {
            return GroupBy(source, keySelector, resultSelector, (object[])null);
        }


        /// <summary>
        /// Groups the elements of a sequence according to a specified key string function 
        /// and creates a result value from each group and its key.
        /// </summary>
        /// <param name="source">A <see cref="IQueryable"/> whose elements to group.</param>
        /// <param name="keySelector">A string expression to specify the key for each element.</param>
        /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similiar to the way String.Format formats strings.</param>
        /// <returns>A <see cref="IQueryable"/> where each element represents a projection over a group and its key.</returns>
        /// <example>
        /// <code>
        /// var groupResult1 = qry.GroupBy("NumberPropertyAsKey");
        /// var groupResult2 = qry.GroupBy("new (NumberPropertyAsKey, StringPropertyAsKey)");
        /// </code>
        /// </example>
        public static IQueryable GroupBy(this IQueryable source, string keySelector, object[] args)
        {
            Check.NotNull(source, nameof(source));
            Check.NotEmpty(keySelector, nameof(keySelector));

            LambdaExpression keyLambda = DynamicExpression.ParseLambda(source.ElementType, null, keySelector, args);

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "GroupBy",
                    new Type[] { source.ElementType, keyLambda.Body.Type },
                    source.Expression, Expression.Quote(keyLambda)));
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
        public static IQueryable GroupBy(this IQueryable source, string keySelector)
        {
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
        public static IEnumerable<GroupResult> GroupByMany<TElement>(this IEnumerable<TElement> source, params string[] keySelectors)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelectors, nameof(keySelectors));

            var selectors = new List<Func<TElement, object>>(keySelectors.Length);

            foreach (var selector in keySelectors)
            {
                LambdaExpression l = DynamicExpression.ParseLambda(typeof(TElement), typeof(object), selector);
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
        public static IEnumerable<GroupResult> GroupByMany<TElement>(this IEnumerable<TElement> source, params Func<TElement, object>[] keySelectors)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(keySelectors, nameof(keySelectors));

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
        /// <param name="args">An object array that contains zero or more objects to insert into the predicates as parameters.  Similiar to the way String.Format formats strings.</param>
        /// <returns>An <see cref="IQueryable"/> obtained by performing an inner join on two sequences.</returns>
        public static IQueryable Join(this IQueryable outer, IEnumerable inner, string outerKeySelector, string innerKeySelector, string resultSelector, params object[] args)
        {
            //http://stackoverflow.com/questions/389094/how-to-create-a-dynamic-linq-join-extension-method

            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotEmpty(outerKeySelector, nameof(outerKeySelector));
            Check.NotEmpty(innerKeySelector, nameof(innerKeySelector));
            Check.NotEmpty(resultSelector, nameof(resultSelector));

            LambdaExpression outerSelectorLambda = DynamicExpression.ParseLambda(outer.ElementType, null, outerKeySelector, args);
            LambdaExpression innerSelectorLambda = DynamicExpression.ParseLambda(inner.AsQueryable().ElementType, null, innerKeySelector, args);

            ParameterExpression[] parameters = new ParameterExpression[]
            {
                Expression.Parameter(outer.ElementType, "outer"), Expression.Parameter(inner.AsQueryable().ElementType, "inner")
            };

            LambdaExpression resultsSelectorLambda = DynamicExpression.ParseLambda(parameters, null, resultSelector, args);

            return outer.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable), "Join",
                    new Type[] { outer.ElementType, inner.AsQueryable().ElementType, outerSelectorLambda.Body.Type, resultsSelectorLambda.Body.Type },
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
        /// <param name="args">An object array that contains zero or more objects to insert into the predicates as parameters.  Similiar to the way String.Format formats strings.</param>
        /// <remarks>This overload only works on elements where both sequences and the resulting element match.</remarks>
        /// <returns>An <see cref="IQueryable{T}"/> that has elements of type TResult obtained by performing an inner join on two sequences.</returns>
        public static IQueryable<TElement> Join<TElement>(this IQueryable<TElement> outer, IEnumerable<TElement> inner, string outerKeySelector, string innerKeySelector, string resultSelector, params object[] args)
        {
            return (IQueryable<TElement>)Join((IQueryable)outer, (IEnumerable)inner, outerKeySelector, innerKeySelector, resultSelector, args);
        }

        #endregion
    }
}