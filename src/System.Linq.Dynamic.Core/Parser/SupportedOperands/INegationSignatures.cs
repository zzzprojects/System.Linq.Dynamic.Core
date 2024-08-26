using System.Diagnostics.CodeAnalysis;

namespace System.Linq.Dynamic.Core.Parser.SupportedOperands;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
internal interface INegationSignatures
{
    void F(int x);
    void F(long x);
    void F(float x);
    void F(double x);
    void F(decimal x);
    void F(int? x);
    void F(long? x);
    void F(float? x);
    void F(double? x);
    void F(decimal? x);
}