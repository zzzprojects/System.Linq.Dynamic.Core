using System;
using System.Linq;
using ConsoleAppEF2.Database;
using System.Linq.Dynamic.Core;

namespace ConsoleAppEF31
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new TestContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            var dateDeleted = new DateTime(2019, 2, 2);

            var dateLastModified = new DateTime(2018, 1, 15);
            if (!context.Cars.Any())
            {
                context.Cars.Add(new Car { Brand = "Ford", Color = "Blue", Vin = "yes", Year = "2017", Extra = "e1", NullableInt = 1, DateLastModified = dateLastModified, DateDeleted = dateDeleted });
                context.Cars.Add(new Car { Brand = "Fiat", Color = "Red", Vin = "yes", Year = "2016", DateLastModified = dateLastModified.AddDays(1) });
                context.Cars.Add(new Car { Brand = "Alfa", Color = "Black", Vin = "no", Year = "1979", Extra = "e2", NullableInt = 2, DateLastModified = dateLastModified.AddDays(2) });
                context.Cars.Add(new Car { Brand = "Alfa", Color = "Black", Vin = "a%bc", Year = "1979", Extra = "e3", NullableInt = 3, DateLastModified = dateLastModified.AddDays(3), DateDeleted = dateDeleted.AddDays(1) }); ;
                context.Cars.Add(new Car { Brand = "Ford", Color = "Yellow", Vin = "no", Year = "2020", DateLastModified = dateLastModified });
                context.SaveChanges();
            }

            var npExtra1 = context.Cars.Select("np(Extra, \"no-extra\")").ToDynamicList();
            var npExtra2 = context.Cars.Select("np(Extra, string.Empty)").ToDynamicList();
            var npExtra3 = context.Cars.Any("np(Extra, string.Empty).ToUpper() == \"e1\"");

            var npNullableInt = context.Cars.Select("np(NullableInt, 42)").ToDynamicList();

            var selectNullableDateTime = context.Cars.FirstOrDefault(c => c.DateDeleted == dateDeleted);
            Console.WriteLine($"selectNullableDateTime.Key = {selectNullableDateTime.Key}");

            var orderByNullableDateTimeResult = context.Cars.OrderBy(c => c.DateDeleted);
            foreach (var x in orderByNullableDateTimeResult)
            {
                Console.WriteLine($"orderByNullableDateTimeResult.Key,DateDeleted = {x.Key},{x.DateDeleted}");
            }

            Console.WriteLine(new string('-', 80));

            var orderByNullableDateTimeDynamicResult = context.Cars.OrderBy("DateDeleted");
            foreach (var x in orderByNullableDateTimeDynamicResult)
            {
                Console.WriteLine($"orderByNullableDateTimeDynamicResult.Key,DateDeleted = {x.Key},{x.DateDeleted}");
            }

            Console.WriteLine(new string('-', 80));
            var orderByNullableDateTimeDynamicResultNew = context.Cars.Select("new (Color, DateDeleted)").OrderBy("DateDeleted desc");
            foreach (dynamic x in orderByNullableDateTimeDynamicResultNew)
            {
                Console.WriteLine($"orderByNullableDateTimeDynamicResult2.Color,DateDeleted = {x.Color},{x.DateDeleted}");
            }

            var users = new[] { new User { FirstName = "Doe" } }.AsQueryable();

            var resultDynamic = users.Any("c => np(c.FirstName, string.Empty).ToUpper() == \"DOE\"");
            Console.WriteLine(resultDynamic);
        }

        public class User
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmailAddress { get; set; }
        }
    }
}
