#if !(NET35 || NET40)
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Define async extensions on <see cref="IEnumerable"/>.
    /// </summary>
    public static class DynamicEnumerableAsyncExtensions
    {
        private static readonly MethodInfo ToListAsyncGenericMethod;

        static DynamicEnumerableAsyncExtensions()
        {
            ToListAsyncGenericMethod = typeof(DynamicEnumerableAsyncExtensions).GetTypeInfo().GetDeclaredMethods("ToListAsync")
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
        public static async Task<dynamic[]> ToDynamicArrayAsync([NotNull] this IEnumerable source, [NotNull] Type type, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(type, nameof(type));

            var result = await ((Task<List<dynamic>>)ToListAsyncGenericMethod.MakeGenericMethod(type).Invoke(source, new object[] { source, cancellationToken }));

            return result.ToArray(); // Task.Run(() => source.ToDynamicArray(type));
        }

        /// <summary>
        /// Async creates an array of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>An array that contains the elements from the input sequence.</returns>
        [PublicAPI]
        public static async Task<dynamic[]> ToDynamicArrayAsync([NotNull] this IEnumerable source, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            return (await ToListAsync<dynamic>(source, cancellationToken).ConfigureAwait(false)).ToArray();
        }

        /// <summary>
        /// Async creates an array of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>An Array{T} that contains the elements from the input sequence.</returns>
        [PublicAPI]
        public static async Task<T[]> ToDynamicArrayAsync<T>([NotNull] this IEnumerable source, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            return (await ToListAsync<T>(source, cancellationToken).ConfigureAwait(false)).ToArray();
        }

        /// <summary>
        /// Async creates a list of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <param name="type">A <see cref="Type"/> cast to.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>An List that contains the elements from the input sequence.</returns>
        [PublicAPI]
        public static async Task<List<dynamic>> ToDynamicListAsync([NotNull] this IEnumerable source, [NotNull] Type type, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(type, nameof(type));

            var task = (Task)ToListAsyncGenericMethod.MakeGenericMethod(type).Invoke(source, new object[] { source, cancellationToken });

            await task.ConfigureAwait(false);

            // ReSharper disable once PossibleNullReferenceException
            var list = (IList)task.GetType().GetProperty(nameof(Task<object>.Result)).GetValue(task);

            return list.Cast<dynamic>().ToList();
        }

        /// <summary>
        /// Async creates a list of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create a list from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A List that contains the elements from the input sequence.</returns>
        [PublicAPI]
        public static Task<List<dynamic>> ToDynamicListAsync([NotNull] this IEnumerable source, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            return ToListAsync<dynamic>(source, cancellationToken);
        }

        /// <summary>
        /// Async creates a list of dynamic objects from an <see cref="IEnumerable"/>.
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="source">A <see cref="IEnumerable"/> to create a list from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> (optional).</param>
        /// <returns>A List{T} that contains the elements from the input sequence.</returns>
        [PublicAPI]
        public static Task<List<T>> ToDynamicListAsync<T>([NotNull] this IEnumerable source, CancellationToken cancellationToken = default)
        {
            Check.NotNull(source, nameof(source));
            return ToListAsync<T>(source, cancellationToken);
        }

        private static async Task<List<T>> ToListAsync<T>(IEnumerable source, CancellationToken cancellationToken)
        {
            var list = new List<T>();

            switch (source)
            {
#if NETSTANDARD2_1_OR_GREATER || EFCORE
                case IAsyncEnumerable<T> asyncEnumerable:
                    await foreach (var element in asyncEnumerable.WithCancellation(cancellationToken).ConfigureAwait(false))
                    {
                        list.Add(element);
                    }
                    return list;
#endif
                case IEnumerable<T> enumerable:
                    foreach (var element in enumerable)
                    {
                        list.Add(element);
                    }
                    return list;

                default:
                    foreach (var element in source.Cast<T>())
                    {
                        list.Add(element);
                    }
                    return list;
            }
        }

        //private static async Task<List<dynamic>> ToListAsync(IEnumerable source, Type type, CancellationToken cancellationToken)
        //{
        //    var list = new List<dynamic>();

        //    switch (source)
        //    {
        //        case IAsyncEnumerable<dynamic> asyncEnumerable:
        //            await foreach (var element in asyncEnumerable.WithCancellation(cancellationToken).ConfigureAwait(false))
        //            {
        //                list.Add(element);
        //            }
        //            return list;

        //        case IEnumerable<dynamic> enumerable:
        //            foreach (var element in enumerable)
        //            {
        //                list.Add(element);
        //            }
        //            return list;

        //        default:
        //            foreach (var element in source.Cast<dynamic>())
        //            {
        //                list.Add(element);
        //            }
        //            return list;
        //    }
        //}
    }
}
#endif