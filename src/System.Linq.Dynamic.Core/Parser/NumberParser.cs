using System.Globalization;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace System.Linq.Dynamic.Core.Parser
{
    /// <summary>
    /// NumberParser
    /// </summary>
    public class NumberParser
    {
        private static readonly Regex RegexBinary32 = new("^[01]{1,32}$", RegexOptions.Compiled);
        private static readonly Regex RegexBinary64 = new("^[01]{1,64}$", RegexOptions.Compiled);
        private static readonly char[] Qualifiers = { 'U', 'u', 'L', 'l', 'F', 'f', 'D', 'd', 'M', 'm' };
        private static readonly char[] QualifiersHex = { 'U', 'u', 'L', 'l' };
        private static readonly string[] QualifiersReal = { "F", "f", "D", "d", "M", "m" };

        private readonly CultureInfo _culture;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberParser"/> class.
        /// </summary>
        /// <param name="config">The ParsingConfig.</param>
        public NumberParser(ParsingConfig? config)
        {
            _culture = config?.NumberParseCulture ?? CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Tries to parse the text into a IntegerLiteral ConstantExpression.
        /// </summary>
        /// <param name="tokenPosition">The current token position (needed for error reporting).</param>
        /// <param name="text">The text.</param>
        public Expression ParseIntegerLiteral(int tokenPosition, string text)
        {
            Check.NotEmpty(text, nameof(text));

            var last = text[text.Length - 1];
            var isNegative = text[0] == '-';
            var isHexadecimal = text.StartsWith(isNegative ? "-0x" : "0x", StringComparison.OrdinalIgnoreCase);
            var isBinary = text.StartsWith(isNegative ? "-0b" : "0b", StringComparison.OrdinalIgnoreCase);
            var qualifiers = isHexadecimal ? QualifiersHex : Qualifiers;

            string? qualifier = null;
            if (qualifiers.Contains(last))
            {
                int pos = text.Length - 1, count = 0;
                while (qualifiers.Contains(text[pos]))
                {
                    ++count;
                    --pos;
                }
                qualifier = text.Substring(text.Length - count, count);
                text = text.Substring(0, text.Length - count);
            }

            if (!isNegative)
            {
                if (isHexadecimal || isBinary)
                {
                    text = text.Substring(2);
                }

                if (isBinary)
                {
                    return ParseAsBinary(tokenPosition, text, isNegative);
                }

                if (!ulong.TryParse(text, isHexadecimal ? NumberStyles.HexNumber : NumberStyles.Integer, _culture, out ulong unsignedValue))
                {
                    throw new ParseException(string.Format(_culture, Res.InvalidIntegerLiteral, text), tokenPosition);
                }

                if (!string.IsNullOrEmpty(qualifier) && qualifier!.Length > 0)
                {
                    if (qualifier == "U" || qualifier == "u")
                    {
                        return ConstantExpressionHelper.CreateLiteral((uint)unsignedValue, text);
                    }

                    if (qualifier == "L" || qualifier == "l")
                    {
                        return ConstantExpressionHelper.CreateLiteral((long)unsignedValue, text);
                    }

                    if (QualifiersReal.Contains(qualifier))
                    {
                        return ParseRealLiteral(text, qualifier[0], false);
                    }

                    return ConstantExpressionHelper.CreateLiteral(unsignedValue, text);
                }

                if (unsignedValue <= int.MaxValue)
                {
                    return ConstantExpressionHelper.CreateLiteral((int)unsignedValue, text);
                }

                if (unsignedValue <= uint.MaxValue)
                {
                    return ConstantExpressionHelper.CreateLiteral((uint)unsignedValue, text);
                }

                if (unsignedValue <= long.MaxValue)
                {
                    return ConstantExpressionHelper.CreateLiteral((long)unsignedValue, text);
                }

                return ConstantExpressionHelper.CreateLiteral(unsignedValue, text);
            }

            if (isHexadecimal || isBinary)
            {
                text = text.Substring(3);
            }

            if (isBinary)
            {
                return ParseAsBinary(tokenPosition, text, isNegative);
            }

            if (!long.TryParse(text, isHexadecimal ? NumberStyles.HexNumber : NumberStyles.Integer, _culture, out long value))
            {
                throw new ParseException(string.Format(_culture, Res.InvalidIntegerLiteral, text), tokenPosition);
            }

            if (isHexadecimal)
            {
                value = -value;
            }

            if (!string.IsNullOrEmpty(qualifier) && qualifier!.Length > 0)
            {
                if (qualifier == "L" || qualifier == "l")
                {
                    return ConstantExpressionHelper.CreateLiteral(value, text);
                }

                if (QualifiersReal.Contains(qualifier))
                {
                    return ParseRealLiteral(text, qualifier[0], false);
                }

                throw new ParseException(Res.MinusCannotBeAppliedToUnsignedInteger, tokenPosition);
            }

            if (value <= int.MaxValue)
            {
                return ConstantExpressionHelper.CreateLiteral((int)value, text);
            }

            return ConstantExpressionHelper.CreateLiteral(value, text);
        }

        /// <summary>
        /// Parse the text into a Real ConstantExpression.
        /// </summary>
        public Expression ParseRealLiteral(string text, char qualifier, bool stripQualifier)
        {
            switch (qualifier)
            {
                case 'f':
                case 'F':
                    return ConstantExpressionHelper.CreateLiteral(ParseNumber(stripQualifier ? text.Substring(0, text.Length - 1) : text, typeof(float))!, text);

                case 'm':
                case 'M':
                    return ConstantExpressionHelper.CreateLiteral(ParseNumber(stripQualifier ? text.Substring(0, text.Length - 1) : text, typeof(decimal))!, text);

                case 'd':
                case 'D':
                    return ConstantExpressionHelper.CreateLiteral(ParseNumber(stripQualifier ? text.Substring(0, text.Length - 1) : text, typeof(double))!, text);

                default:
                    return ConstantExpressionHelper.CreateLiteral(ParseNumber(text, typeof(double))!, text);
            }
        }

        /// <summary>
        /// Tries to parse the number (text) into the specified type.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="type">The type.</param>
        /// <param name="result">The result.</param>
        public bool TryParseNumber(string text, Type type, out object? result)
        {
            result = ParseNumber(text, type);
            return result != null;
        }

        /// <summary>
        /// Parses the number (text) into the specified type.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="type">The type.</param>
        public object? ParseNumber(string text, Type type)
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

        private Expression ParseAsBinary(int tokenPosition, string text, bool isNegative)
        {
            if (RegexBinary32.IsMatch(text))
            {
                return ConstantExpressionHelper.CreateLiteral((isNegative ? -1 : 1) * Convert.ToInt32(text, 2), text);
            }

            if (RegexBinary64.IsMatch(text))
            {
                return ConstantExpressionHelper.CreateLiteral((isNegative ? -1 : 1) * Convert.ToInt64(text, 2), text);
            }

            throw new ParseException(string.Format(_culture, Res.InvalidBinaryIntegerLiteral, text), tokenPosition);
        }
    }
}