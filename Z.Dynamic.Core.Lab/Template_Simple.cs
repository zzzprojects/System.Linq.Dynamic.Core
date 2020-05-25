using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Z.Dynamic.Core.Lab
{
    public class Template_Simple
    {
        public class Customer
        {
            public string City { get; set; }
            public List<Order> Orders { get; set; }
            public string CompanyName { get; set; }
            public  string Phone { get; set; }
        }
        public class Order
        { 
        }

        public static void Execute()
        {
            List<Customer> customers = new List<Customer>() {new Customer() {City = "ZZZ", CompanyName = "ZZZ", Orders = new List<Order>() {new Order()}, Phone = "555 5555"}};

            var query = customers.AsQueryable()
                .Where("City == @0 and Orders.Count >= @1", "ZZZ", 1)
                .OrderBy("CompanyName")
                .Select("new(CompanyName as Name, Phone)").ToDynamicList();
        }
    }
}
