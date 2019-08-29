using System.Collections.Generic;
using System.Linq.Dynamic.Core.Parser.SupportedMethods;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Linq.Dynamic.Core.Parser
{
    internal static class TypeHelper
    {
        private static MethodInfo[] _allExtensionMethodsInCurrentAssembliesCache;

        public static Type FindGenericType(Type generic, Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == generic)
                {
                    return type;
                }

                if (generic.GetTypeInfo().IsInterface)
                {
                    foreach (Type intfType in type.GetInterfaces())
                    {
                        Type found = FindGenericType(generic, intfType);
                        if (found != null) return found;
                    }
                }

                type = type.GetTypeInfo().BaseType;
            }

            return null;
        }

        /// <summary>
        /// Check if a type is copmatible with a target type. If the target type is a generic definition type
        /// the promotedTarget is filled with the needed generic arguments.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsCompatibleWith(Type source, Type target)
        {
            return IsCompatibleWith(source, target, out _);
        }

        /// <summary>
        /// Check if a type is copmatible with a target type. If the target type is a generic definition type
        /// the promotedTarget is filled with the needed generic arguments.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="promotedTarget"></param>
        /// <returns></returns>
        public static bool IsCompatibleWith(Type source, Type target, out Type promotedTarget)
        {
            promotedTarget = null;

#if !(NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            if (source == target)
            {
                return true;
            }

            if (!target.IsValueType)
            {
                // Chain match arguments
                // TODO:
                // Performance of this is terrible, too much reflection and
                // too much interfaces to consider, at compile time this is
                // acceptable, but not at runtime. Figure out a way to minimize
                // the number of introspected types/interfaces
                foreach (var t in source.GetSelfAndAllBaseAndInterfaces())
                {
                    if (target.IsAssignableFrom(t))
                    {
                        return true;
                    }

                    if ((target.IsGenericType || target.IsGenericTypeDefinition) && t.IsGenericType)
                    {
                        var targetGenericTypeDef = target.IsGenericType ? target.GetGenericTypeDefinition() : target;
                        var tGenericTypeDef = t.GetGenericTypeDefinition();

                        if (targetGenericTypeDef != tGenericTypeDef)
                        {
                            continue;
                        }

                        var targetGenericArgs = target.GetGenericArguments();
                        var tGenericArgs = t.GetGenericArguments();

                        bool compatibleArgs = true;
                        for (int x = 0; x < tGenericArgs.Length; x++)
                        {
                            var targetType = targetGenericArgs[x];

                            if (targetType.IsGenericParameter)
                            {
                                var constraints = targetType.GetGenericParameterConstraints();
                                // TODO: Check the constraints, covariance, etc...
                            }
                            else
                            {
                                if (!TypeHelper.IsCompatibleWith(tGenericArgs[x], targetType, out _))
                                {
                                    compatibleArgs = false;
                                    break;
                                }
                            }
                        }

                        if (!compatibleArgs)
                        {
                            continue;
                        }

                        promotedTarget = target.GetGenericTypeDefinition().MakeGenericType(TypeHelper.GetGenericTypeArguments(t));
                        return true;
                    }
                }

                return false;
            }

            Type st = GetNonNullableType(source);
            Type tt = GetNonNullableType(target);

            if (st != source && tt == target)
            {
                return false;
            }

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
                    if (st == tt)
                    {
                        return true;
                    }
                    break;
            }
            return false;
#else
            if (source == target)
            {
                return true;
            }
            if (!target.GetTypeInfo().IsValueType)
            {
                return target.IsAssignableFrom(source);
            }
            Type st = GetNonNullableType(source);
            Type tt = GetNonNullableType(target);

            if (st != source && tt == target)
            {
                return false;
            }
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
            {
                return true;
            }

            return false;
