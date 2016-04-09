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
    }
}