using System.Reflection.Emit;

namespace System.Linq.Dynamic.Core.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class ILGeneratorExtensions
    {
        public static LocalBuilder DeclareLocal(this ILGenerator gen, TypeBuilder tb)
        {
            return gen.DeclareLocal(tb.GetType());
        }

        public static void Emit(this ILGenerator gen, OpCode opcode, TypeBuilder tb)
        {
            gen.Emit(opcode, tb.GetType());
        }

#if DNXCORE50 || DOTNET5_4 || NETSTANDARD1_0
        public static void Emit(this ILGenerator gen, OpCode opcode, GenericTypeParameterBuilder gb)
        {
            gen.Emit(opcode, gb.AsType());
        }
#endif
    }
}