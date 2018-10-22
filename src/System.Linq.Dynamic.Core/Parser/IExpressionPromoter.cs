using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser
{
    /// <summary>
    /// Expression promoter is used whe matching
    /// </summary>
    public interface IExpressionPromoter
    {
        /// <summary>
        /// Promote an expression
        /// </summary>
        /// <param name="expr">Source expression.</param>
        /// <param name="type">Destionation data type to promote to.</param>
        /// <param name="exact">If the match must be exact.</param>
        /// <param name="convertExpr">Convert expression.</param>
        /// <returns></returns>
        Expression Promote(Expression expr, Type type, bool exact, bool convertExpr);
    }
}
