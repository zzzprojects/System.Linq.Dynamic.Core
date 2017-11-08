using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;

namespace NullableDemo
{
    public class Test
    {
        public void Join()
        {
            // Arrange
            Person magnus = new Person { Id = Guid.NewGuid(), Name = "Hedlund, Magnus", From = 100, Till = 1000 };
            Pet daisy = new Pet { Id = Guid.NewGuid(), Name = "Daisy", Owner = magnus, From = 27, Till = 1200 };
            IList<Person> people = new List<Person> { magnus };
            IList<Pet> pets = new List<Pet> { daisy };

            // Act
            Act(people, pets);
        }

        private dynamic[] Act(IList<Person> peopleList, IList<Pet> petsList)
        {
            var peopleQuery = peopleList.AsQueryable().Where(p => p.From < 111 && p.Till > 444);
            var petsQuery = petsList.AsQueryable().Where(p => p.From < 66 && p.Till > 888);

            var query = peopleQuery.Join(petsQuery, people => new { firstKey = (Guid?) people.Id }, pet => new { firstKey = pet.OwnerId }, (inner, outer) => new { pet = inner, person = outer })
                .Select(res => new { personName = res.person.Name, petName = res.pet.Name });

            var queryArray = query.ToArray();
            foreach (var x in queryArray)
            {
                Console.WriteLine(JsonConvert.SerializeObject(x));
            }
            Console.WriteLine(new string('-', 80));

            var dynamicPeopleQuery1 = peopleList.AsQueryable().Where($"From < {111} AND Till > {444}");
            var dynamicPetsQuery1 = petsList.AsQueryable().Where($"From < {66} AND Till > {888}");

            var dynamicQuery1 = dynamicPeopleQuery1.Join(dynamicPetsQuery1, "new (Id as firstKey)", "new (Id as firstKey)", "new (inner as pet, outer as person)")
                .Select("new (person.Name as personName, pet.Name as petName)");

            var dynamicQueryArray1 = dynamicQuery1.ToDynamicArray();
            foreach (var x in dynamicQueryArray1)
            {
                Console.WriteLine(JsonConvert.SerializeObject(x));
            }
            Console.WriteLine(new string('-', 80));
            
            var dynamicPeopleQuery2 = peopleList.AsQueryable().Where($"From < {111} AND Till > {444}");
            var dynamicPetsQuery2 = petsList.AsQueryable().Where($"From < {66} AND Till > {888}");

            var dynamicQuery2 = dynamicPeopleQuery2.Join(dynamicPetsQuery2, "new (Guid?(Id) as firstKey)", "new (OwnerId as firstKey)", "new (inner as pet, outer as person)")
                .Select("new (person.Name as personName, pet.Name as petName)");

            var dynamicQueryArray2 = dynamicQuery2.ToDynamicArray();
            foreach (var x in dynamicQueryArray2)
            {
                Console.WriteLine(JsonConvert.SerializeObject(x));
            }
            Console.WriteLine(new string('-', 80));

            return dynamicQueryArray2;
        }
    }
}
