using JetBrains.Annotations;
using System.ComponentModel;
using System.Linq.Dynamic.Core.Validation;

namespace System.Linq.Dynamic.Core
{
    internal static class TypeConverterFactory
    {
        /// <summary>
        /// Returns a type converter for the specified type.
        /// </summary>
        /// <param name="type">The System.Type of the target component.</param>
        /// <returns>A System.ComponentModel.TypeConverter for the specified type.</returns>
        public static TypeConverter GetConverter([NotNull] Type type)
        {
            Check.NotNull(type, nameof(type));

#if !SILVERLIGHT
            return TypeDescriptor.GetConverter(type);
#else
            var attributes = type.GetCustomAttributes(typeof(TypeConverterAttribute), false);

            if (attributes.Length != 1)
                return new TypeConverter();

            var converterAttribute = (TypeConverterAttribute)attributes[0];
            var converterType = Type.GetType(converterAttribute.ConverterTypeName);

            if (converterType == null)
                return new TypeConverter();

            return Activator.CreateInstance(converterType) as TypeConverter;
#endif
        }
    }
}