using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace MemoryLeakTest
{
    public class Test
    {
        public void Join()
        {
            //Arrange
            Person magnus = new Person { Id = Guid.NewGuid(), Name = "Hedlund, Magnus", From = 100, Till = 1000 };
            Person terry = new Person { Id = Guid.NewGuid(), Name = "Adams, Terry", From = 10230, Till = 100440 };
            Person charlotte = new Person { Id = Guid.NewGuid(), Name = "Weiss, Charlotte", From = 10, Till = 154 };

            Pet barley = new Pet { Id = Guid.NewGuid(), Name = "Barley", Owner = terry, From = 67, Till = 231 };
            Pet boots = new Pet { Id = Guid.NewGuid(), Name = "Boots", Owner = terry, From = 120, Till = 220 };
            Pet whiskers = new Pet { Id = Guid.NewGuid(), Name = "Whiskers", Owner = charlotte, From = 3310, Till = 9010 };
            Pet daisy = new Pet { Id = Guid.NewGuid(), Name = "Daisy", Owner = magnus, From = 27, Till = 1200 };

            Toy ball1 = new Toy { Id = Guid.NewGuid(), From = 10, Till = 200, Pet = barley };
            Toy ball2 = new Toy { Id = Guid.NewGuid(), From = 89, Pet = barley, Till = 348 };
            Toy duck1 = new Toy { Id = Guid.NewGuid(), From = 10, Till = 200, Pet = boots };
            Toy duck2 = new Toy { Id = Guid.NewGuid(), From = 89, Pet = whiskers, Till = 348 };
            Toy toy1 = new Toy { Id = Guid.NewGuid(), From = 10, Till = 200, Pet = boots };
            Toy toy2 = new Toy { Id = Guid.NewGuid(), From = 89, Pet = daisy, Till = 348 };
            Toy something1 = new Toy { Id = Guid.NewGuid(), From = 10, Till = 200, Pet = daisy };
            Toy something2 = new Toy { Id = Guid.NewGuid(), From = 89, Pet = barley, Till = 348 };
            Toy toy3 = new Toy { Id = Guid.NewGuid(), From = 10, Till = 200, Pet = daisy };
            Toy toy4 = new Toy { Id = Guid.NewGuid(), From = 89, Pet = whiskers, Till = 348 };


            IList people = new List<Person> { magnus, terry, charlotte };
            IList pets = new List<Pet> { barley, boots, whiskers, daisy };
            IList toys = new List<Toy> { ball1, ball2, duck1, duck2, toy1, toy2, something1, something2, toy3, toy4 };

            //Act
            var sw = new Stopwatch();
            long t = 0;
            var rnd = new Random();
            for (var i = 0; i < 5000; i++)
            {
                sw.Start();
                Act(people, pets, toys, rnd);
                sw.Stop();
                t = sw.ElapsedMilliseconds;
                if (i % 10 == 0)
                {
                    Console.WriteLine("{0} - {1}", i, t);
                }
                sw.Reset();
            }
        }

        private dynamic[] Act(IList people, IList pets, IList toys, Random rnd)
        {
            var peopleQ = people.AsQueryable().Where(string.Format("From<{0} AND Till>{1}", rnd.Next(0, 500), rnd.Next(500, 20000)));
            var petsQ = pets.AsQueryable().Where(string.Format("From<{0} AND Till>{1}", rnd.Next(0, 500), rnd.Next(500, 20000)));
            var toyQ = toys.AsQueryable().Where(string.Format("From<{0} AND Till>{1}", rnd.Next(0, 500), rnd.Next(500, 20000)));

            var q1 = petsQ.Join(toyQ, "new (Id as firstKey)", "new (PetId as firstKey)", "new (inner as toy, outer as pet)");
            var q2 = peopleQ.Join(q1, "new (Id as firstKey)", "new (pet.OwnerId as firstKey)", "new (inner as pet, outer as person)")
                        .Select("new (person.Name as personName, pet.pet.Name as petName, pet.toy.Id as toyUid)");

            var result = q2.ToDynamicArray();
            return result;
        }
    }
}