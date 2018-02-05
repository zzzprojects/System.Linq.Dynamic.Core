using System.Collections.Generic;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void GroupJoin_1()
        {
            //Arrange
            Person magnus = new Person { Name = "Hedlund, Magnus" };
            Person terry = new Person { Name = "Adams, Terry" };
            Person charlotte = new Person { Name = "Weiss, Charlotte" };

            Pet barley = new Pet { Name = "Barley", Owner = terry };
            Pet boots = new Pet { Name = "Boots", Owner = terry };
            Pet whiskers = new Pet { Name = "Whiskers", Owner = charlotte };
            Pet daisy = new Pet { Name = "Daisy", Owner = magnus };

            var people = new List<Person> { magnus, terry, charlotte };
            var petsList = new List<Pet> { barley, boots, whiskers, daisy };

            //Act
            var realQuery = people.AsQueryable().GroupJoin(
                petsList,
                person => person,
                pet => pet.Owner,
                (person, pets) => new { OwnerName = person.Name, Pets = pets, NumberOfPets = pets.Count() });

            var dynamicQuery = people.AsQueryable().GroupJoin(
                petsList,
                "it",
                "Owner",
                "new(outer.Name as OwnerName, inner as Pets, inner.Count() as NumberOfPets)");

            //Assert
            var realResult = realQuery.ToArray();

#if NETSTANDARD
            var dynamicResult = dynamicQuery.ToDynamicArray<DynamicClass>();

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                Assert.Equal(realResult[i].NumberOfPets, dynamicResult[i].GetDynamicPropertyValue<int>("NumberOfPets"));
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.Equal(realResult[i].Pets.ElementAt(j).Name, dynamicResult[i].GetDynamicPropertyValue<IEnumerable<Pet>>("Pets").ElementAt(j).Name);
                }
            }
#else
            var dynamicResult = dynamicQuery.ToDynamicArray();

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, ((dynamic) dynamicResult[i]).OwnerName);
                Assert.Equal(realResult[i].NumberOfPets, ((dynamic) dynamicResult[i]).NumberOfPets);
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.Equal(realResult[i].Pets.ElementAt(j).Name, (((IEnumerable<Pet>)((dynamic)dynamicResult[i]).Pets)).ElementAt(j).Name);
                }
            }
#endif
        }

        [Fact]
        public void GroupJoin_2()
        {
            //Arrange
            Person magnus = new Person { Name = "Hedlund, Magnus" };
            Person terry = new Person { Name = "Adams, Terry" };
            Person charlotte = new Person { Name = "Weiss, Charlotte" };

            Pet barley = new Pet { Name = "Barley", Owner = terry };
            Pet boots = new Pet { Name = "Boots", Owner = terry };
            Pet whiskers = new Pet { Name = "Whiskers", Owner = charlotte };
            Pet daisy = new Pet { Name = "Daisy", Owner = magnus };

            var people = new List<Person> { magnus, terry, charlotte };
            var petsList = new List<Pet> { barley, boots, whiskers, daisy };

            //Act
            var realQuery = people.AsQueryable().GroupJoin(
                petsList,
                person => person.Id,
                pet => pet.OwnerId,
                (person, pets) => new { OwnerName = person.Name, Pets = pets, NumberOfPets = pets.Count() });

            var dynamicQuery = people.AsQueryable().GroupJoin(
                petsList,
                "it.Id",
                "OwnerId",
                "new(outer.Name as OwnerName, inner as Pets, inner.Count() as NumberOfPets)");

            //Assert
            var realResult = realQuery.ToArray();

#if NETSTANDARD
            var dynamicResult = dynamicQuery.ToDynamicArray<DynamicClass>();

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                Assert.Equal(realResult[i].NumberOfPets, dynamicResult[i].GetDynamicPropertyValue<int>("NumberOfPets"));
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.Equal(realResult[i].Pets.ElementAt(j).Name, dynamicResult[i].GetDynamicPropertyValue<IEnumerable<Pet>>("Pets").ElementAt(j).Name);
                }
            }
