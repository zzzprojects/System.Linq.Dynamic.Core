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
        public void Contains_Dynamic_ListWithStrings()
        {
            // Arrange
            var baseQuery = User.GenerateSampleModels(100).AsQueryable();
            var list = new List<string> { "User1", "User5", "User10" };

            // Act
            var realQuery = baseQuery.Where(x => list.Contains(x.UserName)).Select(x => x.Id);
            var testQuery = baseQuery.Where("@0.Contains(UserName)", list).Select("Id");

            // Assert
            Assert.Equal(realQuery.ToArray(), testQuery.Cast<Guid>().ToArray());
        }

#if NET48 || NET5_0 || NET6_0_OR_GREATER
        [Fact]
        [Trait("Issue", "130")]
        public void Contains_Dynamic_ListWithDynamicObjects()
        {
            // Arrange
            var baseQuery = User.GenerateSampleModels(100).AsQueryable();
            var list = new List<dynamic> { new { UserName = "User1" } };

            var keyType = DynamicClassFactory.CreateType(new[] { new DynamicProperty("UserName", typeof(string)) });
            var keyVals = (IList)CreateGenericInstance(typeof(List<>), new[] { keyType });

            var keyVal = Activator.CreateInstance(keyType);
            keyType.GetProperty("UserName").SetValue(keyVal, "User1");
            keyVals.Add(keyVal);

            // Act
            var realQuery = baseQuery.Where(x => list.Contains(new { UserName = x.UserName })).Select(x => x.Id);
            var testQuery = baseQuery.Where("@0.Contains(new(it.UserName as UserName))", keyVals).Select("Id");

            // Assert
            Assert.Equal(realQuery.ToArray(), testQuery.Cast<Guid>().ToArray());
        }

        private object CreateGenericInstance(Type type, Type[] types, params dynamic[] ctorParams)
        {
            Type genType = type.MakeGenericType(types);

            var constructor = genType.GetConstructors().First();
            return constructor.Invoke(ctorParams);
        }
#endif
    }
}
