using JetBrains.Annotations;
using System.ComponentModel;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core.Validation;

namespace System.Linq.Dynamic.Core.TypeConverters
{
    internal class TypeConverterFactory : ITypeConverterFactory
    {
        private readonly ParsingConfig _config;

        public TypeConverterFactory([NotNull] ParsingConfig config)
        {
            Check.NotNull(config, nameof(config));

            _config = config;
        }

        /// <see cref="ITypeConverterFactory.GetConverter"/>
        public TypeConverter GetConverter(Type type)
        {
            Check.NotNull(type, nameof(type));

            if (_config.DateTimeIsParsedAsUTC && (type == typeof(DateTime) || type == typeof(DateTime?)))
            {
                return new CustomDateTimeConverter();
            }

            var typeToCheck = TypeHelper.IsNullableType(type) ? TypeHelper.GetNonNullableType(type) : type;
            if (_config.TypeConverters != null && _config.TypeConverters.TryGetValue(typeToCheck, out var typeConverter))
            {
                return typeConverter;
            }

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
