using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void OrderBy_Dynamic_IComparer()
        {
            // Arrange
            var testList = User.GenerateSampleModels(2);
            var qry = testList.AsQueryable();

            // Act
            var orderBy = testList.OrderBy(x => x.UserName, StringComparer.OrdinalIgnoreCase).ToArray();
            var orderByDynamic = qry.OrderBy("UserName", StringComparer.OrdinalIgnoreCase).ToArray();

            // Assert
            Assert.Equal(orderBy, orderByDynamic);
        }

        [Fact]
        public void OrderBy_Dynamic()
        {
            // Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            // Act
            var orderById = qry.OrderBy("Id");
            var orderByAge = qry.OrderBy("Profile.Age");
            var orderByComplex1 = qry.OrderBy("Profile.Age, Id");

            // Assert
            Assert.Equal(testList.OrderBy(x => x.Id).ToArray(), orderById.ToArray());
            Assert.Equal(testList.OrderBy(x => x.Profile.Age).ToArray(), orderByAge.ToArray());
            Assert.Equal(testList.OrderBy(x => x.Profile.Age).ThenBy(x => x.Id).ToArray(), orderByComplex1.ToArray());
        }

        [Fact]
        public void OrderBy_Dynamic_AsStringExpression()
        {
            // Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            // Act
            var expected = qry.SelectMany(x => x.Roles.OrderBy(y => y.Name)).Select(x => x.Name);
            var orderById = qry.SelectMany("Roles.OrderBy(Name)").Select("Name");

            // Assert
            Assert.Equal(expected.ToArray(), orderById.Cast<string>().ToArray());
        }

        [Fact]
        public void OrderBy_Dynamic_Exceptions()
        {
            // Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            // Act
            Assert.Throws<ParseException>(() => qry.OrderBy("Bad=3"));
            Assert.Throws<ParseException>(() => qry.Where("Id=123"));

            Assert.Throws<ArgumentNullException>(() => DynamicQueryableExtensions.OrderBy(null, "Id"));
            Assert.Throws<ArgumentNullException>(() => qry.OrderBy(null));
            Assert.Throws<ArgumentException>(() => qry.OrderBy(""));
            Assert.Throws<ArgumentException>(() => qry.OrderBy(" "));
        }
    }
}
