using Moq;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    public class ExpressionPromoterTests
    {
        public class SampleDto
        {
            public Guid data { get; set; }
        }

        private readonly Mock<IExpressionPromoter> _expressionPromoterMock;
        private readonly Mock<IDynamicLinkCustomTypeProvider> _dynamicLinkCustomTypeProviderMock;

        public ExpressionPromoterTests()
        {
            _dynamicLinkCustomTypeProviderMock = new Mock<IDynamicLinkCustomTypeProvider>();
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
            string query = $"new {typeof(SampleDto).FullName}(@0 as data)";
            LambdaExpression expression = DynamicExpressionParser.ParseLambda(parsingConfig, null, query, new object[] { Guid.NewGuid().ToString() });
            Delegate del = expression.Compile();
            SampleDto result = (SampleDto)del.DynamicInvoke();

            // Assert
            Assert.NotNull(result);

            // Verify
            _dynamicLinkCustomTypeProviderMock.Verify(d => d.GetCustomTypes(), Times.Once);
            _dynamicLinkCustomTypeProviderMock.Verify(d => d.ResolveType($"{typeof(SampleDto).FullName}"), Times.Once);

            _expressionPromoterMock.Verify(e => e.Promote(It.IsAny<ConstantExpression>(), typeof(Guid), true, true), Times.Once);
        }
    }
}
