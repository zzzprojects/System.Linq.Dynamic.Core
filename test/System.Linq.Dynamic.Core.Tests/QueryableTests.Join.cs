using System.Collections.Generic;
using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Dynamic.Core.Tests.Helpers.Models;
using FluentAssertions;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests
{
    public partial class QueryableTests
    {
        [Fact]
        public void Join_InheritedClasses()
        {
            // Arrange
            var magnus = new Person { Name = "Hedlund, Magnus" };
            var terry = new Person { Name = "Adams, Terry" };
            var charlotte = new Person { Name = "Weiss, Charlotte" };

            var barley = new Pet { Name = "Barley", Owner = terry };
            var boots = new Pet { Name = "Boots", Owner = terry };
            var whiskers = new Pet { Name = "Whiskers", Owner = charlotte };
            var daisy = new SpecialPet { Name = "Daisy", Owner = magnus, IsSpecial = true };

            var people = new List<Person> { magnus, terry, charlotte };
            var pets = new List<Pet> { barley, boots, whiskers, daisy };

            // Act 1
            var realQuery = people.AsQueryable()
                .Join(
                    pets,
                    person => person,
                    pet => pet.Owner,
                    (person, pet) => new { OwnerName = person.Name, Pet = pet.Name }
                );
            var realResult = realQuery.ToList();

            // Act 2
            var dynamicQuery = people.AsQueryable()
                .Join(
                    pets,
                    "it",
                    "Owner",
                    "new(outer.Name as OwnerName, inner.Name as Pet)"
                );
            var dynamicResult = dynamicQuery.ToDynamicList();

            // Assert
            realResult.Should().BeEquivalentTo(dynamicResult);
        }

        [Fact]
        public void Join()
        {
            // Arrange
            var magnus = new Person { Name = "Hedlund, Magnus" };
            var terry = new Person { Name = "Adams, Terry" };
            var charlotte = new Person { Name = "Weiss, Charlotte" };

            var barley = new Pet { Name = "Barley", Owner = terry };
            var boots = new Pet { Name = "Boots", Owner = terry };
            var whiskers = new Pet { Name = "Whiskers", Owner = charlotte };
            var daisy = new Pet { Name = "Daisy", Owner = magnus };

            var people = new List<Person> { magnus, terry, charlotte };
            var pets = new List<Pet> { barley, boots, whiskers, daisy };

            // Act 1
            var realQuery = people.AsQueryable()
                .Join(
                    pets,
                    person => person,
                    pet => pet.Owner,
                    (person, pet) => new { OwnerName = person.Name, Pet = pet.Name }
                );
            var realResult = realQuery.ToList();

            // Act 2
            var dynamicQuery = people.AsQueryable()
                .Join(
                    pets,
                    "it",
                    "Owner",
                    "new(outer.Name as OwnerName, inner.Name as Pet)");
            var dynamicResult = dynamicQuery.ToDynamicList();

            // Assert
            realResult.Should().BeEquivalentTo(dynamicResult);
        }

        [Fact]
        public void JoinOnNullableType_RightNullable()
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
            var pets = new List<Pet> { barley, boots, whiskers, daisy };

            //Act
            var realQuery = people.AsQueryable()
                .Join(
                    pets,
                    person => person.Id,
                    pet => pet.NullableOwnerId,
                    (person, pet) => new { OwnerName = person.Name, Pet = pet.Name });

            var dynamicQuery = people.AsQueryable()
                .Join(
                    pets,
                    "it.Id",
                    "NullableOwnerId",
                    "new(outer.Name as OwnerName, inner.Name as Pet)");

            //Assert
            var realResult = realQuery.ToArray();
            var dynamicResult = dynamicQuery.ToDynamicArray<DynamicClass>();

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                Assert.Equal(realResult[i].Pet, dynamicResult[i].GetDynamicPropertyValue<string>("Pet"));
            }
        }

        [Fact]
        public void JoinOnNullableType_LeftNullable()
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
            var pets = new List<Pet> { barley, boots, whiskers, daisy };

            //Act
            var realQuery = people.AsQueryable()
                .Join(
                    pets,
                    person => person.NullableId,
                    pet => pet.OwnerId,
                    (person, pet) => new { OwnerName = person.Name, Pet = pet.Name });

            var dynamicQuery = people.AsQueryable()
                .Join(
                    pets,
                    "it.NullableId",
                    "OwnerId",
                    "new(outer.Name as OwnerName, inner.Name as Pet)");

            //Assert
            var realResult = realQuery.ToArray();
            var dynamicResult = dynamicQuery.ToDynamicArray<DynamicClass>();

            Assert.Equal(realResult.Length, dynamicResult.Length);
            for (int i = 0; i < realResult.Length; i++)
            {
                Assert.Equal(realResult[i].OwnerName, dynamicResult[i].GetDynamicPropertyValue<string>("OwnerName"));
                Assert.Equal(realResult[i].Pet, dynamicResult[i].GetDynamicPropertyValue<string>("Pet"));
            }
        }

        [Fact]
        public void JoinOnNullableType_NotSameTypesThrowsException()
        {
            var person = new Person { Id = 1, Name = "Hedlund, Magnus" };
            var people = new List<Person> { person };
            var pets = new List<Pet> { new Pet { Name = "Daisy", OwnerId = person.Id } };

            Check.ThatCode(() =>
                people.AsQueryable()
                    .Join(
                        pets,
                        "it.Id",
                        "Name", // This is wrong
                        "new(outer.Name as OwnerName, inner.Name as Pet)")).Throws<ParseException>();
        }
    }
}