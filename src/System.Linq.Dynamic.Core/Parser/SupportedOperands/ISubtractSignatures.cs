using System.Diagnostics.CodeAnalysis;

namespace System.Linq.Dynamic.Core.Parser.SupportedOperands;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
internal interface ISubtractSignatures : IAddSignatures
{
    void F(DateTime x, DateTime y);
    
    void F(DateTime x, DateTime? y);

    void F(DateTime? x, DateTime y);

    void F(DateTime? x, DateTime? y);
}