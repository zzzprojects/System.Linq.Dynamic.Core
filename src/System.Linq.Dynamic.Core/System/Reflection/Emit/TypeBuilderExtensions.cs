#if DNXCORE50
using System.Linq;
#endif

namespace System.Reflection.Emit
{
    public static class TypeBuilderExtensions
    {
#if !(NET40 || NET35)
        public static Type CreateType(this TypeBuilder tb)
        {
            return tb.CreateTypeInfo().AsType();
        }
#endif

#if NET35
        public static PropertyBuilder DefineProperty(this TypeBuilder tb, string name, PropertyAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
        {
            return tb.DefineProperty(name, attributes, returnType, parameterTypes);
        }
#endif

#if DNXCORE50
        public static ConstructorBuilder DefineConstructor(this TypeBuilder tb, MethodAttributes attributes, CallingConventions callingConventions, GenericTypeParameterBuilder[] parameterTypes)
        {
            return tb.DefineConstructor(attributes, callingConventions, parameterTypes.Select(g => g.AsType()).ToArray());
        }

        public static FieldBuilder DefineField(this TypeBuilder tb, string fieldName, GenericTypeParameterBuilder genericTypeParameterBuilder, FieldAttributes attributes)
        {
            return tb.DefineField(fieldName, genericTypeParameterBuilder.AsType(), attributes);
        }

        public static MethodBuilder DefineMethod(this TypeBuilder tb, string name, MethodAttributes attributes, CallingConventions callingConvention, GenericTypeParameterBuilder returnType, Type[] parameterTypes)
        {
            return tb.DefineMethod(name, attributes, callingConvention, returnType.AsType(), parameterTypes);
        }

        public static MethodBuilder DefineMethod(this TypeBuilder tb, string name, MethodAttributes attributes, CallingConventions callingConvention, Type returnType, GenericTypeParameterBuilder[] parameterTypes)
        {
            return tb.DefineMethod(name, attributes, callingConvention, returnType, parameterTypes.Select(g => g.AsType()).ToArray());
        }

        public static PropertyBuilder DefineProperty(this TypeBuilder tb, string name, PropertyAttributes attributes, CallingConventions callingConvention, GenericTypeParameterBuilder returnType, Type[] parameterTypes)
        {
            return tb.DefineProperty(name, attributes, callingConvention, returnType.AsType(), parameterTypes);
        }
#endif
    }
}