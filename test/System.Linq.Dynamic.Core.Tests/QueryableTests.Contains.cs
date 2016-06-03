using System.Collections.Generic;
//using System.Linq;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        //[Fact]
        //public void Contains()
        //{
        //    const int search = 2;
        //    //Arrange
        //    var queryable = new[] { 1, 2, 3, 4, 5 }.AsQueryable();

        //    //Act
        //    bool expected = queryable.Contains(search);
        //    bool result = queryable.Contains(search);

        //    //Assert
        //    Assert.Equal(expected, result);
        //}

        [Fact]
        public void Contains_Dynamic_Args()
        {
            //Arrange
            var baseQuery = User.GenerateSampleModels(100).AsQueryable();
            var containsList = new List<string>() { "User1", "User5", "User10" };

            //Act
            var realQuery = baseQuery.Where(x => containsList.Contains(x.UserName)).Select(x => x.Id);
            var testQuery = baseQuery.Where("@0.Contains(UserName)", containsList).Select("Id");

            //Assert
            Assert.Equal(realQuery.ToArray(), testQuery.Cast<Guid>().ToArray());
        }
    }
}