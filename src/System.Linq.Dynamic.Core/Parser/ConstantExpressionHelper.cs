using System.Linq.Dynamic.Core.Util.Cache;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser;

internal class ConstantExpressionHelper
{
    // Static shared instance to prevent duplications of the same objects
    private readonly ThreadSafeSlidingCache<object, Expression> _expressions;
    private readonly ThreadSafeSlidingCache<Expression, string> _literals;

    public ConstantExpressionHelper(ParsingConfig config)
    {
        var parsingConfig = Check.NotNull(config);
        var cacheConfig = Check.NotNull(parsingConfig.ConstantExpressionCacheConfig);

        _literals = new ThreadSafeSlidingCache<Expression, string>(
            cacheConfig.TimeToLive,
            cacheConfig.CleanupFrequency,
            cacheConfig.MinItemsTrigger
        );

        _expressions = new ThreadSafeSlidingCache<object, Expression>(
            cacheConfig.TimeToLive,
            cacheConfig.CleanupFrequency,
            cacheConfig.MinItemsTrigger
        );
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