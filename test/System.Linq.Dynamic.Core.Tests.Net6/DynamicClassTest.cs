using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public class DynamicClassTest
    {
        [Fact]
        public void GetPropertiesWorks()
        {
            // Arrange
            var range = new List<object> 
            { 
                new { FieldName = "TestFieldName", Value = 3.14159 }
            };

            // Act
            var rangeResult = range.AsQueryable().Select("new(FieldName as FieldName)").ToDynamicList();
            var item = rangeResult.FirstOrDefault();

            var call = () => item.GetDynamicMemberNames();
            call.Should().NotThrow();
            call.Should().NotThrow();
        }
    }
}