namespace System.Linq.Dynamic.Core.Parser.SupportedOperands
{
    internal interface IAddSignatures : IArithmeticSignatures
    {
        void F(DateTime x, TimeSpan y);
        void F(TimeSpan x, TimeSpan y);
        void F(DateTime? x, TimeSpan? y);
        void F(TimeSpan? x, TimeSpan? y);
    }
}
