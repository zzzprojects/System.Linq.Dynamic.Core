using System.Linq.Dynamic.Core.SystemTextJson.Config;
using System.Linq.Dynamic.Core.SystemTextJson.Extensions;
using System.Linq.Dynamic.Core.Validation;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace System.Linq.Dynamic.Core.SystemTextJson;

/// <summary>
/// 
/// </summary>
public static class JsonExtensions
{
    #region All
    /// <summary>Determines whether all the elements of a sequence satisfy a condition.</summary>
    /// <param name="source">A sequence whose elements to test for a condition.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.</returns>
    public static bool All(this JsonDocument source, string predicate, params object?[] args)
    {
        return All(source, JsonParsingConfig.Default, predicate, args);
    }

    /// <summary>Determines whether all the elements of a sequence satisfy a condition.</summary>
    /// <param name="source">A sequence whose elements to test for a condition.</param>
    /// <param name="config">The <see cref="JsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>true if every element of the source sequence passes the test in the specified predicate, or if the sequence is empty; otherwise, false.</returns>
    public static bool All(this JsonDocument source, JsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);
        Check.NotEmpty(predicate);

        var queryable = ConvertToQueryable(source, config);
        return queryable.All(config, predicate, args);
    }
    #endregion All

    #region Select
    /// <summary>
    /// Projects each element of a sequence into a new form.
    /// </summary>
    /// <param name="source">A sequence of values to project.</param>
    /// <param name="selector">A projection string expression to apply to each element.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
    /// <returns>An <see cref="JsonDocument"/> whose elements are the result of invoking a projection string on each element of source.</returns>
    public static JsonDocument Select(this JsonDocument source, string selector, params object?[] args)
    {
        return Select(source, JsonParsingConfig.Default, selector, args);
    }

    /// <summary>
    /// Projects each element of a sequence into a new form.
    /// </summary>
    /// <param name="source">A sequence of values to project.</param>
    /// <param name="config">The <see cref="JsonParsingConfig"/>.</param>
    /// <param name="selector">A projection string expression to apply to each element.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.  Similar to the way String.Format formats strings.</param>
    /// <returns>An <see cref="JArray"/> whose elements are the result of invoking a projection string on each element of source.</returns>
    public static JsonDocument Select(this JsonDocument source, JsonParsingConfig config, string selector, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);
        Check.NotNullOrEmpty(selector);
        
        var queryable = ConvertToQueryable(source, config);
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
    public static JsonDocument Where(this JsonDocument source, string predicate, params object?[] args)
    {
        return Where(source, JsonParsingConfig.Default, predicate, args);
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="source">A <see cref="JArray"/> to filter.</param>
    /// <param name="config">The <see cref="JsonParsingConfig"/>.</param>
    /// <param name="predicate">An expression string to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JArray"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
    public static JsonDocument Where(this JsonDocument source, JsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);
        Check.NotNullOrEmpty(predicate);

        var queryable = ConvertToQueryable(source, config);
        return ToJArray(() => queryable.Where(config, predicate, args));
    }
    #endregion Where

    /// <summary>
    /// Convert the dynamic results to a JArray.
    /// </summary>
    /// <param name="func">The callback which returns a <see cref="IQueryable"/>.</param>
    /// <returns><see cref="JArray"/></returns>
    private static JsonElement ToJArray(Func<IQueryable> func)
    {
        var array = new JsonElement();
        foreach (var dynamicElement in func())
        {

            var element = dynamicElement is DynamicClass dynamicClass ? JsonDocument..FromObject(dynamicClass) : dynamicElement;
            //array.Add(element);
            TryAddPropertyToArrayElements(array, "element", dynamicElement);
        }
        return array;
    }

    private static IQueryable ConvertToQueryable(JsonElement source, JsonParsingConfig config)
    {
        if (source.ValueKind != JsonValueKind.Array)
        {
            throw new NotSupportedException("The source is not a JSON array.");
        }

        return source.ToDynamicJsonClassArray(config.DynamicJsonClassOptions).AsQueryable();
    }

    private static JsonNode TryAddPropertyToArrayElements<TProperty>(this JsonNode node, string name, TProperty value)
    {
        if (node is JsonArray array)
        {
            foreach (var obj in array.OfType<JsonObject>())
            {
                obj[name] = JsonSerializer.SerializeToNode(value);
            }
        }

        return node;
    }

    private static JsonElement TryAddPropertyToArrayElements<TProperty>(this JsonElement element, string name, TProperty value)
    {
        return element.ValueKind == JsonValueKind.Array
            ? JsonSerializer.SerializeToElement(element.Deserialize<JsonNode>()!.TryAddPropertyToArrayElements(name, value))
            : element;
    }

    private static object? TryAddPropertyToArrayElements<TProperty>(this object? obj, string name, TProperty value) =>
        obj switch
        {
            JsonElement e => e.TryAddPropertyToArrayElements(name, value),
            JsonNode n => n.TryAddPropertyToArrayElements(name, value),
            null => null, // JSON values that are null are deserialized to the c# null value, not some element or node of type null
            _ => throw new ArgumentException("Unexpected type ${obj}"),
        };
}