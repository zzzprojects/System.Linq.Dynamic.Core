using System.Diagnostics.CodeAnalysis;
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
        var cacheConfig = parsingConfig.ConstantExpressionCacheConfig ?? new CacheConfig();

        _literals = new SlidingCache<Expression, string>(cacheConfig);
        _expressions = new SlidingCache<object, Expression>(cacheConfig);
    }

    public bool TryGetText(Expression expression, [NotNullWhen(true)] out string? text)
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