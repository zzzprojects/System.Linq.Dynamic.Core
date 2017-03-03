using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Contains_Dynamic_StringList()
        {
            //Arrange
            var baseQuery = User.GenerateSampleModels(100).AsQueryable();
            var list = new List<string> { "User1", "User5", "User10" };

            //Act
            var realQuery = baseQuery.Where(x => list.Contains(x.UserName)).Select(x => x.Id);
            var testQuery = baseQuery.Where("@0.Contains(UserName)", list).Select("Id");

            //Assert
            Assert.Equal(realQuery.ToArray(), testQuery.Cast<Guid>().ToArray());
        }
    }
}