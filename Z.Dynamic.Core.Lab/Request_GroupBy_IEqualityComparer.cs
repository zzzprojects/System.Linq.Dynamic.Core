using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Z.Dynamic.Core.Lab
{
    class Request_GroupBy_IEqualityComparer
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
            List<Customer> customers = new List<Customer>() { new Customer() { City = "ZZZ" }, new Customer() { City = "ZzZ" } }; 
            var check = customers.GroupBy(x => x.City, StringComparer.InvariantCultureIgnoreCase).ToList();

            var query = customers.AsQueryable().GroupBy("City", StringComparer.InvariantCultureIgnoreCase).ToDynamicList();
            var queray = customers.AsQueryable().GroupBy("City", "new(CompanyName as Name, Phone)", StringComparer.InvariantCultureIgnoreCase).ToDynamicList();
            var queraay = customers.AsQueryable().GroupBy("City", "new(CompanyName as Name, Phone)" ).ToDynamicList();
        } 
    }
} 
