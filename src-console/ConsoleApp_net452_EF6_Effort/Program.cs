using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using ConsoleApp_net452_EF6.Entities;
using Effort;
using Newtonsoft.Json;

namespace ConsoleApp_net452_EF6
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = DbConnectionFactory.CreateTransient();

            using (var context = new KendoGridDbContext(connection))
            {
                context.KendoGridCountry.Add(new Country { Id = 1000, Code = "NL", Name = "Nederland" });

                var main1 = new MainCompany { Name = "Main1" };
                context.KendoGridMainCompany.Add(main1);

                var main2 = new MainCompany { Name = "Main2" };
                context.KendoGridMainCompany.Add(main2);

                var company = new Company { Name = "Other", MainCompany = main1 };
                context.KendoGridCompany.Add(company);

                context.SaveChanges();
            }

            using (var context = new KendoGridDbContext(connection))
            {
                foreach (var c in context.KendoGridCountry.AsNoTracking().Where(c => c.Code.StartsWith("N")))
                {
                    Console.WriteLine(JsonConvert.SerializeObject(c, Formatting.Indented));
                }

                var main2Id = context.KendoGridMainCompany.First(mc => mc.Name == "Main2").Id;

                var company = context.KendoGridCompany.First();
                company.MainCompanyId = main2Id;

                context.SaveChanges();
            }

            using (var context = new KendoGridDbContext(connection))
            {
                foreach (var c in context.KendoGridCompany.AsNoTracking())
                {
                    Console.WriteLine(c.Name + " " + c.MainCompany.Name);
                }
            }
        }
    }
}
