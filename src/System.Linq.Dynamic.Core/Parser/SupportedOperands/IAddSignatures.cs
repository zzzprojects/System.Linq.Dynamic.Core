using System.Diagnostics.CodeAnalysis;

namespace System.Linq.Dynamic.Core.Parser.SupportedOperands;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
internal interface IAddSignatures : IArithmeticSignatures
{
    void F(TimeSpan x, TimeSpan y);

    void F(TimeSpan x, TimeSpan? y);

    void F(TimeSpan? x, TimeSpan y);

    void F(TimeSpan? x, TimeSpan? y);

    void F(DateTime x, TimeSpan y);

    void F(DateTime x, TimeSpan? y);

    void F(DateTime? x, TimeSpan y);

    void F(DateTime? x, TimeSpan? y);
}