using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void OrderByDescending_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var orderByIdDesc = qry.OrderBy("Id DESC");
            var orderByAgeDesc = qry.OrderBy("Profile.Age DESC");
            var orderByComplex2 = qry.OrderBy("Profile.Age DESC, Id");

            //Assert
            Assert.Equal(testList.OrderByDescending(x => x.Id).ToArray(), orderByIdDesc.ToArray());
            Assert.Equal(testList.OrderByDescending(x => x.Profile.Age).ToArray(), orderByAgeDesc.ToArray());
            Assert.Equal(testList.OrderByDescending(x => x.Profile.Age).ThenBy(x => x.Id).ToArray(), orderByComplex2.ToArray());
        }

        [Fact]
        public void OrderByDescending_Dynamic_AsStringExpression()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var expectedDesc = qry.SelectMany(x => x.Roles.OrderByDescending(y => y.Name)).Select(x => x.Name);
            var orderByIdDesc = qry.SelectMany("Roles.OrderByDescending(Name)").Select("Name");

            //Assert
            Assert.Equal(expectedDesc.ToArray(), orderByIdDesc.Cast<string>().ToArray());
        }

        [Theory]
        [InlineData(KeywordsHelper.KEYWORD_IT)]
        [InlineData(KeywordsHelper.SYMBOL_IT)]
        [InlineData(KeywordsHelper.KEYWORD_ROOT)]
        [InlineData(KeywordsHelper.SYMBOL_ROOT)]
        [InlineData("\"User\" + \"Name\"")]
        [InlineData("\"User\" + \"Name\" asc")]
        [InlineData("\"User\" + \"Name\" desc")]
        public void Entities_OrderBy_RestrictOrderByIsTrue_NonRestrictedExpressionShouldNotThrow(string expression)
        {
            // Arrange
            var queryable = User.GenerateSampleModels(3).AsQueryable();

            // Act
            Action action = () => _ = queryable.OrderBy(expression);

            // Assert 2
            action.Should().NotThrow();
        }
    }
}