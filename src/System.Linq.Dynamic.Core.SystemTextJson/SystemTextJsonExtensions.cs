using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.SystemTextJson.Config;
using System.Linq.Dynamic.Core.SystemTextJson.Extensions;
using System.Linq.Dynamic.Core.SystemTextJson.Utils;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using System.Text.Json;

namespace System.Linq.Dynamic.Core.SystemTextJson;

/// <summary>
/// Extension methods for <see cref="JsonDocument"/>.
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
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
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
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
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
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
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
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    public static bool Any(this JsonDocument source, string predicate, params object?[] args)
    {
        return Any(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion Any

    #region Average
    /// <summary>
    /// Computes the average of a sequence of numeric values.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <returns>The average of the values in the sequence.</returns>
    public static double Average(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Average();
    }

    /// <summary>
    /// Computes the average of a sequence of numeric values.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
    /// <returns>The average of the values in the sequence.</returns>
    public static double Average(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);
        Check.NotEmpty(predicate);

        var queryable = ToQueryable(source, config);
        return queryable.Average(config, predicate, args);
    }

    /// <summary>
    /// Computes the average of a sequence of numeric values.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
    /// <returns>The average of the values in the sequence.</returns>
    public static double Average(this JsonDocument source, string predicate, params object?[] args)
    {
        return Average(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion Average

    #region Cast
    /// <summary>
    /// Converts the elements of an <see cref="JsonDocument"/> to the specified type.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> that contains the elements to be converted.</param>
    /// <param name="type">The type to convert the elements of source to.</param>
    /// <returns>An <see cref="IQueryable"/> that contains each element of the source sequence converted to the specified type.</returns>
    public static IQueryable Cast(this JsonDocument source, Type type)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Cast(type);
    }

    /// <summary>
    /// Converts the elements of an <see cref="JsonDocument"/> to the specified type.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> that contains the elements to be converted.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="typeName">The type to convert the elements of source to.</param>
    /// <returns>An <see cref="JsonDocument"/> that contains each element of the source sequence converted to the specified type.</returns>
    public static IQueryable Cast(this JsonDocument source, SystemTextJsonParsingConfig config, string typeName)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return queryable.Cast(typeName);
    }

    /// <summary>
    /// Converts the elements of an <see cref="JsonDocument"/> to the specified type.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> that contains the elements to be converted.</param>
    /// <param name="typeName">The type to convert the elements of source to.</param>
    /// <returns>An <see cref="IQueryable"/> that contains each element of the source sequence converted to the specified type.</returns>
    public static IQueryable Cast(this JsonDocument source, string typeName)
    {
        return Cast(source, SystemTextJsonParsingConfig.Default, typeName);
    }
    #endregion Cast

    #region Count
    /// <summary>
    /// Returns the number of elements in a sequence.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> that contains the elements to be counted.</param>
    /// <returns>The number of elements in the input sequence.</returns>
    public static int Count(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Count();
    }

    /// <summary>
    /// Returns the number of elements in a sequence.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> that contains the elements to be counted.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
    public static int Count(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return queryable.Count(config, predicate, args);
    }

    /// <summary>
    /// Returns the number of elements in a sequence.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> that contains the elements to be counted.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The number of elements in the specified sequence that satisfies a condition.</returns>
    public static int Count(this JsonDocument source, string predicate, params object?[] args)
    {
        return Count(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion Count

    #region DefaultIfEmpty
    /// <summary>
    /// Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return a default value for if empty.</param>
    /// <returns>An <see cref="JsonDocument"/> that contains default if source is empty; otherwise, source.</returns>
    public static JsonDocument DefaultIfEmpty(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonDocumentArray(queryable.DefaultIfEmpty);
    }

    /// <summary>
    /// Returns the elements of the specified sequence or the type parameter's default value in a singleton collection if the sequence is empty.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return a default value for if empty.</param>
    /// <param name="defaultValue">The value to return if the sequence is empty.</param>
    /// <returns>An <see cref="JsonDocument"/> that contains defaultValue if source is empty; otherwise, source.</returns>
    public static JsonDocument DefaultIfEmpty(this JsonDocument source, object? defaultValue)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonDocumentArray(() => queryable.DefaultIfEmpty(defaultValue));
    }
    #endregion

    #region Distinct
    /// <summary>
    /// Returns distinct elements from a sequence by using the default equality comparer to compare values.
    /// </summary>
    /// <param name="source">The sequence to remove duplicate elements from.</param>
    /// <returns>An <see cref="JsonDocument"/> that contains distinct elements from the source sequence.</returns>
    public static JsonDocument Distinct(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonDocumentArray(queryable.Distinct);
    }
    #endregion Distinct

    #region First
    /// <summary>
    /// Returns the first element of a sequence.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the first element of.</param>
    /// <returns>The first element in source.</returns>
    public static JsonElement First(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonElement(queryable.First()) ?? throw new InvalidOperationException(Res.SequenceContainsNoElements);
    }

    /// <summary>
    /// Returns the first element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the first element of.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JsonElement First(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJsonElement(queryable.First(config, predicate, args)) ?? throw new InvalidOperationException(Res.SequenceContainsNoElements);
    }

    /// <summary>
    /// Returns the first element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the first element of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JsonElement First(this JsonDocument source, string predicate, params object?[] args)
    {
        return First(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion First

    #region FirstOrDefault
    /// <summary>
    /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    /// <param name="source">The <see cref="IQueryable"/> to return the first element of.</param>
    /// <returns>default if source is empty; otherwise, the first element in source.</returns>
    public static JsonElement? FirstOrDefault(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonElement(queryable.FirstOrDefault());
    }

    /// <summary>
    /// Returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the first element of.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>default if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate.</returns>
    public static JsonElement? FirstOrDefault(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJsonElement(queryable.FirstOrDefault(predicate, args));
    }

    /// <summary>
    /// Returns the first element of a sequence that satisfies a specified condition or a default value if no such element is found.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the first element of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>default if source is empty or if no element passes the test specified by predicate; otherwise, the first element in source that passes the test specified by predicate.</returns>
    public static JsonElement? FirstOrDefault(this JsonDocument source, string predicate, params object?[] args)
    {
        return FirstOrDefault(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion FirstOrDefault

    #region Last
    /// <summary>
    /// Returns the last element of a sequence.
    /// </summary>
    /// <param name="source">The <see cref="IQueryable"/> to return the last element of.</param>
    /// <returns>The last element in source.</returns>
    public static JsonElement Last(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonElement(queryable.Last()) ?? throw new InvalidOperationException(Res.SequenceContainsNoElements);
    }

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the last element of.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JsonElement Last(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJsonElement(queryable.Last(predicate, args)) ?? throw new InvalidOperationException(Res.SequenceContainsNoElements);
    }

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the last element of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JsonElement Last(this JsonDocument source, string predicate, params object?[] args)
    {
        return Last(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion Last

    #region LastOrDefault
    /// <summary>
    /// Returns the last element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the last element of.</param>
    /// <returns>default if source is empty; otherwise, the last element in source.</returns>
    public static JsonElement? LastOrDefault(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonElement(queryable.LastOrDefault());
    }

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition, or a default value if the sequence contains no elements.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the last element of.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JsonElement? LastOrDefault(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJsonElement(queryable.LastOrDefault(config, predicate, args));
    }

    /// <summary>
    /// Returns the last element of a sequence that satisfies a specified condition, or a default value if the sequence contains no elements.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the last element of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JsonElement? LastOrDefault(this JsonDocument source, string predicate, params object?[] args)
    {
        return LastOrDefault(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion LastOrDefault

    #region Max
    /// <summary>
    /// Computes the max element of a sequence.
    /// </summary>
    /// <param name="source">A sequence of values to calculate find the max for.</param>
    /// <returns>The max element in the sequence.</returns>
    public static object Max(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Max();
    }

    /// <summary>
    /// Computes the max element of a sequence.
    /// </summary>
    /// <param name="source">A sequence of values to calculate find the max for.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The max element in the sequence.</returns>
    public static object Max(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return queryable.Max(config, predicate, args);
    }

    /// <summary>
    /// Computes the max element of a sequence.
    /// </summary>
    /// <param name="source">A sequence of values to calculate find the max for.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The max element in the sequence.</returns>
    public static object Max(this JsonDocument source, string predicate, params object?[] args)
    {
        return Max(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion Max

    #region Min
    /// <summary>
    /// Computes the min element of a sequence.
    /// </summary>
    /// <param name="source">A sequence of values to calculate find the min for.</param>
    /// <returns>The min element in the sequence.</returns>
    public static object Min(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Min();
    }

    /// <summary>
    /// Computes the min element of a sequence.
    /// </summary>
    /// <param name="source">A sequence of values to calculate find the min for.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The min element in the sequence.</returns>
    public static object Min(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return queryable.Min(config, predicate, args);
    }

    /// <summary>
    /// Computes the min element of a sequence.
    /// </summary>
    /// <param name="source">A sequence of values to calculate find the min for.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The min element in the sequence.</returns>
    public static object Min(this JsonDocument source, string predicate, params object?[] args)
    {
        return Min(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion Min

    #region OrderBy
    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order according to a key.
    /// </summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="ordering">An expression string to indicate values to order by.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JsonDocument"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
    public static JsonDocument OrderBy(this JsonDocument source, SystemTextJsonParsingConfig config, string ordering, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(source);
        Check.NotNullOrEmpty(ordering);

        var queryable = ToQueryable(source, config);
        return ToJsonDocumentArray(() => queryable.OrderBy(config, ordering, args));
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order according to a key.
    /// </summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="ordering">An expression string to indicate values to order by.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JsonDocument"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
    public static JsonDocument OrderBy(this JsonDocument source, string ordering, params object?[] args)
    {
        return OrderBy(source, SystemTextJsonParsingConfig.Default, ordering, args);
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order according to a key.
    /// </summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="ordering">An expression string to indicate values to order by.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JsonDocument"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
    public static JsonDocument OrderBy(this JsonDocument source, SystemTextJsonParsingConfig config, string ordering, IComparer comparer, params object?[] args)
    {
        var queryable = ToQueryable(source, config);
        return ToJsonDocumentArray(() => queryable.OrderBy(config, ordering, comparer, args));
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending or descending order according to a key.
    /// </summary>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="ordering">An expression string to indicate values to order by.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>A <see cref="JsonDocument"/> whose elements are sorted according to the specified <paramref name="ordering"/>.</returns>
    public static JsonDocument OrderBy(this JsonDocument source, string ordering, IComparer comparer, params object?[] args)
    {
        return OrderBy(source, SystemTextJsonParsingConfig.Default, ordering, comparer, args);
    }
    #endregion OrderBy

    #region Page/PageResult
    /// <summary>
    /// Returns the elements as paged.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="page">The page to return.</param>
    /// <param name="pageSize">The number of elements per page.</param>
    /// <returns>A <see cref="JsonDocument"/> that contains the paged elements.</returns>
    public static JsonDocument Page(this JsonDocument source, int page, int pageSize)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonDocumentArray(() => queryable.Page(page, pageSize));
    }

    /// <summary>
    /// Returns the elements as paged and include the CurrentPage, PageCount, PageSize and RowCount.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="page">The page to return.</param>
    /// <param name="pageSize">The number of elements per page.</param>
    /// <param name="rowCount">If this optional parameter has been defined, this value is used as the RowCount instead of executing a Linq `Count()`.</param>
    /// <returns>PagedResult</returns>
    public static PagedResult PageResult(this JsonDocument source, int page, int pageSize, int? rowCount = null)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.PageResult(page, pageSize, rowCount);
    }
    #endregion Page/PageResult

    #region Reverse
    /// <summary>
    /// Inverts the order of the elements in a sequence.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <returns>A <see cref="JsonDocument"/> whose elements correspond to those of the input sequence in reverse order.</returns>
    public static JsonDocument Reverse(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonDocumentArray(() => queryable.Reverse());
    }
    #endregion Reverse

    #region Select
    /// <summary>
    /// Projects each element of a sequence into a new form.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="selector">A projection string expression to apply to each element.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. </param>
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
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
    /// <returns>An <see cref="JsonElement"/> whose elements are the result of invoking a projection string on each element of source.</returns>
    public static JsonDocument Select(this JsonDocument source, SystemTextJsonParsingConfig config, string selector, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);
        Check.NotNullOrEmpty(selector);

        var queryable = ToQueryable(source, config);
        return ToJsonDocumentArray(() => queryable.Select(config, selector, args));
    }

    /// <summary>
    /// Projects each element of a sequence into a new class of type TResult.
    /// Details see http://solutionizing.net/category/linq/ 
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="config">The <see cref="ParsingConfig"/>.</param>
    /// <param name="resultType">The result type.</param>
    /// <param name="selector">A projection string expression to apply to each element.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
    /// <returns>An <see cref="JsonDocument"/> whose elements are the result of invoking a projection string on each element of source.</returns>
    public static JsonDocument Select(this JsonDocument source, SystemTextJsonParsingConfig config, Type resultType, string selector, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJsonDocumentArray(() => queryable.Select(config, resultType, selector, args));
    }

    /// <summary>
    /// Projects each element of a sequence into a new class of type TResult.
    /// Details see http://solutionizing.net/category/linq/ 
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="resultType">The result type.</param>
    /// <param name="selector">A projection string expression to apply to each element.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
    /// <returns>An <see cref="JsonDocument"/> whose elements are the result of invoking a projection string on each element of source.</returns>
    public static JsonDocument Select(this JsonDocument source, Type resultType, string selector, params object?[] args)
    {
        return Select(source, SystemTextJsonParsingConfig.Default, resultType, selector, args);
    }
    #endregion Select

    #region Single
    /// <summary>
    /// Returns the only element of a sequence, and throws an exception if there
    /// is not exactly one element in the sequence.
    /// </summary>
    /// <param name="source">A <see cref="IQueryable"/> to return the single element of.</param>
    /// <returns>The single element of the input sequence.</returns>
    public static JsonElement Single(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonElement(queryable.Single()) ?? throw new InvalidOperationException(Res.SequenceContainsNoElements);
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if there
    /// is not exactly one element in the sequence.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the last element of.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JsonElement Single(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJsonElement(queryable.Single(predicate, args)) ?? throw new InvalidOperationException(Res.SequenceContainsNoElements);
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition, and throws an exception if there
    /// is not exactly one element in the sequence.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the last element of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JsonElement Single(this JsonDocument source, string predicate, params object?[] args)
    {
        return Single(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion Single

    #region SingleOrDefault
    /// <summary>
    /// Returns the only element of a sequence, or a default value if the sequence
    /// is empty; this method throws an exception if there is more than one element
    /// in the sequence.
    /// </summary>
    /// <param name="source">A <see cref="JsonDocument"/> to return the single element of.</param>
    /// <returns>The single element of the input sequence, or default if the sequence contains no elements.</returns>
    public static JsonElement? SingleOrDefault(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonElement(queryable.SingleOrDefault());
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition or a default value if the sequence
    /// is empty; and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the last element of.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JsonElement? SingleOrDefault(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJsonElement(queryable.SingleOrDefault(predicate, args));
    }

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified condition or a default value if the sequence
    /// is empty; and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    /// <param name="source">The <see cref="JsonDocument"/> to return the last element of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The first element in source that passes the test in predicate.</returns>
    public static JsonElement? SingleOrDefault(this JsonDocument source, string predicate, params object?[] args)
    {
        return SingleOrDefault(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion SingleOrDefault

    #region Skip
    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
    /// </summary>
    /// <param name="source">A <see cref="JsonDocument"/> to return elements from.</param>
    /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
    /// <returns>A <see cref="JsonDocument"/> that contains elements that occur after the specified index in the input sequence.</returns>
    public static JsonDocument Skip(this JsonDocument source, int count)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonDocumentArray(() => queryable.Skip(count));
    }
    #endregion Skip

    #region SkipWhile
    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
    /// </summary>
    /// <param name="source">A <see cref="JsonDocument"/> to return elements from.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>An <see cref="JsonDocument"/> that contains elements from source starting at the first element in the linear series that does not pass the test specified by predicate.</returns>
    public static JsonDocument SkipWhile(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source);
        return ToJsonDocumentArray(() => queryable.SkipWhile(predicate, args));
    }

    /// <summary>
    /// Bypasses elements in a sequence as long as a specified condition is true and then returns the remaining elements.
    /// </summary>
    /// <param name="source">A <see cref="JsonDocument"/> to return elements from.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>An <see cref="JsonDocument"/> that contains elements from source starting at the first element in the linear series that does not pass the test specified by predicate.</returns>
    public static JsonDocument SkipWhile(this JsonDocument source, string predicate, params object?[] args)
    {
        return SkipWhile(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion SkipWhile

    #region Sum
    /// <summary>
    /// Computes the sum of a sequence of numeric values.
    /// </summary>
    /// <param name="source">A sequence of numeric values to calculate the sum of.</param>
    /// <returns>The sum of the values in the sequence.</returns>
    public static object Sum(this JsonDocument source)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return queryable.Sum();
    }

    /// <summary>
    /// Computes the sum of a sequence of numeric values.
    /// </summary>
    /// <param name="source">A sequence of numeric values to calculate the sum of.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The sum of the values in the sequence.</returns>
    public static object Sum(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return queryable.Sum(predicate, args);
    }

    /// <summary>
    /// Computes the sum of a sequence of numeric values.
    /// </summary>
    /// <param name="source">A sequence of numeric values to calculate the sum of.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>The sum of the values in the sequence.</returns>
    public static object Sum(this JsonDocument source, string predicate, params object?[] args)
    {
        return Sum(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion Sum

    #region Take
    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a sequence.
    /// </summary>
    /// <param name="source">The sequence to return elements from.</param>
    /// <param name="count">The number of elements to return.</param>
    /// <returns>A <see cref="IQueryable"/> that contains the specified number of elements from the start of source.</returns>
    public static JsonDocument Take(this JsonDocument source, int count)
    {
        Check.NotNull(source);

        var queryable = ToQueryable(source);
        return ToJsonDocumentArray(() => queryable.Take(count));
    }
    #endregion Take

    #region TakeWhile
    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true.
    /// </summary>
    /// <param name="source">The sequence to return elements from.</param>
    /// <param name="config">The <see cref="SystemTextJsonParsingConfig"/>.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>An <see cref="JsonDocument"/> that contains elements from the input sequence occurring before the element at which the test specified by predicate no longer passes.</returns>
    public static JsonDocument TakeWhile(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source);
        return ToJsonDocumentArray(() => queryable.TakeWhile(predicate, args));
    }

    /// <summary>
    /// Returns elements from a sequence as long as a specified condition is true.
    /// </summary>
    /// <param name="source">The sequence to return elements from.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters. Similar to the way String.Format formats strings.</param>
    /// <returns>An <see cref="JsonDocument"/> that contains elements from the input sequence occurring before the element at which the test specified by predicate no longer passes.</returns>
    public static JsonDocument TakeWhile(this JsonDocument source, string predicate, params object?[] args)
    {
        return TakeWhile(source, SystemTextJsonParsingConfig.Default, predicate, args);
    }
    #endregion TakeWhile

    #region Where
    /// <summary>
    /// Filters a sequence of values based on a predicate.
    /// </summary>
    /// <param name="source">The source <see cref="JsonDocument"/></param>
    /// <param name="predicate">An expression string to test each element for a condition.</param>
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
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
    /// <param name="args">An object array that contains zero or more objects to insert into the predicate as parameters.</param>
    /// <returns>A <see cref="JsonDocument"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
    public static JsonDocument Where(this JsonDocument source, SystemTextJsonParsingConfig config, string predicate, params object?[] args)
    {
        Check.NotNull(source);
        Check.NotNull(config);

        var queryable = ToQueryable(source, config);
        return ToJsonDocumentArray(() => queryable.Where(config, predicate, args));
    }
    #endregion Where

    #region Private Methods
    private static JsonElement? ToJsonElement(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is JsonElement jsonElement)
        {
            return jsonElement;
        }

        return JsonElementUtils.FromObject(value);
    }

    private static JsonDocument ToJsonDocumentArray(Func<IQueryable> func)
    {
        var array = new List<object?>();
        foreach (var dynamicElement in func())
        {
            array.Add(ToJsonElement(dynamicElement));
        }

        return JsonDocumentUtils.FromObject(array);
    }

    // ReSharper disable once UnusedParameter.Local
    private static IQueryable ToQueryable(JsonDocument source, SystemTextJsonParsingConfig? config = null)
    {
        var array = source.RootElement;
        if (array.ValueKind != JsonValueKind.Array)
        {
            throw new NotSupportedException("The source is not a JSON array.");
        }

        return JsonDocumentExtensions.ToDynamicJsonClassArray(array).AsQueryable();
    }
    #endregion
}