#else
            var dynamicResult = dynamicQuery.ToDynamicArray();

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, ((dynamic) dynamicResult[i]).OwnerName);
                Assert.Equal(realResult[i].NumberOfPets, ((dynamic) dynamicResult[i]).NumberOfPets);
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.Equal(realResult[i].Pets.ElementAt(j).Name, (((IEnumerable<Pet>)((dynamic)dynamicResult[i]).Pets)).ElementAt(j).Name);
                }
            }
#endif
        }

        [Fact]
        public void GroupJoinOnNullableType_RightNullable()
        {
            //Arrange
            Person magnus = new Person { Id = 1, Name = "Hedlund, Magnus" };
            Person terry = new Person { Id = 2, Name = "Adams, Terry" };
            Person charlotte = new Person { Id = 3, Name = "Weiss, Charlotte" };

            Pet barley = new Pet { Name = "Barley", NullableOwnerId = terry.Id };
            Pet boots = new Pet { Name = "Boots", NullableOwnerId = terry.Id };
            Pet whiskers = new Pet { Name = "Whiskers", NullableOwnerId = charlotte.Id };
            Pet daisy = new Pet { Name = "Daisy", NullableOwnerId = magnus.Id };

            var people = new List<Person> { magnus, terry, charlotte };
            var petsList = new List<Pet> { barley, boots, whiskers, daisy };

            //Act
            var realQuery = people.AsQueryable().GroupJoin(
                petsList,
                person => person.Id,
                pet => pet.NullableOwnerId,
                (person, pets) => new { OwnerName = person.Name, Pets = pets });

            var dynamicQuery = people.AsQueryable().GroupJoin(
                petsList,
                "it.Id",
                "NullableOwnerId",
                "new(outer.Name as OwnerName, inner as Pets)");

            //Assert
            var realResult = realQuery.ToArray();
            var dynamicResult = dynamicQuery.ToDynamicArray<DynamicClass>();

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.Equal(realResult[i].Pets.ElementAt(j).Name, dynamicResult[i].GetDynamicPropertyValue<IEnumerable<Pet>>("Pets").ElementAt(j).Name);
                }
            }
        }

        [Fact]
        public void GroupJoinOnNullableType_LeftNullable()
        {
            //Arrange
            Person magnus = new Person { NullableId = 1, Name = "Hedlund, Magnus" };
            Person terry = new Person { NullableId = 2, Name = "Adams, Terry" };
            Person charlotte = new Person { NullableId = 3, Name = "Weiss, Charlotte" };

            Pet barley = new Pet { Name = "Barley", OwnerId = terry.Id };
            Pet boots = new Pet { Name = "Boots", OwnerId = terry.Id };
            Pet whiskers = new Pet { Name = "Whiskers", OwnerId = charlotte.Id };
            Pet daisy = new Pet { Name = "Daisy", OwnerId = magnus.Id };

            var people = new List<Person> { magnus, terry, charlotte };
            var petsList = new List<Pet> { barley, boots, whiskers, daisy };

            //Act
            var realQuery = people.AsQueryable().GroupJoin(
                petsList,
                person => person.NullableId,
                pet => pet.OwnerId,
                (person, pets) => new { OwnerName = person.Name, Pets = pets });

            var dynamicQuery = people.AsQueryable().GroupJoin(
                petsList,
                "it.NullableId",
                "OwnerId",
                "new(outer.Name as OwnerName, inner as Pets)");

            //Assert
            var realResult = realQuery.ToArray();
            var dynamicResult = dynamicQuery.ToDynamicArray<DynamicClass>();

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                for (int j = 0; j < realResult[i].Pets.Count(); j++)
                {
                    Assert.Equal(realResult[i].Pets.ElementAt(j).Name, dynamicResult[i].GetDynamicPropertyValue<IEnumerable<Pet>>("Pets").ElementAt(j).Name);
                }
            }
        }

        [Fact]
        public void GroupJoinOnNullableType_NotSameTypesThrowsException()
        {
            var person = new Person { Id = 1, Name = "Hedlund, Magnus" };
            var people = new List<Person> { person };
            var pets = new List<Pet> { new Pet { Name = "Daisy", OwnerId = person.Id } };

            Check.ThatCode(() =>
                people.AsQueryable()
                    .GroupJoin(
                        pets,
                        "it.Id",
                        "Name", // This is wrong
                        "new(outer.Name as OwnerName, inner as Pets)")).Throws<ParseException>();
        }
    }
}
