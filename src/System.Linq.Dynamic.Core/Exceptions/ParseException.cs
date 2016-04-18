using System.Globalization;
#if !(SILVERLIGHT || DNXCORE50 || DOTNET5_4 || NETSTANDARD)
using System.Runtime.Serialization;
#endif

namespace System.Linq.Dynamic.Core.Exceptions
{
    /// <summary>
    /// Represents errors that occur while parsing dynamic linq string expressions.
    /// </summary>
#if !(SILVERLIGHT || DNXCORE50 || DOTNET5_4 || NETSTANDARD)
    [Serializable]
#endif
    public sealed class ParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class with a specified error message and position.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="position">The location in the parsed string that produced the <see cref="ParseException"/></param>
        public ParseException(string message, int position)
            : base(message)
        {
            Position = position;
        }

        /// <summary>
        /// The location in the parsed string that produced the <see cref="ParseException"/>.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>A string representation of the current exception.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, Res.ParseExceptionFormat, Message, Position);
        }

#if !(SILVERLIGHT || DNXCORE50 || DOTNET5_4 || NETSTANDARD)
        ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Position = (int)info.GetValue("position", typeof(int));
        }

        /// <summary>
        /// Supports Serialization
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("position", Position);
        }
#endif
    }
}