using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Expressions;
using System.Text;

namespace Z.Dynamic.Core.Lab
{
    class Request_AddMethod
    {
        public class Customer
        {
            public string City { get; set; }
            public Dictionary<string, Order> Orders { get; set; }
            public string CompanyName { get; set; }
            public string Phone { get; set; }
        }
        public class Order
        {
        }

        public static void Execute()
        {
            List<Customer> customers = new List<Customer>()
            {
                new Customer() { City = "ZZZ1", CompanyName = "ZZZ", Orders = new Dictionary<string, Order>() },
                new Customer() { City = "ZZZ2", CompanyName = "ZZZ", Orders = new Dictionary<string, Order>()  },
                new Customer() { City = "ZZZ3", CompanyName = "ZZZ", Orders = new Dictionary<string, Order>()  }
            };
            customers.ForEach( x=> x.Orders.Add(x.City + "TEST", new Order()));

             

            var test = customers.Where(x => x.Orders.ContainsKey("A")).ToList();
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict.Add("A", 1);

            var dictExpression = Expression.Constant(dict);
            var containsKeyMethod = dict.GetType().GetMethod("ContainsKey");
            var callExpression = Expression.Call(dictExpression, containsKeyMethod, Expression.Constant("A"));

            var compiled = Expression.Lambda<Func<bool>>(callExpression).Compile();
            var result = compiled();
            var query = customers.AsQueryable()
                .Where("Orders.ContainsKey(\"ZZZ2TEST\")", "ZZZ", 1)
                .OrderBy("CompanyName")
                .Select("new(City as City, Phone)").ToDynamicList();
        } 
    }
}
