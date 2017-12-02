using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser
{
    internal static class ExpressionPromoter
    {
        public static Expression Promote(Expression expr, Type type, bool exact, bool convertExpr)
        {
            if (expr.Type == type)
            {
                return expr;
            }

            var ce = expr as ConstantExpression;

            if (ce != null)
            {
                if (ce == Constants.NullLiteral || ce.Value == null)
                {
                    if (!type.GetTypeInfo().IsValueType || TypeHelper.IsNullableType(type))
                    {
                        return Expression.Constant(null, type);
                    }
                }
                else
                {
                    if (ConstantExpressionHelper.TryGetText(ce, out string text))
                    {
                        Type target = TypeHelper.GetNonNullableType(type);
                        object value = null;

#if !(NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
                        switch (Type.GetTypeCode(ce.Type))
                        {
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                                value = ParseNumber(text, target);

                                // Make sure an enum value stays an enum value
                                if (target.IsEnum)
                                {
                                    value = Enum.ToObject(target, value);
                                }
                                break;

                            case TypeCode.Double:
                                if (target == typeof(decimal)) value = ParseNumber(text, target);
                                break;

                            case TypeCode.String:
                                value = ParseEnum(text, target);
                                break;
                        }
#else
                        if (ce.Type == typeof(Int32) || ce.Type == typeof(UInt32) || ce.Type == typeof(Int64) || ce.Type == typeof(UInt64))
                        {
                            value = ParseNumber(text, target);

                            // Make sure an enum value stays an enum value
                            if (target.GetTypeInfo().IsEnum)
                            {
                                value = Enum.ToObject(target, value);
                            }
                        }
                        else if (ce.Type == typeof(Double))
                        {
                            if (target == typeof(decimal))
                            {
                                value = ParseNumber(text, target);
                            }
                        }
                        else if (ce.Type == typeof(String))
                        {
                            value = ParseEnum(text, target);
                        }
#endif
                        if (value != null)
                        {
                            return Expression.Constant(value, type);
                        }
                    }
                }
            }

            if (TypeHelper.IsCompatibleWith(expr.Type, type))
            {
                if (type.GetTypeInfo().IsValueType || exact || expr.Type.GetTypeInfo().IsValueType && convertExpr)
                {
                    return Expression.Convert(expr, type);
                }

                return expr;
            }

            return null;
        }

        static object ParseNumber(string text, Type type)
        {
#if !(NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            switch (Type.GetTypeCode(TypeHelper.GetNonNullableType(type)))
            {
                case TypeCode.SByte:
                    sbyte sb;
                    if (sbyte.TryParse(text, out sb)) return sb;
                    break;
                case TypeCode.Byte:
                    byte b;
                    if (byte.TryParse(text, out b)) return b;
                    break;
                case TypeCode.Int16:
                    short s;
                    if (short.TryParse(text, out s)) return s;
                    break;
                case TypeCode.UInt16:
                    ushort us;
                    if (ushort.TryParse(text, out us)) return us;
                    break;
                case TypeCode.Int32:
                    int i;
                    if (int.TryParse(text, out i)) return i;
                    break;
                case TypeCode.UInt32:
                    uint ui;
                    if (uint.TryParse(text, out ui)) return ui;
                    break;
                case TypeCode.Int64:
                    long l;
                    if (long.TryParse(text, out l)) return l;
                    break;
                case TypeCode.UInt64:
                    ulong ul;
                    if (ulong.TryParse(text, out ul)) return ul;
                    break;
                case TypeCode.Single:
                    float f;
                    if (float.TryParse(text, out f)) return f;
                    break;
                case TypeCode.Double:
                    double d;
                    if (double.TryParse(text, out d)) return d;
                    break;
                case TypeCode.Decimal:
                    decimal e;
                    if (decimal.TryParse(text, out e)) return e;
                    break;
            }
#else
            var tp = TypeHelper.GetNonNullableType(type);
            if (tp == typeof(SByte))
            {
                sbyte sb;
                if (sbyte.TryParse(text, out sb)) return sb;
            }
            else if (tp == typeof(Byte))
            {
                byte b;
                if (byte.TryParse(text, out b)) return b;
            }
            else if (tp == typeof(Int16))
            {
                short s;
                if (short.TryParse(text, out s)) return s;
            }
            else if (tp == typeof(UInt16))
            {
                ushort us;
                if (ushort.TryParse(text, out us)) return us;
            }
            else if (tp == typeof(Int32))
            {
                int i;
                if (int.TryParse(text, out i)) return i;
            }
            else if (tp == typeof(UInt32))
            {
                uint ui;
                if (uint.TryParse(text, out ui)) return ui;
            }
            else if (tp == typeof(Int64))
            {
                long l;
                if (long.TryParse(text, out l)) return l;
            }
            else if (tp == typeof(UInt64))
            {
                ulong ul;
                if (ulong.TryParse(text, out ul)) return ul;
            }
            else if (tp == typeof(Single))
            {
                float f;
                if (float.TryParse(text, out f)) return f;
            }
            else if (tp == typeof(Double))
            {
                double d;
                if (double.TryParse(text, out d)) return d;
            }
            else if (tp == typeof(Decimal))
            {
                decimal e;
                if (decimal.TryParse(text, out e)) return e;
            }
#endif
            return null;
        }

        static object ParseEnum(string value, Type type)
        {
#if !(NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            if (type.IsEnum)
            {
                MemberInfo[] memberInfos = type.FindMembers(MemberTypes.Field, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static, Type.FilterNameIgnoreCase, value);

                if (memberInfos.Length != 0)
                {
                    return ((FieldInfo)memberInfos[0]).GetValue(null);
                }
            }
#else
            if (type.GetTypeInfo().IsEnum)
            {
                return Enum.Parse(type, value, true);
            }
#endif
            return null;
        }
    }
}
