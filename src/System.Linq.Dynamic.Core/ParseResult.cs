using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core
{
    internal class ParseResult
    {
        public Expression Expression1 { get; }

        public Expression? Expression2 { get; }

        public ParseResult(Expression expression1, Expression? expression2 = null)
        {
            Expression1 = expression1;
            Expression2 = expression2;
        }
    }
}