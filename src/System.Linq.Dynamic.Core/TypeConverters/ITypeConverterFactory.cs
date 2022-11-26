using System.ComponentModel;

namespace System.Linq.Dynamic.Core.TypeConverters;

internal interface ITypeConverterFactory
{
    /// <summary>
    /// Returns a type converter for the specified type.
    /// </summary>
    /// <param name="type">The System.Type of the target component.</param>
    /// <returns>A System.ComponentModel.TypeConverter for the specified type.</returns>
    TypeConverter GetConverter(Type type);
}