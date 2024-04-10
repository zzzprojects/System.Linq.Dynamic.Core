using System.Collections;
using System.Linq.Dynamic.Core.NewtonsoftJson.Config;
using System.Linq.Dynamic.Core.NewtonsoftJson.Extensions;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;

namespace System.Linq.Dynamic.Core.NewtonsoftJson;

/// <summary>
/// Extension methods for <see cref="JArray"/>.
/// </summary>
public static class NewtonsoftJsonExtensions
{
    #region Aggregate
    /// <summary>
    /// Dynamically runs an aggregate function on the <see cref="JArray"/>>.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/>> data source.</param>
    /// <param name="function">The name of the function to run. Can be Sum, Average, Min or Max.</param>
    /// <param name="member">The name of the property to aggregate over.</param>
    /// <returns>The value of the aggregate function run over the specified property.</returns>
    public static object Aggregate(this JArray source, string function, string member)
    {
        Check.NotNull(source);
        Check.NotEmpty(function);
        Check.NotEmpty(member);

        var queryable = ToQueryable(source);
        return queryable.Aggregate(function, member);
    }
    #endregion Aggregate

    #region All
    /// <summary>
    /// Determines whether all the elements of a sequence satisfy a condition.
    /// </summary>
    /// <param name="source">A sequence whose elements to test for a condition.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.</returns>
    public static bool All(this JArray source, string predicate, params object?[] args)
    {
        return All(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Determines whether all the elements of a sequence satisfy a condition.
    /// </summary>
    /// <param name="source">A sequence whose elements to test for a condition.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.</returns>
    public static bool All(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);
        Check.NotEmpty(predicate);

        var queryable = ToQueryable(source, config);
        return queryable.All(config, predicate, args);
    }
    #endregion All

    #region Any
    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    /// <param name="source">The source <see cref="JArray"/></param>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    public static bool Any(this JArray source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Any();
    }

    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    /// <param name="source">The source <see cref="JArray"/></param>
    /// <param name="config">The <see cref="ParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    public static bool Any(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return queryable.Any(config, predicate, args);
    }

    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    /// <param name="source">The source <see cref="JArray"/></param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    public static bool Any(this JArray source, string predicate, params object?[] args)
    {
        return Any(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    /// <param name="source">The source <see cref="JArray"/></param>
    /// <param name="lambda">A Lambda Expression.</param>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    public static bool Any(this JArray source, LambdaExpression lambda)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Any(lambda);
    }
    #endregion Any

    #region Average
    /// <summary>
    /// Computes the average of a sequence of numeric values.
    /// </summary>
    /// <param name="source">The source <see cref="JArray"/></param>
    /// <returns>The average of the values in the sequence.</returns>
    public static double Average(this JArray source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Average();
    }

    /// <summary>
    /// Computes the average of a sequence of numeric values.
    /// </summary>
    /// <param name="source">The source <see cref="JArray"/></param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
    /// <returns>The average of the values in the sequence.</returns>
    public static double Average(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);
        Check.NotEmpty(predicate);

        var queryable = ToQueryable(source);
        return queryable.Average(config, predicate, args);
    }

    /// <summary>
    /// Computes the average of a sequence of numeric values.
    /// </summary>
    /// <param name="source">The source <see cref="JArray"/></param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
    /// <returns>The average of the values in the sequence.</returns>
    public static double Average(this JArray source, string predicate, params object?[] args)
    {
        return Average(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Computes the average of a sequence of numeric values.
    /// </summary>
    /// <param name="source">The source <see cref="JArray"/></param>
    /// <param name="lambda">A Lambda Expression.</param>
    /// <returns>The average of the values in the sequence.</returns>
    public static double Average(this JArray source, LambdaExpression lambda)
    {
        Check.NotNull(source);
        Check.NotNull(lambda);

        var queryable = ToQueryable(source);
        return queryable.Average(lambda);
    }
    #endregion Average

    #region Cast
    /// <summary>
    /// Converts the elements of an <see cref="JArray"/> to the specified type.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> that contains the elements to be converted.</param>
    /// <param name="type">The type to convert the elements of source to.</param>
    /// <returns>An <see cref="IQueryable"/> that contains each element of the source sequence converted to the specified type.</returns>
    public static IQueryable Cast(this JArray source, Type type)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Cast(type);
    }

    /// <summary>
    /// Converts the elements of an <see cref="JArray"/> to the specified type.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> that contains the elements to be converted.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="typeName">The type to convert the elements of source to.</param>
    /// <returns>An <see cref="JArray"/> that contains each element of the source sequence converted to the specified type.</returns>
    public static IQueryable Cast(this JArray source, NewtonsoftJsonParsingConfig config, string typeName)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return queryable.Cast(typeName);
    }

    /// <summary>
    /// Converts the elements of an <see cref="JArray"/> to the specified type.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> that contains the elements to be converted.</param>
    /// <param name="typeName">The type to convert the elements of source to.</param>
    /// <returns>An <see cref="IQueryable"/> that contains each element of the source sequence converted to the specified type.</returns>
    public static IQueryable Cast(this JArray source, string typeName)
    {
        return Cast(source, NewtonsoftJsonParsingConfig.Default, typeName);
    }
    #endregion Cast

    #region Count
    /// <summary>
    /// Returns the number of elements in a sequence.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> that contains the elements to be counted.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
    public static int Count(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return queryable.Count(config, predicate, args);
    }

    /// <summary>
    /// Returns the number of elements in a sequence.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> that contains the elements to be counted.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
    public static int Count(this JArray source, string predicate, params object?[] args)
    {
        return Count(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Returns the number of elements in a sequence.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> that contains the elements to be counted.</param>
    /// <param name="lambda">A cached Lambda Expression.</param>
    /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
    public static int Count(this JArray source, LambdaExpression lambda)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Count(lambda);
    }
    #endregion Count

    #region DefaultIfEmpty
    /// <summary>
    /// Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return a default value for if empty.</param>
    /// <param name="defaultValue">The value to return if the sequence is empty.</param>
    /// <returns>An <see cref="JArray"/> that contains defaultValue if source is empty; otherwise, source.</returns>
    public static JArray DefaultIfEmpty(this JArray source, object? defaultValue)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJArray(() => queryable.DefaultIfEmpty(defaultValue));
    }
    #endregion

    #region First
    /// <summary>
    /// Returns the first element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the first element of.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken First(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJToken(queryable.First(config, predicate, args));
    }

    /// <summary>
    /// Returns the first element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the first element of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken First(this JArray source, string predicate, params object?[] args)
    {
        return First(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Returns the first element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the first element of.</param>
    /// <param name="lambda">A cached Lambda Expression.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken First(this JArray source, LambdaExpression lambda)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJToken(queryable.First(lambda));
    }
    #endregion First

    #region FirstOrDefault
    /// <summary>
    /// Returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the first element of.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>default if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate.</returns>
    public static JToken? FirstOrDefault(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJToken(queryable.FirstOrDefault(predicate, args));
    }

    /// <summary>
    /// Returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the first element of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>default if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate.</returns>
    public static JToken? FirstOrDefault(this JArray source, string predicate, params object?[] args)
    {
        return FirstOrDefault(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the first element of.</param>
    /// <param name="lambda">A cached Lambda Expression.</param>
    /// <returns>default if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate.</returns>
    public static JToken? FirstOrDefault(this JArray source, LambdaExpression lambda)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJToken(queryable.FirstOrDefault(lambda));
    }
    #endregion FirstOrDefault

    #region Last
    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the last element of.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken Last(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJToken(queryable.Last(predicate, args));
    }

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the last element of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken Last(this JArray source, string predicate, params object?[] args)
    {
        return Last(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the last element of.</param>
    /// <param name="lambda">A cached Lambda Expression.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken Last(this JArray source, LambdaExpression lambda)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJToken(queryable.Last(lambda));
    }
    #endregion Last

    #region LastOrDefault
    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition, or a default value if the sequence contains no elements.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the last element of.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken? LastOrDefault(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJToken(queryable.LastOrDefault(predicate, args));
    }

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition, or a default value if the sequence contains no elements.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the last element of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken? LastOrDefault(this JArray source, string predicate, params object?[] args)
    {
        return LastOrDefault(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition, or a default value if the sequence contains no elements.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the last element of.</param>
    /// <param name="lambda">A cached Lambda Expression.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken? LastOrDefault(this JArray source, LambdaExpression lambda)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJToken(queryable.LastOrDefault(lambda));
    }
    #endregion LastOrDefault

    #region Max
    /// <summary>
    /// Computes the max element of a sequence.
    /// </summary>
    /// <param name="source">A sequence of values to calculate find the max for.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The max element in the sequence.</returns>
    public static JToken Max(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJToken(queryable.Max(config, predicate, args))!;
    }

    /// <summary>
    /// Computes the max element of a sequence.
    /// </summary>
    /// <param name="source">A sequence of values to calculate find the max for.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The max element in the sequence.</returns>
    public static JToken Max(this JArray source, string predicate, params object?[] args)
    {
        return Max(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Computes the max element of a sequence.
    /// </summary>
    /// <param name="source">A sequence of values to calculate find the max for.</param>
    /// <param name="lambda">A Lambda Expression.</param>
    /// <returns>The max element in the sequence.</returns>
    public static JToken Max(this JArray source, LambdaExpression lambda)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJToken(queryable.Max(lambda))!;
    }
    #endregion Max

    #region Min
    /// <summary>
    /// Computes the min element of a sequence.
    /// </summary>
    /// <param name="source">A sequence of values to calculate find the min for.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The min element in the sequence.</returns>
    public static JToken Min(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJToken(queryable.Min(config, predicate, args))!;
    }

    /// <summary>
    /// Computes the min element of a sequence.
    /// </summary>
    /// <param name="source">A sequence of values to calculate find the min for.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The min element in the sequence.</returns>
    public static JToken Min(this JArray source, string predicate, params object?[] args)
    {
        return Min(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Computes the min element of a sequence.
    /// </summary>
    /// <param name="source">A sequence of values to calculate find the min for.</param>
    /// <param name="lambda">A Lambda Expression.</param>
    /// <returns>The min element in the sequence.</returns>
    public static JToken Min(this JArray source, LambdaExpression lambda)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJToken(queryable.Min(lambda))!;
    }
    #endregion Min

    #region OrderBy
    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order according to a key.
    /// </summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="ordering">An expression string to indicate values to order by.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JArray"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
    public static JArray OrderBy(this JArray source, NewtonsoftJsonParsingConfig config, string ordering, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(source);
        Check.NotNullOrEmpty(ordering);

        var queryable = ToQueryable(source, config);
        return ToJArray(() => queryable.OrderBy(config, ordering, args));
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order according to a key.
    /// </summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="ordering">An expression string to indicate values to order by.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JArray"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
    public static JArray OrderBy(this JArray source, string ordering, params object?[] args)
    {
        return OrderBy(source, NewtonsoftJsonParsingConfig.Default, ordering, args);
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order according to a key.
    /// </summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="ordering">An expression string to indicate values to order by.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JArray"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
    public static JArray OrderBy(this JArray source, NewtonsoftJsonParsingConfig config, string ordering, IComparer comparer, params object?[] args)
    {
        var queryable = ToQueryable(source, config);
        return ToJArray(() => queryable.OrderBy(config, ordering, comparer, args));
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order according to a key.
    /// </summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="ordering">An expression string to indicate values to order by.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JArray"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
    public static JArray OrderBy(this JArray source, string ordering, IComparer comparer, params object?[] args)
    {
        return OrderBy(source, NewtonsoftJsonParsingConfig.Default, ordering, comparer, args);
    }
    #endregion OrderBy

    #region Page/PageResult
    /// <summary>
    /// Returns the elements as paged.
    /// </summary>
    /// <param name="source">The source <see cref="JArray"/></param>
    /// <param name="page">The page to return.</param>
    /// <param name="pageSize">The number of elements per page.</param>
    /// <returns>A <see cref="JArray"/> that contains the paged elements.</returns>
    public static JArray Page(this JArray source, int page, int pageSize)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJArray(() => queryable.Page(page, pageSize));
    }

    /// <summary>
    /// Returns the elements as paged and include the CurrentPage, PageCount, PageSize and RowCount.
    /// </summary>
    /// <param name="source">The source <see cref="JArray"/></param>
    /// <param name="page">The page to return.</param>
    /// <param name="pageSize">The number of elements per page.</param>
    /// <param name="rowCount">If this optional parameter has been defined, this value is used as the RowCount instead of executing a Linq `Count()`.</param>
    /// <returns>PagedResult</returns>
    public static PagedResult PageResult(this JArray source, int page, int pageSize, int? rowCount = null)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.PageResult(page, pageSize, rowCount);
    }
    #endregion Page/PageResult

    #region Select
    /// <summary>
    /// Projects each element of a sequence into a new form.
    /// </summary>
    /// <param name="source">A sequence of values to project.</param>
    /// <param name="selector">A projection string expression to apply to each element.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
    /// <returns>An <see cref="JArray"/> whose elements are the result of invoking a projection string on each element of source.</returns>
    public static JArray Select(this JArray source, string selector, params object?[] args)
    {
        return Select(source, NewtonsoftJsonParsingConfig.Default, selector, args);
    }

    /// <summary>
    /// Projects each element of a sequence into a new form.
    /// </summary>
    /// <param name="source">A sequence of values to project.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="selector">A projection string expression to apply to each element.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
    /// <returns>An <see cref="JArray"/> whose elements are the result of invoking a projection string on each element of source.</returns>
    public static JArray Select(this JArray source, NewtonsoftJsonParsingConfig config, string selector, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);
        Check.NotNullOrEmpty(selector);

        if (source.Count == 0)
        {
            return new JArray();
        }

        var queryable = ToQueryable(source, config);
        return ToJArray(() => queryable.Select(config, selector, args));
    }

    /// <summary>
    /// Projects each element of a sequence into a new class of type TResult.
    /// Details see http://solutionizing.net/category/linq/ 
    /// </summary>
    /// <param name="source">The source <see cref="JArray"/></param>
    /// <param name="config">The <see cref="ParsingConfig"/>.</param>
    /// <param name="resultType">The result type.</param>
    /// <param name="selector">A projection string expression to apply to each element.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
    /// <returns>An <see cref="JArray"/> whose elements are the result of invoking a projection string on each element of source.</returns>
    public static JArray Select(this JArray source, NewtonsoftJsonParsingConfig config, Type resultType, string selector, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJArray(() => queryable.Select(config, resultType, selector, args));
    }

    /// <summary>
    /// Projects each element of a sequence into a new class of type TResult.
    /// Details see http://solutionizing.net/category/linq/ 
    /// </summary>
    /// <param name="source">The source <see cref="JArray"/></param>
    /// <param name="resultType">The result type.</param>
    /// <param name="selector">A projection string expression to apply to each element.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
    /// <returns>An <see cref="JArray"/> whose elements are the result of invoking a projection string on each element of source.</returns>
    public static JArray Select(this JArray source, Type resultType, string selector, params object?[] args)
    {
        return Select(source, NewtonsoftJsonParsingConfig.Default, resultType, selector, args);
    }
    #endregion Select

    #region Single
    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if there
    /// is not exactly one element in the sequence.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the last element of.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken Single(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJToken(queryable.Single(predicate, args))!;
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if there
    /// is not exactly one element in the sequence.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the last element of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken Single(this JArray source, string predicate, params object?[] args)
    {
        return Single(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if there
    /// is not exactly one element in the sequence.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the last element of.</param>
    /// <param name="lambda">A cached Lambda Expression.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken Single(this JArray source, LambdaExpression lambda)
    {
        var queryable = ToQueryable(source);
        return ToJToken(queryable.Single(lambda))!;
    }
    #endregion Single

    #region SingleOrDefault
    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition or a default value if the sequence
    /// is empty; and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the last element of.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken? SingleOrDefault(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJToken(queryable.SingleOrDefault(predicate, args));
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition or a default value if the sequence
    /// is empty; and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the last element of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken? SingleOrDefault(this JArray source, string predicate, params object?[] args)
    {
        return SingleOrDefault(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition or a default value if the sequence
    /// is empty; and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    /// <param name="source">The <see cref="JArray"/> to return the last element of.</param>
    /// <param name="lambda">A cached Lambda Expression.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JToken? SingleOrDefault(this JArray source, LambdaExpression lambda)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJToken(queryable.SingleOrDefault(lambda));
    }
    #endregion SingleOrDefault

    #region SkipWhile
    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
    /// </summary>
    /// <param name="source">A <see cref="JArray"/> to return elements from.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>An <see cref="JArray"/> that contains elements from source starting at the first element in the linear series that does not pass the test specified by predicate.</returns>
    public static JArray SkipWhile(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object[]? args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source);
        return ToJArray(() => queryable.SkipWhile(predicate, args));
    }

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
    /// </summary>
    /// <param name="source">A <see cref="JArray"/> to return elements from.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>An <see cref="JArray"/> that contains elements from source starting at the first element in the linear series that does not pass the test specified by predicate.</returns>
    public static JArray SkipWhile(this JArray source, string predicate, params object[]? args)
    {
        return SkipWhile(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }
    #endregion SkipWhile

    #region Where
    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="source">A <see cref="JArray"/> to filter.</param>
    /// <param name="predicate">An expression string to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JArray"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
    public static JArray Where(this JArray source, string predicate, params object?[] args)
    {
        return Where(source, NewtonsoftJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="source">A <see cref="JArray"/> to filter.</param>
    /// <param name="config">The <see cref="NewtonsoftJsonParsingConfig"/>.</param>
    /// <param name="predicate">An expression string to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JArray"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
    public static JArray Where(this JArray source, NewtonsoftJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);
        Check.NotNullOrEmpty(predicate);

        if (source.Count == 0)
        {
            return new JArray();
        }

        var queryable = ToQueryable(source, config);
        return ToJArray(() => queryable.Where(config, predicate, args));
    }
    #endregion Where

    #region Private Methods
    private static JToken? ToJToken(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is JToken jToken)
        {
            return jToken;
        }

        return JToken.FromObject(value);
    }
    private static JArray ToJArray(Func<IQueryable> func)
    {
        var array = new JArray();
        foreach (var dynamicElement in func())
        {
            var element = dynamicElement is DynamicClass dynamicClass ? JObject.FromObject(dynamicClass) : dynamicElement;
            array.Add(element);
        }

        return array;
    }

    private static IQueryable ToQueryable(JArray source, NewtonsoftJsonParsingConfig? config = null)
    {
        return source.ToDynamicJsonClassArray(config?.DynamicJsonClassOptions).AsQueryable();
    }
    #endregion
}