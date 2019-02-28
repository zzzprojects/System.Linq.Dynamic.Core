using NFluent;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    public class ConstantExpressionWrapperTests
    {
        private readonly IConstantExpressionWrapper _constantExpressionWrapper;

        public ConstantExpressionWrapperTests()
        {
            _constantExpressionWrapper = new ConstantExpressionWrapper();
        }

        [Theory]
        [InlineData(true)]
        [InlineData((int)1)]
        [InlineData((uint)2)]
        [InlineData((short)3)]
        [InlineData((ushort)4)]
        [InlineData(5L)]
        [InlineData(6UL)]
        [InlineData(7.1f)]
        [InlineData(8.1d)]
        [InlineData('c')]
        [InlineData((byte)10)]
        [InlineData((sbyte)11)]
        public void ConstantExpressionWrapper_Wrap_ConstantExpression_PrimitiveTypes<T>(T test)
        {
            // Assign
            var expression = Expression.Constant(test) as Expression;

            // Act
            _constantExpressionWrapper.Wrap(ref expression);

            // Verify
            Check.That(expression).IsNotNull();

            ConstantExpression constantExpression = (expression as MemberExpression).Expression as ConstantExpression;
            dynamic wrappedObj = constantExpression.Value;

            T value = wrappedObj.Value;

            Check.That(value).IsEqualTo(test);
        }

        [Fact]
        public void ConstantExpressionWrapper_Wrap_ConstantExpression_String()
        {
            // Assign
            string test = "abc";
            var expression = Expression.Constant(test) as Expression;

            // Act
            _constantExpressionWrapper.Wrap(ref expression);

            // Verify
            Check.That(expression).IsNotNull();

            ConstantExpression constantExpression = (expression as MemberExpression).Expression as ConstantExpression;
            dynamic wrappedObj = constantExpression.Value;

            string value = wrappedObj.Value;

            Check.That(value).IsEqualTo(test);
        }

        [Fact]
        public void ConstantExpressionWrapper_Wrap_ConstantExpression_ComplexTypes()
        {
            // Assign
            var test = DateTime.Now;
            var expression = Expression.Constant(test) as Expression;

            // Act
            _constantExpressionWrapper.Wrap(ref expression);

            // Verify
            Check.That(expression).IsNotNull();

            var constantExpression = (expression as MemberExpression).Expression as ConstantExpression;
            dynamic wrappedObj = constantExpression.Value;

            DateTime value = wrappedObj.Value;

            Check.That(value).IsEqualTo(test);
        }
    }
}
