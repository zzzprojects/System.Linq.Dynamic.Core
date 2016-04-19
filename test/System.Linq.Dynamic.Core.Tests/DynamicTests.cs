using System.Collections;
using System.Collections.Generic;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public class DynamicTests
    {
        [Fact]
        public void Where()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();


            //Act
            var userById = qry.Where("Id=@0", testList[10].Id);
            var userByUserName = qry.Where("UserName=\"User5\"");
            var nullProfileCount = qry.Where("Profile=null");
            var userByFirstName = qry.Where("Profile!=null && Profile.FirstName=@0", testList[1].Profile.FirstName);


            //Assert
            Assert.Equal(testList[10], userById.Single());
            Assert.Equal(testList[5], userByUserName.Single());
            Assert.Equal(testList.Count(x => x.Profile == null), nullProfileCount.Count());
            Assert.Equal(testList[1], userByFirstName.Single());
        }

        [Fact]
        public void Where_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            Assert.Throws<ParseException>(() => qry.Where("Id"));
            Assert.Throws<ParseException>(() => qry.Where("Bad=3"));
            Assert.Throws<ParseException>(() => qry.Where("Id=123"));

            Assert.Throws<ArgumentNullException>(() => DynamicQueryable.Where(null, "Id=1"));
            Assert.Throws<ArgumentNullException>(() => qry.Where(null));
            Assert.Throws<ArgumentException>(() => qry.Where(""));
            Assert.Throws<ArgumentException>(() => qry.Where(" "));
        }

        [Fact]
        public void OrderBy()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();


            //Act
            var orderById = qry.OrderBy("Id");
            var orderByIdDesc = qry.OrderBy("Id DESC");
            var orderByAge = qry.OrderBy("Profile.Age");
            var orderByAgeDesc = qry.OrderBy("Profile.Age DESC");
            var orderByComplex = qry.OrderBy("Profile.Age, Id");
            var orderByComplex2 = qry.OrderBy("Profile.Age DESC, Id");


            //Assert
            Assert.Equal(testList.OrderBy(x => x.Id).ToArray(), orderById.ToArray());
            Assert.Equal(testList.OrderByDescending(x => x.Id).ToArray(), orderByIdDesc.ToArray());

            Assert.Equal(testList.OrderBy(x => x.Profile.Age).ToArray(), orderByAge.ToArray());
            Assert.Equal(testList.OrderByDescending(x => x.Profile.Age).ToArray(), orderByAgeDesc.ToArray());

            Assert.Equal(testList.OrderBy(x => x.Profile.Age).ThenBy(x => x.Id).ToArray(), orderByComplex.ToArray());
            Assert.Equal(testList.OrderByDescending(x => x.Profile.Age).ThenBy(x => x.Id).ToArray(), orderByComplex2.ToArray());
        }

        [Fact]
        public void OrderBy_AsStringExpression()
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
        public void OrderBy_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            Assert.Throws<ParseException>(() => qry.OrderBy("Bad=3"));
            Assert.Throws<ParseException>(() => qry.Where("Id=123"));

            Assert.Throws<ArgumentNullException>(() => DynamicQueryable.OrderBy(null, "Id"));
            Assert.Throws<ArgumentNullException>(() => qry.OrderBy(null));
            Assert.Throws<ArgumentException>(() => qry.OrderBy(""));
            Assert.Throws<ArgumentException>(() => qry.OrderBy(" "));
        }

        [Fact]
        public void SelectMany()
        {
            // Act
            var users = User.GenerateSampleModels(2);
            users[0].Roles = new List<Role> { new Role { Name = "Admin", Permissions = new List<Permission> { new Permission { Name = "p-Admin" }, new Permission { Name = "p-User" } } } };
            users[1].Roles = new List<Role> { new Role { Name = "Guest", Permissions = new List<Permission> { new Permission { Name = "p-Guest" } } } };

            var query = users.AsQueryable();

            // Assign
            var queryNormal = query.SelectMany(u => u.Roles.SelectMany(r => r.Permissions)).Select(p => p.Name).ToList();

            var queryDynamic = query.SelectMany("Roles.SelectMany(Permissions)").Select("Name").ToDynamicList<string>();

            // Assert
            Xunit.Assert.Equal(queryNormal, queryDynamic);
        }

        [Fact]
        public void Select_String()
        {
            //Arrange
            var testList = new List<Entities.Employee>();
            var qry = testList.AsQueryable();

            //Act
            string includesX = ", it.Company as TEntity__Company, it.Company.MainCompany as TEntity__Company_MainCompany, it.Country as TEntity__Country, it.Function as TEntity__Function, it.SubFunction as TEntity__SubFunction";
            string select = $"new (\"__Key__\" as __Key__, it AS TEntity__{includesX})";

            var userNames = qry.Select(select).ToDynamicList();
            Assert.NotNull(userNames);
        }

        [Fact]
        public void Select()
        {
            //Arrange
            List<int> range = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            IEnumerable rangeResult = range.AsQueryable().Select("it * it");
            var userNames = qry.Select("UserName");
            var userFirstName = qry.Select("new (UserName, Profile.FirstName as MyFirstName)");
            var userRoles = qry.Select("new (UserName, Roles.Select(Id) AS RoleIds)");

            //Assert
            Assert.Equal(range.Select(x => x * x).ToArray(), rangeResult.Cast<int>().ToArray());

#if NET35 || DNXCORE50
            Assert.Equal(testList.Select(x => x.UserName).ToArray(), userNames.AsEnumerable().Cast<string>().ToArray());
            Assert.Equal(
                testList.Select(x => "{ UserName = " + x.UserName + ", MyFirstName = " + x.Profile.FirstName + " }").ToArray(),
                userFirstName.Cast<object>().Select(x => x.ToString()).ToArray());
            Assert.Equal(testList[0].Roles.Select(x => x.Id).ToArray(), Enumerable.ToArray(userRoles.First().GetDynamicProperty<IEnumerable<Guid>>("RoleIds")));
#else
            Assert.Equal(testList.Select(x => x.UserName).ToArray(), userNames.Cast<string>().ToArray());
            Assert.Equal(
                testList.Select(x => "{ UserName = " + x.UserName + ", MyFirstName = " + x.Profile.FirstName + " }").ToArray(),
                userFirstName.AsEnumerable().Select(x => x.ToString()).Cast<string>().ToArray());
            Assert.Equal(testList[0].Roles.Select(x => x.Id).ToArray(), Enumerable.ToArray(userRoles.First().RoleIds));
#endif
        }

        [Fact]
        public void Select_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            Assert.Throws<ParseException>(() => qry.Select("Bad"));
            Assert.Throws<ParseException>(() => qry.Select("Id, UserName"));
            Assert.Throws<ParseException>(() => qry.Select("new Id, UserName"));
            Assert.Throws<ParseException>(() => qry.Select("new (Id, UserName"));
            Assert.Throws<ParseException>(() => qry.Select("new (Id, UserName, Bad)"));

            Assert.Throws<ArgumentNullException>(() => DynamicQueryable.Select(null, "Id"));
            Assert.Throws<ArgumentNullException>(() => qry.Select(null));
            Assert.Throws<ArgumentException>(() => qry.Select(""));
            Assert.Throws<ArgumentException>(() => qry.Select(" "));
        }

        [Fact]
        public void GroupBy()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var byAgeReturnUserName = qry.GroupBy("Profile.Age", "UserName");
            var byAgeReturnAll = qry.GroupBy("Profile.Age");

            //Assert
            Assert.Equal(testList.GroupBy(x => x.Profile.Age).Count(), byAgeReturnUserName.Count());
            Assert.Equal(testList.GroupBy(x => x.Profile.Age).Count(), byAgeReturnAll.Count());
        }

        [Fact]
        public void GroupBy_Exceptions()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100, allowNullableProfiles: true);
            var qry = testList.AsQueryable();

            //Act
            Assert.Throws<ParseException>(() => qry.GroupBy("Bad"));
            Assert.Throws<ParseException>(() => qry.GroupBy("Id, UserName"));
            Assert.Throws<ParseException>(() => qry.GroupBy("new Id, UserName"));
            Assert.Throws<ParseException>(() => qry.GroupBy("new (Id, UserName"));
            Assert.Throws<ParseException>(() => qry.GroupBy("new (Id, UserName, Bad)"));

            Assert.Throws<ArgumentNullException>(() => DynamicQueryable.GroupBy((IQueryable<string>)null, "Id"));
            Assert.Throws<ArgumentNullException>(() => qry.GroupBy(null));
            Assert.Throws<ArgumentException>(() => qry.GroupBy(""));
            Assert.Throws<ArgumentException>(() => qry.GroupBy(" "));

            Assert.Throws<ArgumentNullException>(() => qry.GroupBy("Id", (string)null));
            Assert.Throws<ArgumentException>(() => qry.GroupBy("Id", ""));
            Assert.Throws<ArgumentException>(() => qry.GroupBy("Id", " "));
        }

        [Fact]
        public void GroupByMany_StringExpressions()
        {
            var lst = new List<Tuple<int, int, int>>
            {
                new Tuple<int, int, int>(1, 1, 1),
                new Tuple<int, int, int>(1, 1, 2),
                new Tuple<int, int, int>(1, 1, 3),
                new Tuple<int, int, int>(2, 2, 4),
                new Tuple<int, int, int>(2, 2, 5),
                new Tuple<int, int, int>(2, 2, 6),
                new Tuple<int, int, int>(2, 3, 7)
            };

            var sel = lst.AsQueryable().GroupByMany("Item1", "Item2");

            Assert.Equal(sel.Count(), 2);
            Assert.Equal(sel.First().Subgroups.Count(), 1);
            Assert.Equal(sel.Skip(1).First().Subgroups.Count(), 2);
        }

        [Fact]
        public void GroupByMany_LambdaExpressions()
        {
            var lst = new List<Tuple<int, int, int>>
            {
                new Tuple<int, int, int>(1, 1, 1),
                new Tuple<int, int, int>(1, 1, 2),
                new Tuple<int, int, int>(1, 1, 3),
                new Tuple<int, int, int>(2, 2, 4),
                new Tuple<int, int, int>(2, 2, 5),
                new Tuple<int, int, int>(2, 2, 6),
                new Tuple<int, int, int>(2, 3, 7)
            };

            var sel = lst.AsQueryable().GroupByMany(x => x.Item1, x => x.Item2);

            Assert.Equal(sel.Count(), 2);
            Assert.Equal(sel.First().Subgroups.Count(), 1);
            Assert.Equal(sel.Skip(1).First().Subgroups.Count(), 2);
        }

        class Person
        {
            public string Name { get; set; }
        }

        class Pet
        {
            public string Name { get; set; }
            public Person Owner { get; set; }
        }

        [Fact]
        public void Join()
        {
            //Arrange
            Person magnus = new Person { Name = "Hedlund, Magnus" };
            Person terry = new Person { Name = "Adams, Terry" };
            Person charlotte = new Person { Name = "Weiss, Charlotte" };

            Pet barley = new Pet { Name = "Barley", Owner = terry };
            Pet boots = new Pet { Name = "Boots", Owner = terry };
            Pet whiskers = new Pet { Name = "Whiskers", Owner = charlotte };
            Pet daisy = new Pet { Name = "Daisy", Owner = magnus };

            List<Person> people = new List<Person> { magnus, terry, charlotte };
            List<Pet> pets = new List<Pet> { barley, boots, whiskers, daisy };


            //Act
            var realQuery = people.AsQueryable().Join(
                pets,
                person => person,
                pet => pet.Owner,
                (person, pet) =>
                new { OwnerName = person.Name, Pet = pet.Name });

            var dynamicQuery = people.AsQueryable().Join(
                pets,
                "it",
                "Owner",
                "new(outer.Name as OwnerName, inner.Name as Pet)");

            //Assert
            var realResult = realQuery.ToArray();

#if DNXCORE50
            var dynamicResult = dynamicQuery.ToDynamicArray<DynamicClass>();

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, dynamicResult[i].GetDynamicProperty<string>("OwnerName"));
                Assert.Equal(realResult[i].Pet, dynamicResult[i].GetDynamicProperty<string>("Pet"));
            }
#else
            var dynamicResult = dynamicQuery.ToDynamicArray();

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, dynamicResult[i].OwnerName);
                Assert.Equal(realResult[i].Pet, dynamicResult[i].Pet);
            }
#endif
        }

    }
}