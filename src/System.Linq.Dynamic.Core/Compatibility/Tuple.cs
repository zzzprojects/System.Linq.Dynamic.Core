#if NET35
namespace System
{
    /// <summary>
    /// Represents a 2-tuple, or pair.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    public class Tuple<T1, T2>
    {
        /// <summary>
        /// The value of the current System.Tuple`2 object's first component.
        /// </summary>
        public T1 Item1 { get; private set; }

        /// <summary>
        /// The value of the current System.Tuple`2 object's second component.
        /// </summary>
        public T2 Item2 { get; private set; }

        internal Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
}
#endif