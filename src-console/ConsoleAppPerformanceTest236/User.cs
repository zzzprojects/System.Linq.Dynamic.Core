using System;
using System.Collections.Generic;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace ConsoleAppPerformanceTest236
{
    public class User
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string City { get; set; }

        public int? NullableInt { get; set; }

        public int Income { get; set; }

        public static IList<User> GenerateSampleModels(int total, bool allowNullableProfiles = false)
        {
            var list = new List<User>();

            var randomizerFullName = RandomizerFactory.GetRandomizer(new FieldOptionsFullName());
            var randomizerCity= RandomizerFactory.GetRandomizer(new FieldOptionsCity());

            for (int i = 0; i < total; i++)
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = randomizerFullName.Generate(),
                    City = randomizerCity.Generate(),
                    Income = 1 + i % 15 * 100
                };

                list.Add(user);
            }

            return list.ToArray();
        }
    }
}
