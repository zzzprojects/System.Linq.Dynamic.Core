using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Define extensions on <see cref="IEnumerable"/>.
    /// </summary>
    public static class DynamicEnumerableExtensions
    {
        private static readonly MethodInfo ToDynamicArrayGenericMethod;

        static DynamicEnumerableExtensions()
        {
            ToDynamicArrayGenericMethod = typeof(DynamicEnumerableExtensions).GetTypeInfo().GetDeclaredMethods("ToDynamicArray")
                .First(x => x.IsGenericMethod);
        }

        /// <summary>
        /// Creates an array of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <returns>An array that contains the elements from the input sequence.</returns>
#if NET35
        public static object[] ToDynamicArray([NotNull] this IEnumerable source)
        {
            Check.NotNull(source, nameof(source));
            return CastToArray<object>(source);
        }
#else
        public static dynamic[] ToDynamicArray([NotNull] this IEnumerable source)
        {
            Check.NotNull(source, nameof(source));
            return CastToArray<dynamic>(source);
        }
#endif

        /// <summary>
        /// Creates an array of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <returns>An Array{T} that contains the elements from the input sequence.</returns>
        public static T[] ToDynamicArray<T>([NotNull] this IEnumerable source)
        {
            Check.NotNull(source, nameof(source));
            return CastToArray<T>(source);
        }

        /// <summary>
        /// Creates an array of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <param name="type">A <see cref="Type"/> cast to.</param>
        /// <returns>An Array that contains the elements from the input sequence.</returns>
#if NET35
        public static object[] ToDynamicArray([NotNull] this IEnumerable source, [NotNull] Type type)
#else
        public static dynamic[] ToDynamicArray([NotNull] this IEnumerable source, [NotNull] Type type)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(type, nameof(type));

            IEnumerable result = (IEnumerable)ToDynamicArrayGenericMethod.MakeGenericMethod(type).Invoke(source, new object[] { source });
#if NET35
            return CastToArray<object>(result);
#else
            return CastToArray<dynamic>(result);
#endif
        }

        /// <summary>
        /// Creates a list of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <returns>A List that contains the elements from the input sequence.</returns>
#if NET35
        public static List<object> ToDynamicList([NotNull] this IEnumerable source)
#else
        public static List<dynamic> ToDynamicList([NotNull] this IEnumerable source)
#endif
        {
            Check.NotNull(source, nameof(source));
#if NET35
            return CastToList<object>(source);
#else
            return CastToList<dynamic>(source);
#endif
        }

        /// <summary>
        /// Creates a list of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <param name="type">A <see cref="Type"/> cast to.</param>
        /// <returns>A List that contains the elements from the input sequence.</returns>
#if NET35
        public static List<object> ToDynamicList([NotNull] this IEnumerable source, [NotNull] Type type)
#else
        public static List<dynamic> ToDynamicList([NotNull] this IEnumerable source, [NotNull] Type type)
#endif
        {
            Check.NotNull(source, nameof(source));
            Check.NotNull(type, nameof(type));

            return ToDynamicArray(source, type).ToList();
        }

        /// <summary>
        /// Creates a list of dynamic objects from a <see cref="IEnumerable"/>.
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
        /// <returns>A List{T} that contains the elements from the input sequence.</returns>
        public static List<T> ToDynamicList<T>([NotNull] this IEnumerable source)
        {
            Check.NotNull(source, nameof(source));
            return CastToList<T>(source);
        }

        internal static T[] CastToArray<T>(IEnumerable source)
        {
            return source.Cast<T>().ToArray();
        }

        internal static List<T> CastToList<T>(IEnumerable source)
        {
            return source.Cast<T>().ToList();
        }
    }
}
