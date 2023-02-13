using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser;

internal interface IConstantExpressionWrapper
{
    void Wrap(ref Expression expression);

    bool TryUnwrapAsValue<TValue>(MemberExpression? expression, [NotNullWhen(true)] out TValue? value);

    bool TryUnwrapAsConstantExpression<TValue>(MemberExpression? expression, [NotNullWhen(true)] out ConstantExpression? value);
}