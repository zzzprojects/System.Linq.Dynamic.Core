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
    /// <returns>The number of elements in the input sequence.</returns>
    public static int Count(this JArray source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Count();
    }

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

        var queryable = ToQueryable(source);
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
    #endregion Select

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
}