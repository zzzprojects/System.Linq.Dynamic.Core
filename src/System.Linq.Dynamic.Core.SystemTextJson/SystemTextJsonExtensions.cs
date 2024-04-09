using System.Collections.Generic;
using System.Linq.Dynamic.Core.SystemTextJson.Config;
using System.Linq.Dynamic.Core.SystemTextJson.Extensions;
using System.Linq.Dynamic.Core.SystemTextJson.Utils;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using System.Text.Json;

namespace System.Linq.Dynamic.Core.SystemTextJson;

/// <summary>
/// 
/// </summary>
public static class SystemTextJsonExtensions
{
    #region Aggregate
    /// <summary>
    /// Dynamically runs an aggregate function on the <see cref="JsonDocument"/>>.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/>> data source.</param>
    /// <param name="function">The name of the function to run. Can be Sum, Average, Min or Max.</param>
    /// <param name="member">The name of the property to aggregate over.</param>
    /// <returns>The value of the aggregate function run over the specified property.</returns>
    public static object Aggregate(this JsonDocument source, string function, string member)
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
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.</returns>
    public static bool All(this JsonDocument source, string predicate, params object?[] args)
    {
        return All(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Determines whether all the elements of a sequence satisfy a condition.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.</returns>
    public static bool All(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return queryable.All(config, predicate, args);
    }
    #endregion All

    #region Any
    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    public static bool Any(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Any();
    }

    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="config">The <see cref="ParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    public static bool Any(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return queryable.Any(config, predicate, args);
    }

    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    public static bool Any(this JsonDocument source, string predicate, params object?[] args)
    {
        return Any(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="lambda">A Lambda Expression.</param>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    public static bool Any(this JsonDocument source, LambdaExpression lambda)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Any(lambda);
    }
    #endregion Any

    #region Select
    /// <summary>
    /// Projects each element of a sequence into a new form.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="selector">A projection string expression to apply to each element.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
    /// <returns>An <see cref="JsonDocument"/> whose elements are the result of invoking a projection string on each element of source.</returns>
    public static JsonDocument Select(this JsonDocument source, string selector, params object?[] args)
    {
        return Select(source, SystemTextJsonParsingConfig.Default, selector, args);
    }

    /// <summary>
    /// Projects each element of a sequence into a new form.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="selector">A projection string expression to apply to each element.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>An <see cref="JsonElement"/> whose elements are the result of invoking a projection string on each element of source.</returns>
    public static JsonDocument Select(this JsonDocument source, SystemTextJsonParsingConfig config, string selector, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);
        Check.NotNullOrEmpty(selector);

        var queryable = ToQueryable(source, config);
        return ToJsonDocumentArray(() => queryable.Select(config, selector, args));
    }
    #endregion Select

    #region Where
    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="predicate">An expression string to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JsonDocument"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
    public static JsonDocument Where(this JsonDocument source, string predicate, params object?[] args)
    {
        return Where(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">An expression string to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JsonDocument"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
    public static JsonDocument Where(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);
        Check.NotNullOrEmpty(predicate);

        var queryable = ToQueryable(source, config);
        return ToJsonDocumentArray(() => queryable.Where(config, predicate, args));
    }
    #endregion Where

    #region Private Methods
    private static JsonDocument ToJsonDocumentArray(Func<IQueryable> func)
    {
        var array = new List<object>();
        foreach (var dynamicElement in func())
        {
            object element;
            if (dynamicElement is DynamicClass dynamicClass)
            {
                element = JsonElementUtils.FromObject(dynamicClass);
            }
            else
            {
                element = dynamicElement;
            }

            //var element = dynamicElement is DynamicClass dynamicClass ? JsonElementUtils.FromObject(dynamicClass) : dynamicElement;
            array.Add(element);
        }

        return JsonDocumentUtils.FromObject(array);
    }

    private static IQueryable ToQueryable(JsonDocument source, SystemTextJsonParsingConfig? config = null)
    {
        var array = source.RootElement;
        if (array.ValueKind != JsonValueKind.Array)
        {
            throw new NotSupportedException("The source is not a JSON array.");
        }

        return JsonDocumentExtensions.ToDynamicJsonClassArray(array, config?.DynamicJsonClassOptions).AsQueryable();
    }
    #endregion
}