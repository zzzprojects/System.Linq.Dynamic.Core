using System.Reflection.Emit;

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    internal static class CustomTypeBuilderExtensions
    {
#if !(NET35 || NET40 || SILVERLIGHT || WPSL || UAP10_0)
        public static Type CreateType(this TypeBuilder tb)
        {
            return tb.CreateTypeInfo().AsType();
        }
#endif

#if NET35 || NET40 || SILVERLIGHT || WPSL
        public static PropertyBuilder DefineProperty(this TypeBuilder tb, string name, PropertyAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
        {
            return tb.DefineProperty(name, attributes, returnType, parameterTypes);
        }

        // https://github.com/castleproject/Core/blob/netcore/src/Castle.Core/Compatibility/CustomTypeBuilderExtensions.cs
        // TypeBuilder and GenericTypeParameterBuilder no longer inherit from Type but TypeInfo,
        // so there is now an AsType method to get the Type which we are providing here to shim to itself.
        public static Type AsType(this TypeBuilder builder)
        {
            return builder;
        }

        public static Type AsType(this GenericTypeParameterBuilder builder)
        {
            return builder;
        }
#endif
    }
}