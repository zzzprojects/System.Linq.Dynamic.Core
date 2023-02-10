namespace System.Linq.Dynamic.Core.Parser.SupportedOperands;

internal interface IAddSignatures : IArithmeticSignatures
{
    void F(TimeSpan x, TimeSpan y);

    void F(TimeSpan? x, TimeSpan? y);

    void F(DateTime x, TimeSpan y);
    
    void F(DateTime? x, TimeSpan? y);

#if NET6_0_OR_GREATER
    void F(DateOnly x, TimeSpan y);

    void F(DateOnly? x, TimeSpan? y);
#endif
}