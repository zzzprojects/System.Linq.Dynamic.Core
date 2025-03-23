using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser;

public class ExpressionPromoterTests
{
    public class SampleDto
    {
        public Guid Data { get; set; }
    }

    private readonly Mock<IExpressionPromoter> _expressionPromoterMock;
    private readonly Mock<IDynamicLinqCustomTypeProvider> _dynamicLinkCustomTypeProviderMock;

    public ExpressionPromoterTests()
    {
        _dynamicLinkCustomTypeProviderMock = new Mock<IDynamicLinqCustomTypeProvider>();
        _dynamicLinkCustomTypeProviderMock.Setup(d => d.GetCustomTypes()).Returns(new HashSet<Type>());
        _dynamicLinkCustomTypeProviderMock.Setup(d => d.ResolveType(It.IsAny<string>())).Returns(typeof(SampleDto));

        _expressionPromoterMock = new Mock<IExpressionPromoter>();
        _expressionPromoterMock.Setup(e => e.Promote(It.IsAny<Expression>(), It.IsAny<Type>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(Expression.Constant(Guid.NewGuid()));
    }

    [Fact]
    public void DynamicExpressionParser_ParseLambda_WithCustomExpressionPromoter()
    {
        // Assign
        var parsingConfig = new ParsingConfig()
        {
            AllowNewToEvaluateAnyType = true,
            CustomTypeProvider = _dynamicLinkCustomTypeProviderMock.Object,
            ExpressionPromoter = _expressionPromoterMock.Object
        };

        // Act
        string query = $"new {typeof(SampleDto).FullName}(@0 as Data)";
        LambdaExpression expression = DynamicExpressionParser.ParseLambda(parsingConfig, null, query, Guid.NewGuid().ToString());
        Delegate del = expression.Compile();
        SampleDto result = (SampleDto)del.DynamicInvoke();

        // Assert
        Assert.NotNull(result);

        // Verify
        _dynamicLinkCustomTypeProviderMock.Verify(d => d.GetCustomTypes(), Times.Once);
        _dynamicLinkCustomTypeProviderMock.Verify(d => d.ResolveType($"{typeof(SampleDto).FullName}"), Times.Once);

        _expressionPromoterMock.Verify(e => e.Promote(It.IsAny<ConstantExpression>(), typeof(Guid), true, true), Times.Once);
    }

    [Fact]
    public async Task Promote_Should_Succeed_Even_When_LiteralsCache_Is_Cleaned()
    {
        // Arrange
        var parsingConfig = new ParsingConfig()
        {
            ConstantExpressionCacheConfig = new Core.Util.Cache.CacheConfig
            {
                CleanupFrequency = TimeSpan.FromMilliseconds(500), // Run cleanup more often
                TimeToLive = TimeSpan.FromMilliseconds(500), // Shorten TTL to force expiration
                ReturnExpiredItems = false
            }
        };
        var constantExpressionHelper = ConstantExpressionHelperFactory.GetInstance(parsingConfig);
        var expressionPromoter = new ExpressionPromoter(parsingConfig);

        double value = 0.40;
        string text = "0.40";
        Type targetType = typeof(decimal);

        // Step 1: Add constant to cache
        var literalExpression = constantExpressionHelper.CreateLiteral(value, text);
        Assert.NotNull(literalExpression); // Ensure it was added

        // Step 2: Manually trigger cleanup
        var cts = new CancellationTokenSource(500);
        await Task.Run(async () =>
        {
            while (!cts.IsCancellationRequested)
            {
                constantExpressionHelper.TryGetText(literalExpression, out _);
                await Task.Delay(50); // Give some time for cleanup to be triggered
            }
        });

        // Ensure some cleanup cycles have passed
        await Task.Delay(500); // Allow cache cleanup to happen

        // Step 3: Attempt to promote the expression after cleanup
        var promotedExpression = expressionPromoter.Promote(literalExpression, targetType, exact: false, convertExpression: true);

        // Assert: Promotion should still work even if the cache was cleaned
        promotedExpression.Should().NotBeNull(); // Ensure `Promote()` still returns a valid expression
    }
}