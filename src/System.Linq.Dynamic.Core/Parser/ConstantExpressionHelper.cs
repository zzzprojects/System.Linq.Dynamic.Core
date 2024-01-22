using System.Linq.Dynamic.Core.Util.Cache;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser;

internal class ConstantExpressionHelper
{
    private readonly SlidingCache<object, Expression> _expressions;
    private readonly SlidingCache<Expression, string> _literals;

    public ConstantExpressionHelper(ParsingConfig config)
    {
        var parsingConfig = Check.NotNull(config);
        var useConfig = parsingConfig.ConstantExpressionCacheConfig ?? new CacheConfig();

        _literals = new SlidingCache<Expression, string>(useConfig);
        _expressions = new SlidingCache<object, Expression>(useConfig);
    }

    public bool TryGetText(Expression expression, out string? text)
    {
        return _literals.TryGetValue(expression, out text);
    }

    public Expression CreateLiteral(object value, string text)
    {
        if (_expressions.TryGetValue(value, out var outputValue))
        {
            return outputValue;
        }

        var constantExpression = Expression.Constant(value);

        _expressions.AddOrUpdate(value, constantExpression);
        _literals.AddOrUpdate(constantExpression, text);

        return constantExpression;
    }
}