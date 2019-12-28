using System;
using System.Linq;
using ConsoleAppEF2.Database;

namespace ConsoleAppEF31
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new TestContext();

            // context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var dateDeleted = new DateTime(2019, 2, 2);

            var dateLastModified = new DateTime(2018, 1, 15);
            if (!context.Cars.Any())
            {
                context.Cars.Add(new Car { Brand = "Ford", Color = "Blue", Vin = "yes", Year = "2017", DateLastModified = dateLastModified, DateDeleted = dateDeleted });
                context.Cars.Add(new Car { Brand = "Fiat", Color = "Red", Vin = "yes", Year = "2016", DateLastModified = dateLastModified.AddDays(1) });
                context.Cars.Add(new Car { Brand = "Alfa", Color = "Black", Vin = "no", Year = "1979", DateLastModified = dateLastModified.AddDays(2) });
                context.Cars.Add(new Car { Brand = "Alfa", Color = "Black", Vin = "a%bc", Year = "1979", DateLastModified = dateLastModified.AddDays(3) }); ;
                context.SaveChanges();
            }

            var selectNullableDateTime = context.Cars.FirstOrDefault(c => c.DateDeleted == dateDeleted);
            Console.WriteLine(selectNullableDateTime.Key);
        }
    }
}
