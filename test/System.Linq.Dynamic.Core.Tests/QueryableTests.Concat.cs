using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using System.Reflection;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Concat_ListOfStrings()
        {
            // Arrange
            var list1 = new List<string> { "User0" };
            var list2 = new List<string> { "User3", "User4" };
            var list3 = new List<string> { "User5", "User6", "User7" };

            // Act
            //var realQuery = list1.Concat(list2);
            var testQuery = list1.AsQueryable().Where("@0.Concat(@1) != null", list2, list3);

            // Assert
            var x = testQuery.ToDynamicArray();
            //Assert.Equal(realQuery.ToArray(), testQuery.Cast<Guid>().ToArray());
        }
    }
}
