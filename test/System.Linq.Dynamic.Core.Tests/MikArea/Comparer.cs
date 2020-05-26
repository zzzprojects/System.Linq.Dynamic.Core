using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.MikArea
{
    
    public class Comparer
    {
        public class Customer
        {
            public string City { get; set; }
            public List<Order> Orders { get; set; }
            public string CompanyName { get; set; }
            public string Phone { get; set; }
        }
        public class Order
        {
        }

        [Fact]
        public void Comparer_OrderBy_And_ThenBy()
        {
            List<Customer> customers = new List<Customer>() { new Customer() { CompanyName = "Ååå"} ,
                new Customer() { CompanyName = "Bbb" }  ,
                new Customer() { CompanyName = "Ååå" }  ,
                new Customer() { CompanyName = "Bbb" }  ,
                new Customer() { CompanyName = "Aaa" },
                new Customer() { CompanyName = "Aaa" },
            };

            CultureInfo culture = new CultureInfo("nb-NO");
            var test1 = customers.AsQueryable().OrderBy(x => x.CompanyName, StringComparer.Create(culture, true)).ToList();
            var test2 = customers.AsQueryable().OrderBy(x => x.CompanyName).ToList();
            var test3 = customers.AsQueryable().OrderBy(x => x.City).ThenBy(x => x.CompanyName, StringComparer.Create(culture, true)).ToList();
            var test2_V2 = customers.AsQueryable()
                .OrderBy("CompanyName");
            var test3_V2 = customers.AsQueryable()
                .OrderBy("City").ThenBy("CompanyName", StringComparer.Create(culture, true));
            var test1_V2 = customers.AsQueryable()
                .OrderBy("CompanyName", StringComparer.Create(culture, true));

            for (int i = 0; i < test2.Count; i++)
            { 
                Check.That(test2.ElementAt(i).CompanyName).IsEqualTo(test2_V2.ElementAt(i).CompanyName);
            }
            for (int i = 0; i < test1.Count; i++)
            {
                Check.That(test1.ElementAt(i).CompanyName).IsEqualTo(test1_V2.ElementAt(i).CompanyName);
            }
            for (int i = 0; i < test3.Count; i++)
            {
                Check.That(test3.ElementAt(i).CompanyName).IsEqualTo(test3_V2.ElementAt(i).CompanyName);
            }
        }

        [Fact]
        public void Comparer_GroupBy()
        {
            List<Customer> customers = new List<Customer>() { new Customer() { City = "ZZZ" }, new Customer() { City = "ZzZ" } };
            var check = customers.GroupBy(x => x.City, StringComparer.InvariantCultureIgnoreCase).ToList();
            var check2 = customers.GroupBy(x => x.City,x => new { Name = x.CompanyName, Phone = x.Phone }, StringComparer.InvariantCultureIgnoreCase).ToList();
            var check3 = customers.GroupBy(x => x.City, x => new { Name = x.CompanyName, Phone = x.Phone }).ToList();

            var check_V2 = customers.AsQueryable().GroupBy("City", StringComparer.InvariantCultureIgnoreCase);
            var check2_V2 = customers.AsQueryable().GroupBy("City", "new(CompanyName as Name, Phone)", StringComparer.InvariantCultureIgnoreCase);
            var check3_V2 = customers.AsQueryable().GroupBy("City", "new(CompanyName as Name, Phone)");
             
            Check.That(check.Count).IsEqualTo(check_V2.Count()); 
            Check.That(check2.Count).IsEqualTo(check2_V2.Count()); 
            Check.That(check3.Count).IsEqualTo(check3_V2.Count()); 
        }
    }
}
