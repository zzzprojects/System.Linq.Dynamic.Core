#if !(NET35 || NET40)
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Threading.Tasks;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Define async extensions on <see cref="IEnumerable"/>.
    /// </summary>
    public static class DynamicEnumerableAsyncExtensions
    {
        /// <summary>
        /// Async creates an array of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <param name="type">A <see cref="Type"/> cast to.</param>
        /// <returns>An Array that contains the elements from the input sequence.</returns>
        [PublicAPI]
        public static Task<dynamic[]> ToDynamicArrayAsync([NotNull] this IEnumerable source, [NotNull] Type type)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(type, nameof(type));

            return Task.Run(() => source.ToDynamicArray());
        }

        /// <summary>
        /// Async creates an array of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <returns>An array that contains the elements from the input sequence.</returns>
        [PublicAPI]
        public static Task<dynamic[]> ToDynamicArrayAsync([NotNull] this IEnumerable source)
        {
            Check.NotNull(source, nameof(source));
            return CastToArrayAsync<dynamic>(source);
        }

        /// <summary>
        /// Async creates an array of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <returns>An Array{T} that contains the elements from the input sequence.</returns>
        [PublicAPI]
        public static Task<T[]> ToDynamicArrayAsync<T>([NotNull] this IEnumerable source)
        {
            Check.NotNull(source, nameof(source));
            return CastToArrayAsync<T>(source);
        }

        /// <summary>
        /// Async creates a list of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <param name="type">A <see cref="Type"/> cast to.</param>
        /// <returns>An List that contains the elements from the input sequence.</returns>
        [PublicAPI]
        public static Task<List<dynamic>> ToDynamicListAsync([NotNull] this IEnumerable source, [NotNull] Type type)
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(type, nameof(type));

            return Task.Run(() => source.ToDynamicList());
        }

        /// <summary>
        /// Async creates a list of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <returns>A List that contains the elements from the input sequence.</returns>
        [PublicAPI]
        public static Task<List<dynamic>> ToDynamicListAsync([NotNull] this IEnumerable source)
        {
            Check.NotNull(source, nameof(source));
            return CastToListAsync<dynamic>(source);
        }

        /// <summary>
        /// Async creates a list of dynamic objects from an <see cref="IEnumerable"/>.
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <returns>A List{T} that contains the elements from the input sequence.</returns>
        [PublicAPI]
        public static Task<List<T>> ToDynamicListAsync<T>([NotNull] this IEnumerable source)
        {
            Check.NotNull(source, nameof(source));
            return CastToListAsync<T>(source);
        }

        private static Task<T[]> CastToArrayAsync<T>(IEnumerable source)
        {
            return Task.Run(() => DynamicEnumerableExtensions.CastToArray<T>(source));
        }

        private static Task<List<T>> CastToListAsync<T>(IEnumerable source)
        {
            return Task.Run(() => DynamicEnumerableExtensions.CastToList<T>(source));
        }
    }
}
#endif