#endif
        }

        public static bool IsEnumType(Type type)
        {
            return GetNonNullableType(type).GetTypeInfo().IsEnum;
        }

        public static bool IsNumericType(Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        public static bool IsNullableType(this Type type)
        {
            Check.NotNull(type, nameof(type));

            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type ToNullableType(this Type type)
        {
            Check.NotNull(type, nameof(type));

            return IsNullableType(type) ? type : typeof(Nullable<>).MakeGenericType(type);
        }

        public static bool IsSignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        public static bool IsUnsignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        public static bool GetIsSealed(this Type type)
        {
            return type.GetTypeInfo().IsSealed;
        }

        private static int GetNumericTypeKind(Type type)
        {
            type = GetNonNullableType(type);

#if !(NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            if (type.GetTypeInfo().IsEnum)
            {
                return 0;
            }

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
            if (type.GetTypeInfo().IsEnum)
            {
                return 0;
            }

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

            string name = baseType.Name;
            if (type != baseType)
            {
                name += '?';
            }

            return name;
        }

        public static Type[] GetGenericTypeArguments(Type type)
        {
            Check.NotNull(type, nameof(type));

            return type.GetTypeInfo().GetGenericTypeArguments();
        }

        public static Type GetNonNullableType(Type type)
        {
            Check.NotNull(type, nameof(type));

            return IsNullableType(type) ? type.GetTypeInfo().GetGenericTypeArguments()[0] : type;
        }

        public static bool GetIsGenericType(this Type type)
        {
            Check.NotNull(type, nameof(type));

            return type.GetTypeInfo().IsGenericType;
        }

        public static Type GetUnderlyingType(this Type type)
        {
            Check.NotNull(type, nameof(type));

            Type[] genericTypeArguments = type.GetGenericArguments();
            if (genericTypeArguments.Any())
            {
                var outerType = GetUnderlyingType(genericTypeArguments.LastOrDefault());
                return Nullable.GetUnderlyingType(type) == outerType ? type : outerType;
            }

            return type;
        }

        public static IEnumerable<Type> GetSelfAndAllBaseAndInterfaces(this Type type)
        {
            var types = new List<Type>();

            foreach (var t in GetSelfAndBaseClasses(type))
            {
                AddInterface(types, t);
            }

            return types;
        }

        public static IEnumerable<Type> GetSelfAndBaseTypes(this Type type)
        {
            if (type.GetTypeInfo().IsInterface)
            {
                var types = new List<Type>();
                AddInterface(types, type);
                return types;
            }
            return GetSelfAndBaseClasses(type);
        }

        private static IEnumerable<Type> GetSelfAndBaseClasses(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.GetTypeInfo().BaseType;
            }
        }

        private static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
                foreach (Type t in type.GetInterfaces())
                {
                    AddInterface(types, t);
                }
            }
        }

        public static object ParseNumber(string text, Type type)
        {
#if !(NETFX_CORE || WINDOWS_APP || DOTNET5_1 || UAP10_0 || NETSTANDARD)
            switch (Type.GetTypeCode(GetNonNullableType(type)))
            {
                case TypeCode.SByte:
                    sbyte sb;
                    if (SByte.TryParse(text, out sb)) return sb;
                    break;
                case TypeCode.Byte:
                    byte b;
                    if (Byte.TryParse(text, out b)) return b;
                    break;
                case TypeCode.Int16:
                    short s;
                    if (Int16.TryParse(text, out s)) return s;
                    break;
                case TypeCode.UInt16:
                    ushort us;
                    if (UInt16.TryParse(text, out us)) return us;
                    break;
                case TypeCode.Int32:
                    int i;
                    if (Int32.TryParse(text, out i)) return i;
                    break;
                case TypeCode.UInt32:
                    uint ui;
                    if (UInt32.TryParse(text, out ui)) return ui;
                    break;
                case TypeCode.Int64:
                    long l;
                    if (Int64.TryParse(text, out l)) return l;
                    break;
                case TypeCode.UInt64:
                    ulong ul;
                    if (UInt64.TryParse(text, out ul)) return ul;
                    break;
                case TypeCode.Single:
                    float f;
                    if (Single.TryParse(text, out f)) return f;
                    break;
                case TypeCode.Double:
                    double d;
                    if (Double.TryParse(text, out d)) return d;
                    break;
                case TypeCode.Decimal:
                    decimal e;
                    if (Decimal.TryParse(text, out e)) return e;
                    break;
            }
#else
            var tp = GetNonNullableType(type);
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

        public static object ParseEnum(string value, Type type)
        {
            if (type.GetTypeInfo().IsEnum && Enum.IsDefined(type, value))
            {
                return Enum.Parse(type, value, true);
            }

            return null;
        }

        /// <summary>
        /// This Method extends the System.Type-type to get all extended methods. It searches hereby in all assemblies which are known by the current AppDomain.
        /// </summary>
        /// <remarks>
        /// Inspired by Jon Skeet from his answer on http://stackoverflow.com/questions/299515/c-sharp-reflection-to-identify-extension-methods
        /// </remarks>
        /// <returns>returns MethodInfo[] with the extended Method</returns>

        internal static List<MethodData> GetExtensionMethods(this Type t, string name, List<Type> types)
        {
            // TODO: We need an intermediate cache for this, that should not use t as key, but each individual type in the inheritance chain
            // TODO: We might need to promote the incoming expression instance for the extension param
            List<MethodData> methods = new List<MethodData>();

            foreach (var method in AllExtensionMethods(types))
            {
                if (!method.Name.Equals(name, StringComparison.CurrentCulture))
                {
                    continue;
                }

                methods.Add(new MethodData(method));
            }

            return methods;
        }

        private static MethodInfo[] AllExtensionMethods(List<Type> types)
        {
            if (_allExtensionMethodsInCurrentAssembliesCache != null)
            {
                return _allExtensionMethodsInCurrentAssembliesCache;
            }

            List<Type> assTypes = types;

            var query = from type in assTypes
                        where type.GetIsSealed() && !type.GetIsGenericType() && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        select method;

            _allExtensionMethodsInCurrentAssembliesCache = query.ToArray<MethodInfo>();

            return _allExtensionMethodsInCurrentAssembliesCache;
        }
    }
}
