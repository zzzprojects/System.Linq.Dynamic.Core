using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Z.Dynamic.Core.Lab
{
    class Request_OrderBy_StringComparer
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

        public static void Execute()
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
            var test3 = customers.AsQueryable()
                .OrderBy("City").ThenBy("CompanyName", StringComparer.Create(culture, true)).ToDynamicList();
            var test4 = customers.AsQueryable() 
                .OrderBy("CompanyName", StringComparer.Create(culture, true)).ToDynamicList(); 
        }
    }
}
