using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser
{
    internal interface IConstantExpressionWrapper
    {
        void Wrap(ref Expression expression);
    }
}
