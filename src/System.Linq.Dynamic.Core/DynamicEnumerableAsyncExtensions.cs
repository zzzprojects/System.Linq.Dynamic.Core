#if !(NET35 || NET40)
using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace System.Linq.Dynamic.Core;

/// <summary>
/// Define async extensions on <see cref="IEnumerable"/>.
/// </summary>
public static class DynamicEnumerableAsyncExtensions
{
    private static readonly MethodInfo ToListAsyncGenericMethod;

    static DynamicEnumerableAsyncExtensions()
    {
        ToListAsyncGenericMethod = typeof(DynamicEnumerableAsyncExtensions).GetTypeInfo()
            .GetDeclaredMethods("ToListAsync")
            .First(x => x.IsGenericMethod);
    }

    /// <summary>
    /// Async creates an array of dynamic objects from a <see cref="IEnumerable"/>.
    /// </summary>
    /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
    /// <param name="type">A <see cref="Type"/> cast to.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>An Array that contains the elements from the input sequence.</returns>
    [PublicAPI]
    public static async Task<dynamic[]> ToDynamicArrayAsync(this IEnumerable source, Type type, CancellationToken cancellationToken = default)
    {
        var result = await ToDynamicListAsync(Check.NotNull(source), Check.NotNull(type), cancellationToken).ConfigureAwait(false);
        return result.ToArray();
    }

    /// <summary>
    /// Async creates an array of dynamic objects from a <see cref="IEnumerable"/>.
    /// </summary>
    /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>An array that contains the elements from the input sequence.</returns>
    [PublicAPI]
    public static async Task<dynamic[]> ToDynamicArrayAsync(this IEnumerable source, CancellationToken cancellationToken = default)
    {
        return (await ToListAsync<dynamic>(Check.NotNull(source), cancellationToken).ConfigureAwait(false)).ToArray();
    }

    /// <summary>
    /// Async creates an array of dynamic objects from a <see cref="IEnumerable"/>.
    /// </summary>
    /// <typeparam name="T">The generic type.</typeparam>
    /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>An Array{T} that contains the elements from the input sequence.</returns>
    [PublicAPI]
    public static async Task<T[]> ToDynamicArrayAsync<T>(this IEnumerable source, CancellationToken cancellationToken = default)
    {
        return (await ToListAsync<T>(Check.NotNull(source), cancellationToken).ConfigureAwait(false)).ToArray();
    }

    /// <summary>
    /// Async creates a list of dynamic objects from a <see cref="IEnumerable"/>.
    /// </summary>
    /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
    /// <param name="type">A <see cref="Type"/> cast to.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>An List that contains the elements from the input sequence.</returns>
    [PublicAPI]
    public static async Task<List<dynamic>> ToDynamicListAsync(this IEnumerable source, Type type, CancellationToken cancellationToken = default)
    {
        Check.NotNull(source);
        Check.NotNull(type);

        var task = (Task)ToListAsyncGenericMethod.MakeGenericMethod(type).Invoke(source, new object[] { source, cancellationToken })!;

        await task.ConfigureAwait(false);

        var list = (IList)task.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(task)!;

        return list.Cast<dynamic>().ToList();
    }

    /// <summary>
    /// Async creates a list of dynamic objects from a <see cref="IEnumerable"/>.
    /// </summary>
    /// <param name="source">A <see cref="IEnumerable"/> to create a list from.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A List that contains the elements from the input sequence.</returns>
    [PublicAPI]
    public static Task<List<dynamic>> ToDynamicListAsync(this IEnumerable source, CancellationToken cancellationToken = default)
    {
        return ToListAsync<dynamic>(Check.NotNull(source), cancellationToken);
    }

    /// <summary>
    /// Async creates a list of dynamic objects from an <see cref="IEnumerable"/>.
    /// </summary>
    /// <typeparam name="T">Generic Type</typeparam>
    /// <param name="source">A <see cref="IEnumerable"/> to create a list from.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
    /// <returns>A List{T} that contains the elements from the input sequence.</returns>
    [PublicAPI]
    public static Task<List<T>> ToDynamicListAsync<T>(this IEnumerable source, CancellationToken cancellationToken = default)
    {
        return ToListAsync<T>(Check.NotNull(source), cancellationToken);
    }

#pragma warning disable CS1998
    // ReSharper disable once UnusedParameter.Local
    private static async Task<List<T>> ToListAsync<T>(IEnumerable source, CancellationToken cancellationToken)
#pragma warning restore CS1998
    {
        switch (source)
        {
#if NETSTANDARD2_1_OR_GREATER || ASYNCENUMERABLE
            case IAsyncEnumerable<T> asyncEnumerable:
                var list = new List<T>();
                await foreach (var element in asyncEnumerable.WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    list.Add(element);
                }
                return list;
#endif
            case IEnumerable<T> enumerable:
                return enumerable.ToList();

            default:
                return source.Cast<T>().ToList();
        }
    }
}
#endif