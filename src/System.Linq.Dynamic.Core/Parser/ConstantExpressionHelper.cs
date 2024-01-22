using System.Linq.Dynamic.Core.Util.Cache;
using System.Linq.Expressions;

namespace System.Linq.Dynamic.Core.Parser;

internal class ConstantExpressionHelper
{
    private readonly ParsingConfig _config;

    // Static shared instance to prevent duplications of the same objects
    private static ThreadSafeSlidingCache<object, Expression>? _expressions;
    private static ThreadSafeSlidingCache<Expression, string>? _literals;

    public ConstantExpressionHelper(ParsingConfig config)
    {
        _config = config;
        
    }

    private ThreadSafeSlidingCache<Expression, string> GetLiterals()
    {
        _literals ??= new ThreadSafeSlidingCache<Expression, string>(
            _config.ConstantExpressionSlidingCacheTimeToLive,
            _config.ConstantExpressionSlidingCacheCleanupFrequency,
            _config.ConstantExpressionSlidingCacheMinItemsTrigger
        );
        return _literals;
    }

    private ThreadSafeSlidingCache<object, Expression> GetExpression()
    {
        _expressions ??= new ThreadSafeSlidingCache<object, Expression>(
            _config.ConstantExpressionSlidingCacheTimeToLive,
            _config.ConstantExpressionSlidingCacheCleanupFrequency,
            _config.ConstantExpressionSlidingCacheMinItemsTrigger
        );
        return _expressions;
    }


    public bool TryGetText(Expression expression, out string? text)
    {
        return GetLiterals().TryGetValue(expression, out text);
    }

    public Expression CreateLiteral(object value, string text)
    {
        if (GetExpression().TryGetValue(value, out var outputValue))
        {
            return outputValue;
        }

        var constantExpression = Expression.Constant(value);

        GetExpression().AddOrUpdate(value, constantExpression);
        GetLiterals().AddOrUpdate(constantExpression, text);

        return constantExpression;
    }
}