using System.ComponentModel;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core.Validation;

namespace System.Linq.Dynamic.Core.TypeConverters;

internal class TypeConverterFactory : ITypeConverterFactory
{
    private readonly ParsingConfig _config;

#if NET6_0
    static TypeConverterFactory()
    {
        TypeDescriptor.AddAttributes(typeof(DateOnly), new TypeConverterAttribute(typeof(DateOnlyConverter)));
        TypeDescriptor.AddAttributes(typeof(TimeOnly), new TypeConverterAttribute(typeof(TimeOnlyConverter)));
    }
#endif

    public TypeConverterFactory(ParsingConfig config)
    {
        _config = Check.NotNull(config);
    }

    /// <see cref="ITypeConverterFactory.GetConverter"/>
    public TypeConverter GetConverter(Type type)
    {
        Check.NotNull(type);

        if (_config.DateTimeIsParsedAsUTC && (type == typeof(DateTime) || type == typeof(DateTime?)))
        {
            return new CustomDateTimeConverter();
        }

        var typeToCheck = TypeHelper.IsNullableType(type) ? TypeHelper.GetNonNullableType(type) : type;
        if (_config.TypeConverters != null && _config.TypeConverters.TryGetValue(typeToCheck, out var typeConverter))
        {
            return typeConverter;
        }

        return TypeDescriptor.GetConverter(type);
    }
}