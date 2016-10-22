using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class EntitiesTests
    {
        [Fact]
        public void Entities_DynamicEnumerableExtensions_ToDynamicArray()
        {
            //Arrange
            var list = new List<SimpleValuesModel>();
            list.Add(new SimpleValuesModel { IntValue = 1 });
            list.Add(new SimpleValuesModel { IntValue = 2 });

            //Act
            var expected = list.ToArray() as object[];
            var result = DynamicEnumerableExtensions.ToDynamicArray(list);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Entities_DynamicEnumerableExtensions_ToDynamicArray_Type1()
        {
            //Arrange
            var list = new List<SimpleValuesModel>();
            list.Add(new SimpleValuesModel { IntValue = 1 });
            list.Add(new SimpleValuesModel { IntValue = 2 });

            //Act
            var expected = list.ToArray();
            var result = DynamicEnumerableExtensions.ToDynamicArray<SimpleValuesModel>(list);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Entities_DynamicEnumerableExtensions_ToDynamicArray_Type2()
        {
            //Arrange
            var list = new List<SimpleValuesModel>();
            list.Add(new SimpleValuesModel { IntValue = 1 });
            list.Add(new SimpleValuesModel { IntValue = 2 });

            //Act
            var expected = list.ToArray();
            var result = DynamicEnumerableExtensions.ToDynamicArray(list, typeof(SimpleValuesModel));

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Entities_DynamicEnumerableExtensions_ToDynamicList()
        {
            //Arrange
            var list = new List<SimpleValuesModel>();
            list.Add(new SimpleValuesModel { IntValue = 1 });
            list.Add(new SimpleValuesModel { IntValue = 2 });

            //Act
            var expected = list.Select(sv => (object)sv);
            var result = DynamicEnumerableExtensions.ToDynamicList(list);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Entities_DynamicEnumerableExtensions_ToDynamicList_Type1()
        {
            //Arrange
            var list = new List<SimpleValuesModel>();
            list.Add(new SimpleValuesModel { IntValue = 1 });
            list.Add(new SimpleValuesModel { IntValue = 2 });

            //Act
            var expected = list;
            var result = DynamicEnumerableExtensions.ToDynamicList<SimpleValuesModel>(list);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Entities_DynamicEnumerableExtensions_ToDynamicList_Type2()
        {
            //Arrange
            var list = new List<SimpleValuesModel>();
            list.Add(new SimpleValuesModel { IntValue = 1 });
            list.Add(new SimpleValuesModel { IntValue = 2 });

            //Act
            var expected = list;
            var result = DynamicEnumerableExtensions.ToDynamicList(list, typeof(SimpleValuesModel));

            //Assert
            Assert.Equal(expected, result);
        }
    }
}