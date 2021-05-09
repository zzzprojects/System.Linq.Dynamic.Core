using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Intersect_Dynamic_ListOfStrings()
        {
            // Arrange
            var list1 = new List<bool> { true };
            var list2 = new List<string> { "User3", "User4" };
            var list3 = new List<string> { "User3", "User6", "User7" };

            // Act
            var testQuery = list1.AsQueryable().Select("@0.Intersect(@1).ToList()", list2, list3);

            // Assert
            var result = testQuery.ToDynamicArray<List<string>>();
            result.First().Should().BeEquivalentTo(list2.Intersect(list3));
        }
    }
}
