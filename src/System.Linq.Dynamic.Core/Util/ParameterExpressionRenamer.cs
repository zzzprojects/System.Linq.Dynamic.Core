using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Util;

/// <summary>
/// Renames a single (Typed)ParameterExpression in an Expression.
/// </summary>
/// <seealso cref="ExpressionVisitor" />
internal class ParameterExpressionRenamer : ExpressionVisitor
{
    private readonly string _newName;
    private readonly string _oldName;

    private ParameterExpression? _parameterExpression;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterExpressionRenamer"/> class.
    /// </summary>
    /// <param name="newName">The new name (the oldName is assumed to be "").</param>
    public ParameterExpressionRenamer(string newName) : this(string.Empty, newName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterExpressionRenamer"/> class.
    /// </summary>
    /// <param name="oldName">The old name.</param>
    /// <param name="newName">The new name.</param>
    public ParameterExpressionRenamer(string oldName, string newName)
    {
        _oldName = Check.NotNull(oldName);
        _newName = Check.NotEmpty(newName);
    }

    /// <summary>
    /// Renames a single (Typed)ParameterExpression from specified expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="parameterExpression">The new generated (Typed)ParameterExpression.</param>
    /// <returns>Renamed Expression</returns>
    public Expression Rename(Expression expression, out ParameterExpression? parameterExpression)
    {
        var visitedExpression = Visit(expression);

        parameterExpression = _parameterExpression;

        return visitedExpression;
    }

    /// <inheritdoc cref="ExpressionVisitor.VisitParameter"/>
    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (string.Equals(_oldName, node.Name, StringComparison.Ordinal))
        {
            if (_parameterExpression == null)
            {
                _parameterExpression = ParameterExpressionHelper.CreateParameterExpression(node.Type, _newName);
            }

            return _parameterExpression;
            // throw new InvalidOperationException($"The {nameof(ParameterExpressionRenamer)} can only rename 1 (Typed)ParameterExpression in an Expression.");
        }

        return node;
    }
}