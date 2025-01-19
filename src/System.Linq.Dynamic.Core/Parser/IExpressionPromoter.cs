using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser;

/// <summary>
/// Expression promoter is used to promote object or value types to their destination type when an automatic promotion is available such as: int to int?.
/// </summary>
public interface IExpressionPromoter
{
    /// <summary>
    /// Promote an expression.
    /// </summary>
    /// <param name="sourceExpression">Source expression</param>
    /// <param name="type">Destination data type to promote</param>
    /// <param name="exact">If the match must be exact</param>
    /// <param name="convertExpression">Convert expression</param>
    /// <returns>The promoted <see cref="Expression"/> or <c>null</c>.</returns>
    Expression? Promote(Expression sourceExpression, Type type, bool exact, bool convertExpression);
}