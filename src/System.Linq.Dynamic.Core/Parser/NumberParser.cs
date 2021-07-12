using System.Globalization;
using JetBrains.Annotations;

namespace System.Linq.Dynamic.Core.Parser
{
    /// <summary>
    /// NumberParser
    /// </summary>
    public class NumberParser
    {
        private readonly CultureInfo _culture;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberParser"/> class.
        /// </summary>
        /// <param name="config">The ParsingConfig.</param>
        public NumberParser([CanBeNull] ParsingConfig config)
        {
            _culture = config?.NumberParseCulture ?? CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Tries to parse the number (text) into the specified type.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="type">The type.</param>
        /// <param name="result">The result.</param>
        public bool TryParseNumber(string text, Type type, out object result)
        {
            result = ParseNumber(text, type);
            return type != null;
        }

        /// <summary>
        /// Parses the number (text) into the specified type.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="type">The type.</param>
        public object ParseNumber(string text, Type type)
        {
            try
            {
#if !(NETFX_CORE || WINDOWS_APP || UAP10_0 || NETSTANDARD)
                switch (Type.GetTypeCode(TypeHelper.GetNonNullableType(type)))
                {
                    case TypeCode.SByte:
                        return sbyte.Parse(text, _culture);
                    case TypeCode.Byte:
                        return byte.Parse(text, _culture);
                    case TypeCode.Int16:
                        return short.Parse(text, _culture);
                    case TypeCode.UInt16:
                        return ushort.Parse(text, _culture);
                    case TypeCode.Int32:
                        return int.Parse(text, _culture);
                    case TypeCode.UInt32:
                        return uint.Parse(text, _culture);
                    case TypeCode.Int64:
                        return long.Parse(text, _culture);
                    case TypeCode.UInt64:
                        return ulong.Parse(text, _culture);
                    case TypeCode.Single:
                        return float.Parse(text, _culture);
                    case TypeCode.Double:
                        return double.Parse(text, _culture);
                    case TypeCode.Decimal:
                        return decimal.Parse(text, _culture);
                }
#else
                var tp = TypeHelper.GetNonNullableType(type);
                if (tp == typeof(sbyte))
                {
                    return sbyte.Parse(text, _culture);
                }
                if (tp == typeof(byte))
                {
                    return byte.Parse(text, _culture);
                }
                if (tp == typeof(short))
                {
                    return short.Parse(text, _culture);
                }
                if (tp == typeof(ushort))
                {
                    return ushort.Parse(text, _culture);
                }
                if (tp == typeof(int))
                {
                    return int.Parse(text, _culture);
                }
                if (tp == typeof(uint))
                {
                    return uint.Parse(text, _culture);
                }
                if (tp == typeof(long))
                {
                    return long.Parse(text, _culture);
                }
                if (tp == typeof(ulong))
                {
                    return ulong.Parse(text, _culture);
                }
                if (tp == typeof(float))
                {
                    return float.Parse(text, _culture);
                }
                if (tp == typeof(double))
                {
                    return double.Parse(text, _culture);
                }
                if (tp == typeof(decimal))
                {
                    return decimal.Parse(text, _culture);
                }
#endif
            }
            catch
            {
                return null;
            }

            return null;
        }
    }
}
