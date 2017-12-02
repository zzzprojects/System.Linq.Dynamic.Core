using System.Linq.Dynamic.Core.Validation;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser
{
    internal static class TypeHelper
    {
        public static bool IsNumericType(Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        public static bool IsSignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        public static bool IsUnsignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        public static int GetNumericTypeKind(Type type)
        {
            type = GetNonNullableType(type);
#if !(NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            if (type.GetTypeInfo().IsEnum) return 0;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
#else
            if (type.GetTypeInfo().IsEnum) return 0;

            if (type == typeof(Char) || type == typeof(Single) || type == typeof(Double) || type == typeof(Decimal))
                return 1;
            if (type == typeof(SByte) || type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64))
                return 2;
            if (type == typeof(Byte) || type == typeof(UInt16) || type == typeof(UInt32) || type == typeof(UInt64))
                return 3;
            return 0;
#endif
        }

        public static string GetTypeName(Type type)
        {
            Type baseType = GetNonNullableType(type);

            string s = baseType.Name;
            if (type != baseType)
            {
                s += '?';
            }
            return s;
        }

        public static Type GetNonNullableType(Type type)
        {
            Check.NotNull(type, nameof(type));

            return IsNullableType(type) ? type.GetTypeInfo().GetGenericTypeArguments()[0] : type;
        }

        public static bool IsNullableType(Type type)
        {
            Check.NotNull(type, nameof(type));

            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsCompatibleWith(Type source, Type target)
        {
#if !(NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            if (source == target) return true;
            if (!target.IsValueType) return target.IsAssignableFrom(source);
            Type st = GetNonNullableType(source);
            Type tt = GetNonNullableType(target);
            if (st != source && tt == target) return false;
            TypeCode sc = st.GetTypeInfo().IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
            TypeCode tc = tt.GetTypeInfo().IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                default:
                    if (st == tt) return true;
                    break;
            }
            return false;
#else
            if (source == target) return true;
            if (!target.GetTypeInfo().IsValueType) return target.IsAssignableFrom(source);
            Type st = GetNonNullableType(source);
            Type tt = GetNonNullableType(target);
            if (st != source && tt == target) return false;
            Type sc = st.GetTypeInfo().IsEnum ? typeof(Object) : st;
            Type tc = tt.GetTypeInfo().IsEnum ? typeof(Object) : tt;

            if (sc == typeof(SByte))
            {
                if (tc == typeof(SByte) || tc == typeof(Int16) || tc == typeof(Int32) || tc == typeof(Int64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(Byte))
            {
                if (tc == typeof(Byte) || tc == typeof(Int16) || tc == typeof(UInt16) || tc == typeof(Int32) || tc == typeof(UInt32) || tc == typeof(Int64) || tc == typeof(UInt64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(Int16))
            {
                if (tc == typeof(Int16) || tc == typeof(Int32) || tc == typeof(Int64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(UInt16))
            {
                if (tc == typeof(UInt16) || tc == typeof(Int32) || tc == typeof(UInt32) || tc == typeof(Int64) || tc == typeof(UInt64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(Int32))
            {
                if (tc == typeof(Int32) || tc == typeof(Int64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(UInt32))
            {
                if (tc == typeof(UInt32) || tc == typeof(Int64) || tc == typeof(UInt64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(Int64))
            {
                if (tc == typeof(Int64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(UInt64))
            {
                if (tc == typeof(UInt64) || tc == typeof(Single) || tc == typeof(Double) || tc == typeof(Decimal))
                    return true;
            }
            else if (sc == typeof(Single))
            {
                if (tc == typeof(Single) || tc == typeof(Double))
                    return true;
            }

            if (st == tt)
                return true;

            return false;
#endif
        }
    }
}
