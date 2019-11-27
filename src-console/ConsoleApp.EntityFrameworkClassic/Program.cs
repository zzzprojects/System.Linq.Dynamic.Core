using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace ConsoleApp.EntityFrameworkClassic
{
    internal class Program
    {
        public static string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EFClassic;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public static void Main()
        {
            // CLEAR
            using (var context = new EntityContext())
            {
                context.Customers.RemoveRange(context.Customers);
                context.SaveChanges();

                Console.WriteLine("Database clearer...");
            }

            // ADD 2 new customers
            using (var context = new EntityContext())
            {
                context.Customers.Add(new Customer { Name = "Customer_A", Description = "Description", IsActive = true });
                context.Customers.Add(new Customer { Name = "Customer_B", Description = "Description", IsActive = true });

                context.SaveChanges();

                Console.WriteLine("Customers added...");
            }

            using (var context = new EntityContext())
            {
                foreach (var customer in context.Customers.AsQueryable().Where("it != null && CustomerID >= 1000").ToList())
                {
                    Console.WriteLine("");
                    Console.WriteLine("Customer.CustomerID : " + customer.CustomerID);
                    Console.WriteLine("Customer.Name : " + customer.Name);
                    Console.WriteLine("Customer.Description : " + customer.Description);
                    Console.WriteLine("Customer.IsActive : " + customer.IsActive);
                }

                Console.WriteLine("");
                Console.WriteLine("---");
                Console.WriteLine("");
            }

            Console.WriteLine("Press any key.");
            Console.ReadKey();
        }

        public class EntityContext : DbContext
        {
            public EntityContext() : base(ConnectionString)
            {
            }

            public DbSet<Customer> Customers { get; set; }
        }

        public class Customer
        {
            public int CustomerID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
