using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void ThenBy_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
			var ordered = qry.OrderBy("Id");
            var thenByUserName = ordered.ThenBy("UserName");
            var thenByComplex1 = ordered.ThenBy("Profile.Age, Income");

            //Assert
            Assert.Equal(testList.OrderBy(x => x.Id).ThenBy(x => x.UserName).ToArray(), thenByUserName.ToArray());
            Assert.Equal(testList.OrderBy(x => x.Id).ThenBy(x => x.Profile.Age).ThenBy(x => x.Income).ToArray(), thenByComplex1.ToArray());
        }

        [Fact]
        public void ThenBy_Dynamic_AsStringExpression()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var expected = qry.SelectMany(x => x.Roles.OrderBy(y => y.Name).ThenBy(y => y.Id)).Select(x => x.Name);
            var orderById = qry.SelectMany("Roles.OrderBy(Name).ThenBy(Id)").Select("Name");

            //Assert
            Assert.Equal(expected.ToArray(), orderById.Cast<string>().ToArray());
        }

        [Fact]
        public void ThenBy_Dynamic_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            var ordered = qry.OrderBy("Id");
            Assert.Throws<ParseException>(() => ordered.ThenBy("Bad=3"));
            Assert.Throws<ParseException>(() => ordered.Where("Id=123"));

            Assert.Throws<ArgumentNullException>(() => DynamicQueryableExtensions.ThenBy(null, "Id"));
            Assert.Throws<ArgumentNullException>(() => ordered.ThenBy(null));
            Assert.Throws<ArgumentException>(() => ordered.ThenBy(""));
            Assert.Throws<ArgumentException>(() => ordered.ThenBy(" "));
        }
    }
}