namespace System.Reflection.Emit
{
    // ReSharper disable once InconsistentNaming
    public static class ILGeneratorExtensions
    {
        public static LocalBuilder DeclareLocal(this ILGenerator gen, TypeBuilder tb)
        {
            return gen.DeclareLocal(tb.GetType());
        }

        public static void Emit(this ILGenerator gen,  OpCode opcode, TypeBuilder tb)
        {
            gen.Emit(opcode, tb.GetType());
        }
    }
}