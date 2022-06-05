using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void SelectMany_Dynamic()
        {
            // Act
            var users = User.GenerateSampleModels(2);
            users[0].Roles = new List<Role>
            {
                new Role { Name = "Admin", Permissions = new List<Permission> {new Permission {Name = "p-Admin"}, new Permission {Name = "p-User"}}
                }
            };
            users[1].Roles = new List<Role>
            {
                new Role {Name = "Guest", Permissions = new List<Permission> {new Permission {Name = "p-Guest"}}}
            };

            var query = users.AsQueryable();

            // Assign
            var queryNormal = query.SelectMany(u => u.Roles.SelectMany(r => r.Permissions)).Select(p => p.Name).ToList();
            var queryDynamic = query.SelectMany("Roles.SelectMany(Permissions)").Select("Name").ToDynamicList<string>();

            // Assert
            Assert.Equal(queryNormal, queryDynamic);
        }

        /// <summary>
        /// https://github.com/NArnott/System.Linq.Dynamic/issues/42 and https://github.com/StefH/System.Linq.Dynamic.Core/issues/18
        /// </summary>
        [Fact]
        public void SelectMany_Dynamic_OverArray()
        {
            var testList = new[]
            {
                new[] {1},
                new[] {1, 2},
                new[] {1, 2, 3},
                new[] {1, 2, 3, 4},
                new[] {1, 2, 3, 4, 5}
            };

            var expectedResult = testList.SelectMany(it => it).ToList();
            var result = testList.AsQueryable().SelectMany("it").ToDynamicList<int>();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void SelectMany_TResult()
        {
            // Act
            var users = User.GenerateSampleModels(2);
            users[0].Roles = new List<Role>
            {
                new Role { Name = "Admin", Permissions = new List<Permission> {new Permission {Name = "p-Admin"}, new Permission {Name = "p-User"}}
                }
            };
            users[1].Roles = new List<Role>
            {
                new Role {Name = "Guest", Permissions = new List<Permission> {new Permission {Name = "p-Guest"}}}
            };

            var query = users.AsQueryable();

            // Assign
            var queryNormal = query.SelectMany(u => u.Roles.SelectMany(r => r.Permissions)).ToList();
            var queryDynamic = query.SelectMany<Permission>("Roles.SelectMany(Permissions)").ToDynamicList();

            // Assert
            Assert.Equal(queryNormal, queryDynamic);
        }

        [Fact]
        public void SelectMany_Dynamic_IntoType()
        {
            // Act
            var users = User.GenerateSampleModels(2);
            users[0].Roles = new List<Role>
            {
                new Role { Name = "Admin", Permissions = new List<Permission> {new Permission {Name = "p-Admin"}, new Permission {Name = "p-User"}}
                }
            };
            users[1].Roles = new List<Role>
            {
                new Role {Name = "Guest", Permissions = new List<Permission> {new Permission {Name = "p-Guest"}}}
            };

            var query = users.AsQueryable();

            // Assign
            var queryNormal = query.SelectMany(u => u.Roles.SelectMany(r => r.Permissions)).ToList();
            var queryDynamic = query.SelectMany(typeof(Permission), "Roles.SelectMany(Permissions)").ToDynamicList();

            // Assert
            Assert.Equal(queryNormal, queryDynamic);
        }

        [Fact]
        public void SelectMany_Dynamic_OverJArray_TResult()
        {
            // Arrange
            var array1 = JArray.Parse("[1,2,3]");
            var array2 = JArray.Parse("[4,5,6]");

            // Act
            var expectedResult = new[] { array1, array2 }.SelectMany(it => it).ToArray();
            var result = new[] { array1, array2 }.AsQueryable().SelectMany("it").ToDynamicArray<JToken>();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            // result.Should().HaveCount(6).And.Subject.Select(j => j.Value).Should().ContainInOrder(new[] { 1, 2, 3, 4, 5, 6 });
        }

        [Fact]
        public void SelectMany_Dynamic_OverArray_TResult()
        {
            var testList = new[]
            {
                new[] {new Permission {Name = "p-Admin"}},
                new[] {new Permission {Name = "p-Admin"}, new Permission {Name = "p-User"}},
                new[] {new Permission {Name = "p-x"}, new Permission {Name = "p-y"}}
            };

            var expectedResult = testList.SelectMany(it => it).ToList();
            var result = testList.AsQueryable().SelectMany<Permission>("it").ToList();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void SelectMany_Dynamic_OverArray_Int()
        {
            var testList = new[]
            {
                new[] { 1, 2, 3 },
                new[] { 4, 5, 6 }
            };

            var expectedResult = testList.SelectMany(it => it).ToList();
            var result = testList.AsQueryable().SelectMany<int>("it").ToList();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void SelectMany_Dynamic_OverArray_IntoType()
        {
            var testList = new[]
            {
                new[] {new Permission {Name = "p-Admin"}},
                new[] {new Permission {Name = "p-Admin"}, new Permission {Name = "p-User"}},
                new[] {new Permission {Name = "p-x"}, new Permission {Name = "p-y"}}
            };

            var expectedResult = testList.SelectMany(it => it).ToList();
            var result = testList.AsQueryable().SelectMany(typeof(Permission), "it").ToDynamicList<Permission>();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void SelectMany_Dynamic_WithResultProjection()
        {
            //Arrange
            List<int> rangeOfInt = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<double> rangeOfDouble = new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 };
            List<KeyValuePair<int, double>> range = rangeOfInt.SelectMany(e => rangeOfDouble, (x, y) => new KeyValuePair<int, double>(x, y)).ToList();

            //Act
            IEnumerable rangeResult = rangeOfInt.AsQueryable()
                .SelectMany("@0", "new(x as _A, y as _B)", new object[] { rangeOfDouble })
                .Select("it._A * it._B");

            //Assert
            Assert.Equal(range.Select(t => t.Key * t.Value).ToArray(), rangeResult.Cast<double>().ToArray());
        }

        [Fact]
        public void SelectMany_Dynamic_WithResultProjection_CustomParameterNames()
        {
            //Arrange
            List<int> rangeOfInt = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            List<double> rangeOfDouble = new List<double> { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 };
            List<KeyValuePair<int, double>> range = rangeOfInt.SelectMany(e => rangeOfDouble, (x, y) => new KeyValuePair<int, double>(x, y)).ToList();

            //Act
            IEnumerable rangeResult = rangeOfInt.AsQueryable()
                .SelectMany("@0", "new(VeryNiceName as _A, OtherName as _X)", "VeryNiceName", "OtherName", new object[] { rangeOfDouble })
                .Select("it._A * it._X");

            //Assert
            Assert.Equal(range.Select(t => t.Key * t.Value).ToArray(), rangeResult.Cast<double>().ToArray());
        }
    }
}