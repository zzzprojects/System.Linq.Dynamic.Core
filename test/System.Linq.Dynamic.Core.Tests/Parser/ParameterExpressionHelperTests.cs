﻿using NFluent;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.Parser
{
    public class ParameterExpressionHelperTests
    {
        [Theory]
        [InlineData(typeof(int), "it", "x", "x", false)]
        [InlineData(typeof(int), "it == 1", @"\(x == 1\)", "x", false)]
        [InlineData(typeof(int), "it eq 1", @"\(x == 1\)", "x", false)]
        [InlineData(typeof(int), "it equal 1", @"\(x == 1\)", "x", false)]
        [InlineData(typeof(int), "it != 1", @"\(x != 1\)", "x", false)]
        [InlineData(typeof(int), "it ne 1", @"\(x != 1\)", "x", false)]
        [InlineData(typeof(int), "it neq 1", @"\(x != 1\)", "x", false)]
        [InlineData(typeof(int), "it notequal 1", @"\(x != 1\)", "x", false)]
        [InlineData(typeof(int), "it lt 1", @"\(x < 1\)", "x", false)]
        [InlineData(typeof(int), "it LessThan 1", @"\(x < 1\)", "x", false)]
        [InlineData(typeof(int), "it le 1", @"\(x <= 1\)", "x", false)]
        [InlineData(typeof(int), "it LessThanEqual 1", @"\(x <= 1\)", "x", false)]
        [InlineData(typeof(int), "it gt 1", @"\(x > 1\)", "x", false)]
        [InlineData(typeof(int), "it GreaterThan 1", @"\(x > 1\)", "x", false)]
        [InlineData(typeof(int), "it ge 1", @"\(x >= 1\)", "x", false)]
        [InlineData(typeof(int), "it GreaterThanEqual 1", @"\(x >= 1\)", "x", false)]

        [InlineData(typeof(int), "it", "x", "x", true)]
        [InlineData(typeof(int), "it == 1", @"\(x == 1\)", "x", true)]
        [InlineData(typeof(int), "it eq 1", @"\(x == 1\)", "x", true)]
        [InlineData(typeof(int), "it equal 1", @"\(x == 1\)", "x", true)]
        [InlineData(typeof(int), "it != 1", @"\(x != 1\)", "x", true)]
        [InlineData(typeof(int), "it ne 1", @"\(x != 1\)", "x", true)]
        [InlineData(typeof(int), "it neq 1", @"\(x != 1\)", "x", true)]
        [InlineData(typeof(int), "it notequal 1", @"\(x != 1\)", "x", true)]
        [InlineData(typeof(int), "it lt 1", @"\(x < 1\)", "x", true)]
        [InlineData(typeof(int), "it LessThan 1", @"\(x < 1\)", "x", true)]
        [InlineData(typeof(int), "it le 1", @"\(x <= 1\)", "x", true)]
        [InlineData(typeof(int), "it LessThanEqual 1", @"\(x <= 1\)", "x", true)]
        [InlineData(typeof(int), "it gt 1", @"\(x > 1\)", "x", true)]
        [InlineData(typeof(int), "it GreaterThan 1", @"\(x > 1\)", "x", true)]
        [InlineData(typeof(int), "it ge 1", @"\(x >= 1\)", "x", true)]
        [InlineData(typeof(int), "it GreaterThanEqual 1", @"\(x >= 1\)", "x", true)]

        [InlineData(typeof(int), "it", "Param_0", "", false)]
        [InlineData(typeof(int), "it == 1", @"\(Param_0 == 1\)", "", false)]
        [InlineData(typeof(int), "it eq 1", @"\(Param_0 == 1\)", "", false)]
        [InlineData(typeof(int), "it equal 1", @"\(Param_0 == 1\)", "", false)]
        [InlineData(typeof(int), "it != 1", @"\(Param_0 != 1\)", "", false)]
        [InlineData(typeof(int), "it ne 1", @"\(Param_0 != 1\)", "", false)]
        [InlineData(typeof(int), "it neq 1", @"\(Param_0 != 1\)", "", false)]
        [InlineData(typeof(int), "it notequal 1", @"\(Param_0 != 1\)", "", false)]
        [InlineData(typeof(int), "it lt 1", @"\(Param_0 < 1\)", "", false)]
        [InlineData(typeof(int), "it LessThan 1", @"\(Param_0 < 1\)", "", false)]
        [InlineData(typeof(int), "it le 1", @"\(Param_0 <= 1\)", "", false)]
        [InlineData(typeof(int), "it LessThanEqual 1", @"\(Param_0 <= 1\)", "", false)]
        [InlineData(typeof(int), "it gt 1", @"\(Param_0 > 1\)", "", false)]
        [InlineData(typeof(int), "it GreaterThan 1", @"\(Param_0 > 1\)", "", false)]
        [InlineData(typeof(int), "it ge 1", @"\(Param_0 >= 1\)", "", false)]
        [InlineData(typeof(int), "it GreaterThanEqual 1", @"\(Param_0 >= 1\)", "", false)]

        [InlineData(typeof(int), "it", "[a-z]{6}", "", true)]
        [InlineData(typeof(int), "it == 1", @"\([a-z]{6} == 1\)", "", true)]
        [InlineData(typeof(int), "it eq 1", @"\([a-z]{6} == 1\)", "", true)]
        [InlineData(typeof(int), "it equal 1", @"\([a-z]{6} == 1\)", "", true)]
        [InlineData(typeof(int), "it != 1", @"\([a-z]{6} != 1\)", "", true)]
        [InlineData(typeof(int), "it ne 1", @"\([a-z]{6} != 1\)", "", true)]
        [InlineData(typeof(int), "it neq 1", @"\([a-z]{6} != 1\)", "", true)]
        [InlineData(typeof(int), "it notequal 1", @"\([a-z]{6} != 1\)", "", true)]
        [InlineData(typeof(int), "it lt 1", @"\([a-z]{6} < 1\)", "", true)]
        [InlineData(typeof(int), "it LessThan 1", @"\([a-z]{6} < 1\)", "", true)]
        [InlineData(typeof(int), "it le 1", @"\([a-z]{6} <= 1\)", "", true)]
        [InlineData(typeof(int), "it LessThanEqual 1", @"\([a-z]{6} <= 1\)", "", true)]
        [InlineData(typeof(int), "it gt 1", @"\([a-z]{6} > 1\)", "", true)]
        [InlineData(typeof(int), "it GreaterThan 1", @"\([a-z]{6} > 1\)", "", true)]
        [InlineData(typeof(int), "it ge 1", @"\([a-z]{6} >= 1\)", "", true)]
        [InlineData(typeof(int), "it GreaterThanEqual 1", @"\([a-z]{6} >= 1\)", "", true)]

        [InlineData(typeof(bool), "it || true", @"\(x OrElse True\)", "x", false)]
        [InlineData(typeof(bool), "it or true", @"\(x OrElse True\)", "x", false)]
        [InlineData(typeof(bool), "it OrElse true", @"\(x OrElse True\)", "x", false)]
        [InlineData(typeof(bool), "it || true", @"\(x OrElse True\)", "x", true)]
        [InlineData(typeof(bool), "it or true", @"\(x OrElse True\)", "x", true)]
        [InlineData(typeof(bool), "it OrElse true", @"\(x OrElse True\)", "x", true)]
        [InlineData(typeof(bool), "it || true", @"\(Param_0 OrElse True\)", "", false)]
        [InlineData(typeof(bool), "it or true", @"\(Param_0 OrElse True\)", "", false)]
        [InlineData(typeof(bool), "it OrElse true", @"\(Param_0 OrElse True\)", "", false)]
        [InlineData(typeof(bool), "it || true", @"\([a-z]{6} OrElse True\)", "", true)]
        [InlineData(typeof(bool), "it or true", @"\([a-z]{6} OrElse True\)", "", true)]
        [InlineData(typeof(bool), "it OrElse true", @"\([a-z]{6} OrElse True\)", "", true)]

        [InlineData(typeof(bool), "it && true", @"\(x AndAlso True\)", "x", false)]
        [InlineData(typeof(bool), "it and true", @"\(x AndAlso True\)", "x", false)]
        [InlineData(typeof(bool), "it AndAlso true", @"\(x AndAlso True\)", "x", false)]
        [InlineData(typeof(bool), "it && true", @"\(x AndAlso True\)", "x", true)]
        [InlineData(typeof(bool), "it and true", @"\(x AndAlso True\)", "x", true)]
        [InlineData(typeof(bool), "it AndAlso true", @"\(x AndAlso True\)", "x", true)]
        [InlineData(typeof(bool), "it && true", @"\(Param_0 AndAlso True\)", "", false)]
        [InlineData(typeof(bool), "it and true", @"\(Param_0 AndAlso True\)", "", false)]
        [InlineData(typeof(bool), "it AndAlso true", @"\(Param_0 AndAlso True\)", "", false)]
        [InlineData(typeof(bool), "it && true", @"\([a-z]{6} AndAlso True\)", "", true)]
        [InlineData(typeof(bool), "it and true", @"\([a-z]{6} AndAlso True\)", "", true)]
        [InlineData(typeof(bool), "it AndAlso true", @"\([a-z]{6} AndAlso True\)", "", true)]
        public void ParameterExpressionHelper_CreateParameterExpression(Type type, string expression, string result, string substitute, bool renameEmpty)
        {
            // Arrange
            ParameterExpression[] parameters = { ParameterExpressionHelper.CreateParameterExpression(type, substitute, renameEmpty) };
            var sut = new ExpressionParser(parameters, expression, null, null);

            // Act
            var parsedExpression = sut.Parse(null).ToString();

            // Assert
            Check.That(parsedExpression).Matches(result);
        }
    }
}
