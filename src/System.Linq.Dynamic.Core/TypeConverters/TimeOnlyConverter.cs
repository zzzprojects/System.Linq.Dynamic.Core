#if NET6_0
using System.ComponentModel;
using System.Globalization;

namespace System.Linq.Dynamic.Core.TypeConverters;

/// <summary>
/// Based on https://github.com/dotnet/runtime/issues/68743
/// </summary>
internal class TimeOnlyConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string s)
        {
            return TimeOnly.Parse(s, culture);
        }

        return base.ConvertFrom(context, culture, value);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string))
        {
            return ((TimeOnly?)value)?.ToString(culture);
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override bool IsValid(ITypeDescriptorContext? context, object? value)
    {
        return value is TimeOnly || base.IsValid(context, value);
    }
}
#endif