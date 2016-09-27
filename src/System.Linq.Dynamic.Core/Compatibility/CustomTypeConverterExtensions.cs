
namespace System.ComponentModel
{
#if SILVERLIGHT
    internal static class CustomTypeConverterExtensions
    {
        /// <summary>
        /// Converts the given string to the type of this converter.
        /// </summary>
        /// <param name="typeConverter">The System.ComponentModel.TypeConverter</param>
        /// <param name="text">The System.String to convert</param>
        /// <returns>An System.Object that represents the converted text.</returns>
        public static object ConvertFromInvariantString(this TypeConverter typeConverter, string text)
        {
            return typeConverter.ConvertFromString(text);
        }
    }
#endif
}