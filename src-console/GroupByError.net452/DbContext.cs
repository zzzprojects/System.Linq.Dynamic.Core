using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupByError
{
    public class MyDbContext : DbContext
    {
        public DbSet<Element> Element { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TestEFCore;Integrated Security=True;MultipleActiveResultSets=True");
        }
    }

    [Table("Element")]
    public class Element
    {
        [Key]
        public int Id { get; set; }
        public int Attribute1 { get; set; }
        public int Attribute2 { get; set; }
    }
}