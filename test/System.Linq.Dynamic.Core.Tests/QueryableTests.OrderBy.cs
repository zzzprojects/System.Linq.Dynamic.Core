using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void OrderBy_Dynamic()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var orderById = qry.OrderBy("Id");
            var orderByIdDesc = qry.OrderBy("Id DESC");
            var orderByAge = qry.OrderBy("Profile.Age");
            var orderByAgeDesc = qry.OrderBy("Profile.Age DESC");
            var orderByComplex1 = qry.OrderBy("Profile.Age, Id");
            var orderByComplex2 = qry.OrderBy("Profile.Age DESC, Id");

            //Assert
            Assert.Equal(testList.OrderBy(x => x.Id).ToArray(), orderById.ToArray());
            Assert.Equal(testList.OrderByDescending(x => x.Id).ToArray(), orderByIdDesc.ToArray());

            Assert.Equal(testList.OrderBy(x => x.Profile.Age).ToArray(), orderByAge.ToArray());
            Assert.Equal(testList.OrderByDescending(x => x.Profile.Age).ToArray(), orderByAgeDesc.ToArray());

            Assert.Equal(testList.OrderBy(x => x.Profile.Age).ThenBy(x => x.Id).ToArray(), orderByComplex1.ToArray());
            Assert.Equal(testList.OrderByDescending(x => x.Profile.Age).ThenBy(x => x.Id).ToArray(), orderByComplex2.ToArray());
        }

        [Fact]
        public void OrderBy_Dynamic_AsStringExpression()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var orderById = qry.SelectMany("Roles.OrderBy(Name)").Select("Name");
            var expected = qry.SelectMany(x => x.Roles.OrderBy(y => y.Name)).Select(x => x.Name);

            var orderByIdDesc = qry.SelectMany("Roles.OrderByDescending(Name)").Select("Name");
            var expectedDesc = qry.SelectMany(x => x.Roles.OrderByDescending(y => y.Name)).Select(x => x.Name);

            //Assert
            Assert.Equal(expected.ToArray(), orderById.Cast<string>().ToArray());
            Assert.Equal(expectedDesc.ToArray(), orderByIdDesc.Cast<string>().ToArray());
        }

        [Fact]
        public void OrderBy_Dynamic_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            Assert.Throws<ParseException>(() => qry.OrderBy("Bad=3"));
            Assert.Throws<ParseException>(() => qry.Where("Id=123"));

            Assert.Throws<ArgumentNullException>(() => DynamicQueryableExtensions.OrderBy(null, "Id"));
            Assert.Throws<ArgumentNullException>(() => qry.OrderBy(null));
            Assert.Throws<ArgumentException>(() => qry.OrderBy(""));
            Assert.Throws<ArgumentException>(() => qry.OrderBy(" "));
        }
    }
}