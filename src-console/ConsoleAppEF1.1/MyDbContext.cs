using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ConsoleAppEF1
{
    public class MyDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TestEFCore101;Integrated Security=True;MultipleActiveResultSets=True");
        }
    }

    [Table("Person")]
    public class Person
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public Address Address { get; set; }
    }

    [Table("Address")]
    public class Address
    {
        [Key]
        public int Id { get; set; }
        public string Street { get; set; }
    }
}
