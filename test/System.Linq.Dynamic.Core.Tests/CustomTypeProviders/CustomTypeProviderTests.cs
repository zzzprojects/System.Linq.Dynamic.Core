using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.CustomTypeProviders
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public class CustomTypeProviderTests
    {
        private readonly ParsingConfig _config = new()
        {
            CustomTypeProvider = new MyCustomTypeProvider(ParsingConfig.Default, typeof(IS), typeof(IS.NOT), typeof(IST), typeof(IST.NOT)),
            IsCaseSensitive = true
        };

        [Theory]
        [InlineData("IS.NULL(null)", true)]
        [InlineData("IS.NOT.NULL(null)", false)]
        [InlineData("IST.NULL(null)", true)]
        [InlineData("IST.NOT.NULL(null)", false)]
        public void Test(string expression, bool expected)
        {
            // Act 1
            var lambdaExpression = DynamicExpressionParser.ParseLambda(
                _config,
                false,
                [],
                typeof(bool),
                expression
            );

            // Act 2
            var result = (bool?)lambdaExpression.Compile().DynamicInvoke();

            // Assert
            result.Should().Be(expected);
        }

        public class MyCustomTypeProvider : DefaultDynamicLinqCustomTypeProvider
        {
            private readonly HashSet<Type> _types;

            public MyCustomTypeProvider(ParsingConfig config, params Type[] types) : base(config)
            {
                _types = new HashSet<Type>(types);
            }

            public override HashSet<Type> GetCustomTypes()
            {
                return _types;
            }
        }

        public static class IS
        {
            public static bool NULL(object? value) => value == null;

            public static class NOT
            {
                public static bool NULL(object? value) => value != null;
            }
        }

        public static class IST
        {
            public static bool NULL(object? value) => value == null;

            public static class NOT
            {
                public static bool NULL(object? value) => value != null;
            }
        }
    }
}