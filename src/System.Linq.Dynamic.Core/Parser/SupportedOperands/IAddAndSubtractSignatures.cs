namespace System.Linq.Dynamic.Core.Parser.SupportedOperands;

internal interface IAddAndSubtractSignatures : IArithmeticSignatures
{
    void F(TimeSpan x, TimeSpan y);

    void F(TimeSpan x, TimeSpan? y);

    void F(TimeSpan? x, TimeSpan y);

    void F(TimeSpan? x, TimeSpan? y);

    void F(DateTime x, DateTime y);
    
    void F(DateTime x, DateTime? y);

    void F(DateTime? x, DateTime y);

    void F(DateTime? x, DateTime? y);

#if NET6_0_OR_GREATER
    void F(DateOnly x, DateOnly y);

    void F(DateOnly x, DateOnly? y);

    void F(DateOnly? x, DateOnly y);

    void F(DateOnly? x, DateOnly? y);

    void F(TimeOnly x, TimeOnly y);

    void F(TimeOnly x, TimeOnly? y);

    void F(TimeOnly? x, TimeOnly y);

    void F(TimeOnly? x, TimeOnly? y);
#endif
}