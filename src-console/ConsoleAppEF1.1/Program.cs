using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json;

namespace ConsoleAppEF1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new MyDbContext())
            {
                var persons = new List<Person>
                {
                    new Person { Name = "a", Age = 18, Address = new Address { Street = "s1" } },
                    new Person { Name = "b", Age = 19, Address = new Address { Street = "s2" } },
                    new Person { Name = "c", Age = 20, Address = new Address { Street = "s3" } },
                };

                // Database
                bool deleted = db.Database.EnsureDeleted();
                Console.WriteLine("Database is deleted = {0}", deleted);
                if (db.Database.EnsureCreated())
                {
                    Console.WriteLine("Database is created");
                    persons.ForEach(p => db.Add(p));
                    db.SaveChanges();
                }

                Console.WriteLine(new string('-', 80));
                var result1 = db.Persons.Where("Age > 17").OrderBy("Name desc");
                foreach (var person in result1)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(person, Formatting.Indented));
                }

                Console.WriteLine(new string('-', 80));
                var result2 = db.Persons.Where("Age > 17").OrderBy("Address.Street");
                foreach (var person in result2)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(person, Formatting.Indented));
                }

                int x = 0;
            }
        }
    }
}
