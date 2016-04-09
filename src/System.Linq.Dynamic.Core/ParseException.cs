using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace System.Linq.Dynamic.Core
{
    /// <summary>
    /// Represents errors that occur while parsing dynamic linq string expressions.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
#if !(SILVERLIGHT || DNXCORE50 || DOTNET5_4)
    [Serializable]
#endif
    public sealed class ParseException : Exception
    {
        readonly int _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class with a specified error message and position.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="position">The location in the parsed string that produced the <see cref="ParseException"/></param>
        public ParseException(string message, int position)
            : base(message)
        {
            _position = position;
        }

        /// <summary>
        /// The location in the parsed string that produced the <see cref="ParseException"/>.
        /// </summary>
        public int Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>A string representation of the current exception.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, Res.ParseExceptionFormat, Message, _position);
        }

#if !(SILVERLIGHT || DNXCORE50 || DOTNET5_4)
        ParseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _position = (int)info.GetValue("position", typeof(int));
        }

        /// <summary>
        /// Supports Serialization
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("position", _position);
        }
#endif
    }
}
