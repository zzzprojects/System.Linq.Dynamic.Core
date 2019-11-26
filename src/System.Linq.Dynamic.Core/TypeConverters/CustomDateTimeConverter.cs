using System.ComponentModel;
using System.Globalization;

namespace System.Linq.Dynamic.Core.TypeConverters
{
    internal class CustomDateTimeConverter : DateTimeOffsetConverter
    {
        /// <summary>
        /// Converts the specified object to a <see cref="DateTime"></see>.
        /// </summary>
        /// <param name="context">The date format context.</param>
        /// <param name="culture">The date culture.</param>
        /// <param name="value">The object to be converted.</param>
        /// <returns>A <see cref="Nullable{DateTime}"></see> that represents the specified object.</returns>
        /// <exception cref="NotSupportedException">The conversion cannot be performed.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var dateTimeOffset = base.ConvertFrom(context, culture, value) as DateTimeOffset?;

            return dateTimeOffset?.UtcDateTime;
        }
    }
}
