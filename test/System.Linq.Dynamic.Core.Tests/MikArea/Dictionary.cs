using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using FluentAssertions;
using NFluent;
using Xunit;

namespace System.Linq.Dynamic.Core.Tests.MikArea
{
    public class Dictionary
    {
        public class Customer
        {
            public string City { get; set; }
            public Dictionary<string, Order> Orders { get; set; }
            public IReadOnlyDictionary<string, Order> ReadOnlyOrders { get; set; }
            public string CompanyName { get; set; }
            public string Phone { get; set; }
        }

        public class Order
        {
        }

        [Fact]
        public void ReadOnlyDictionary_ContainsKey_1()
        {
            var orders = new ReadOnlyDictionary<string, Order>(new Dictionary<string, Order>
            {
                { "TEST", new Order() }
            });
            var customers = new List<Customer>
            {
                new Customer { City = "abc", CompanyName = "ZZZ", ReadOnlyOrders = orders }
            };

            var data = customers.AsQueryable()
                .Where("ReadOnlyOrders.ContainsKey(\"TEST\")")
                .ToList();

            data.Should().HaveCount(1);
        }

        [Fact]
        public void Dictionary_ContainsKey_1()
        {
            var customers = new List<Customer>
            {
                new Customer { City = "ZZZ1", CompanyName = "ZZZ", Orders = new Dictionary<string, Order>() },
                new Customer { City = "ZZZ2", CompanyName = "ZZZ", Orders = new Dictionary<string, Order>() },
                new Customer { City = "ZZZ3", CompanyName = "ZZZ", Orders = new Dictionary<string, Order>() }
            };
            customers.ForEach(x => x.Orders.Add(x.City + "TEST", new Order()));

            var data = customers.AsQueryable()
                .Where("Orders.ContainsKey(\"ZZZ2TEST\")")
                .OrderBy("CompanyName")
                .Select("new(City as City, Phone)").ToDynamicList();

            Check.That("ZZZ2").IsEqualTo((string)data.First().City);
        }

        [Fact]
        public void Dictionary_ContainsKey_2()
        {
            var customers = new List<Customer>
            {
                new Customer { City = "ZZZ1", CompanyName = "ZZZ", Orders = new Dictionary<string, Order>() },
                new Customer { City = "ZZZ2", CompanyName = "ZZZ", Orders = new Dictionary<string, Order>() },
                new Customer { City = "ZZZ3", CompanyName = "ZZZ", Orders = new Dictionary<string, Order>() }
            };
            customers.ForEach(x => x.Orders.Add(x.City + "TEST", new Order()));

            var data = customers.AsQueryable()
                .Where("Orders.ContainsKey(it.City + \"TEST\")")
                .OrderBy("City")
                .Select("new(City as City, Phone)").ToDynamicList();

            Check.That("ZZZ1").IsEqualTo((string)data.First().City);
            Check.That(3).IsEqualTo(data.Count);
        }

        [Fact]
        public void Dictionary_ContainsKey_3()
        {
            var customers = new List<Customer>
            {
                new Customer { City = "ZZZ1", CompanyName = "ZZZ", Orders = new Dictionary<string, Order>() },
                new Customer { City = "ZZZ2", CompanyName = "ZZZ", Orders = new Dictionary<string, Order>() },
                new Customer { City = "ZZZ3", CompanyName = "ZZZ", Orders = new Dictionary<string, Order>() }
            };
            customers.ForEach(x => x.Orders.Add(x.City + "TEST1", new Order()));
            customers.ForEach(x => x.Orders.Add(x.City + "TEST2", new Order()));
            var noDynamicList = customers
                .Where(x => x.Orders.Skip(1).First().Key == (x.City + "TEST2"))
                .OrderBy(x => x.City)
                .ToList();

            var data = customers.AsQueryable()
                .Where("Orders.Skip(1).First().Key == (it.City + \"TEST2\")")
                .OrderBy("City")
                .Select("new(City as City, Phone)").ToDynamicList();

            Check.That(3).IsEqualTo(noDynamicList.Count);
            Check.That("ZZZ1").IsEqualTo((string)data.First().City);
            Check.That(3).IsEqualTo(data.Count);
        }

#if NETCOREAPP3_1
        [Fact]
#else
        [Fact(Skip = "Fails in NET452 CI")]
#endif
        public void DynamicIndexCall() // https://github.com/zzzprojects/System.Linq.Dynamic.Core/issues/397
        {
            object CreateDicParameter(string name) => new Dictionary<string, object>
            {
                { "Name", new Dictionary<string, object> { {"FirstName", name }, { "LastName", name + "Test" } } }
            };

            var parType = new Dictionary<string, object>().GetType();
            var lambda = DynamicExpressionParser.ParseLambda(new[] { Expression.Parameter(parType, "item") }, typeof(object), "item.Name.FirstName + \"7\" + item.Name.LastName ").Compile();

            var x1 = lambda.DynamicInvoke(CreateDicParameter("Julio"));
            var x2 = lambda.DynamicInvoke(CreateDicParameter("John"));

            Check.That(x1).IsEqualTo("Julio7JulioTest");
            Check.That(x2).IsEqualTo("John7JohnTest");
        }
    }
}
