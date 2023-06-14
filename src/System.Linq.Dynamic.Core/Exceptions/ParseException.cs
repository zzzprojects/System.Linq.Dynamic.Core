using System.Globalization;

#if !(SILVERLIGHT || WINDOWS_APP ||  UAP10_0 || NETSTANDARD || PORTABLE || WPSL)
using System.Runtime.Serialization;
#endif

namespace System.Linq.Dynamic.Core.Exceptions
{
    /// <summary>
    /// Represents errors that occur while parsing dynamic linq string expressions.
    /// </summary>
#if !(SILVERLIGHT || WINDOWS_APP || UAP10_0 || NETSTANDARD || PORTABLE || WPSL || NETSTANDARD2_0)
    [Serializable]
#endif
    public sealed class ParseException : Exception
    {
        /// <summary>
        /// The location in the parsed string that produced the <see cref="ParseException"/>.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class with a specified error message and position.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="position">The location in the parsed string that produced the <see cref="ParseException"/></param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public ParseException(string message, int position, Exception? innerException = null) : base(message, innerException)
        {
            Position = position;
        }

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>A string representation of the current exception.</returns>
        public override string ToString()
        {
            var text = string.Format(CultureInfo.CurrentCulture, Res.ParseExceptionFormat, Message, Position);

            if (InnerException != null)
            {
                text = $"{text} ---> {InnerException}{Environment.NewLine}   --- End of inner exception stack trace ---";
            }

            if (StackTrace != null)
            {
                text = $"{text}{Environment.NewLine}{StackTrace}";
            }

            return text;
        }

#if !(SILVERLIGHT || WINDOWS_APP ||  UAP10_0 || NETSTANDARD || PORTABLE || WPSL || NETSTANDARD2_0)
        private ParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Position = (int)info.GetValue("position", typeof(int));
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("position", Position);
        }
#endif
    }
}
