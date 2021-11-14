using System.Globalization;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using static System.Net.Mime.MediaTypeNames;

namespace System.Linq.Dynamic.Core.Parser
{
    /// <summary>
    /// NumberParser
    /// </summary>
    public class NumberParser
    {
        private static readonly Regex RegexBinary32 = new Regex("^[01]{1,32}$", RegexOptions.Compiled);
        private static readonly Regex RegexBinary64 = new Regex("^[01]{1,64}$", RegexOptions.Compiled);
        private static readonly char[] Qualifiers = new[] { 'U', 'u', 'L', 'l', 'F', 'f', 'D', 'd', 'M', 'm' };
        private static readonly char[] QualifiersForHex = new[] { 'U', 'u', 'L', 'l' };

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
        /// <param name="tokenPosition">The current token position (needed for error repoorting).</param>
        /// <param name="text">The text.</param>
        /// <param name="expression">The constant expression.</param>
        public bool TryParseNumber(int tokenPosition, string text, out Expression expression)
        {
            string qualifier = null;
            var last = text[^1];
            var isNegative = text[0] == '-';
            var isHexadecimal = text.StartsWith(isNegative ? "-0x" : "0x", StringComparison.OrdinalIgnoreCase);
            var isBinary = text.StartsWith(isNegative ? "-0b" : "0b", StringComparison.OrdinalIgnoreCase);
            var qualifierLetters = isHexadecimal ? QualifiersForHex : Qualifiers;

            if (qualifierLetters.Contains(last))
            {
                int pos = text.Length - 1, count = 0;
                while (qualifierLetters.Contains(text[pos]))
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
                    text = text[2..];
                }

                if (isBinary)
                {
                    return TryParseAsBinary(text, isNegative, out expression);
                }

                if (!ulong.TryParse(text, isHexadecimal ? NumberStyles.HexNumber : NumberStyles.Integer, _culture, out ulong value))
                {
                    expression = null;
                    return false;
                }

                if (!string.IsNullOrEmpty(qualifier))
                {
                    if (qualifier == "U" || qualifier == "u")
                    {
                        expression = ConstantExpressionHelper.CreateLiteral((uint)value, text);
                        return true;
                    }
                    
                    if (qualifier == "L" || qualifier == "l")
                    {
                        expression = ConstantExpressionHelper.CreateLiteral((long)value, text);
                        return true;
                    }
                  
                    expression = ConstantExpressionHelper.CreateLiteral(value, text);
                    return true;
                }

                if (value <= int.MaxValue)
                {
                    expression = ConstantExpressionHelper.CreateLiteral((int)value, text);
                    return true;
                }
                
                if (value <= uint.MaxValue)
                {
                    expression = ConstantExpressionHelper.CreateLiteral((uint)value, text);
                    return true;
                }
                
                if (value <= long.MaxValue)
                {
                    expression = ConstantExpressionHelper.CreateLiteral((long)value, text);
                    return true;
                }
                
                expression = ConstantExpressionHelper.CreateLiteral(value, text);
                return true;
            }
            
            if (isHexadecimal || isBinary)
            {
                text = text[3..];
            }

            if (isBinary)
            {
                return TryParseAsBinary(text, isNegative, out expression);
            }

            if (!long.TryParse(text, isHexadecimal ? NumberStyles.HexNumber : NumberStyles.Integer, _culture, out long value))
            {
                expression = null;
                return false;
            }

            if (isHexadecimal)
            {
                value = -value;
            }

            if (!string.IsNullOrEmpty(qualifier))
            {
                if (qualifier == "L" || qualifier == "l")
                {
                    expression = ConstantExpressionHelper.CreateLiteral(value, text);
                    return true;
                }

                if (qualifier == "F" || qualifier == "f" || qualifier == "D" || qualifier == "d" || qualifier == "M" || qualifier == "m")
                {
                    return ParseRealLiteral(text, qualifier[0], false);
                }

                throw ParseError(Res.MinusCannotBeAppliedToUnsignedInteger);
            }

            if (value <= int.MaxValue)
            {
                return ConstantExpressionHelper.CreateLiteral((int)value, text);
            }

            return ConstantExpressionHelper.CreateLiteral(value, text);
         
        }

        private static bool TryParseAsBinary(string text, bool isNegative, out Expression expression)
        {
            if (RegexBinary32.IsMatch(text))
            {
                expression = ConstantExpressionHelper.CreateLiteral((isNegative ? -1 : 1) * Convert.ToInt32(text, 2), text);
                return true;
            }

            if (RegexBinary64.IsMatch(text))
            {
                expression = ConstantExpressionHelper.CreateLiteral((isNegative ? -1 : 1) * Convert.ToInt64(text, 2), text);
                return true;
            }

            expression = null;
            return false;
        }

        private bool TryParseAsReal(string text, char qualifier, bool stripQualifier, out Expression expression)
        {
            object o;
            switch (qualifier)
            {
                case 'f':
                case 'F':
                    o = ParseNumber(stripQualifier ? text.Substring(0, text.Length - 1) : text, typeof(float));
                    break;

                case 'm':
                case 'M':
                    o = ParseNumber(stripQualifier ? text.Substring(0, text.Length - 1) : text, typeof(decimal));
                    break;

                case 'd':
                case 'D':
                    o = ParseNumber(stripQualifier ? text.Substring(0, text.Length - 1) : text, typeof(double));
                    break;

                default:
                    o = ParseNumber(text, typeof(double));
                    break;
            }

            if (o != null)
            {
                expression = ConstantExpressionHelper.CreateLiteral(o, text);
            }

            //throw ParseError(Res.InvalidRealLiteral, text);
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
