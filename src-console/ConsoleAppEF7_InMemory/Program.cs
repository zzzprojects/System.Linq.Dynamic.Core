using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace DynamicLinqEfCoreExample;

internal class Program
{
    static void Main(string[] args)
    {
        var options = new DbContextOptionsBuilder<ExampleContext>().UseInMemoryDatabase(databaseName: "Example").Options;

        using var context = new ExampleContext(options);
        Root root = new()
        {
            Id = 1,
            Children = new Child[]
            {
                new()
                {
                    Id = 2,
                    Grandchildren = new Grandchild[]
                    {
                        new()
                        {
                            Id = 3
                        }
                    }
                }
            }
        };
        context.Roots.Add(root);
        context.SaveChanges();

        // LINQ to Objects (works)
        var data = new[] { root }.AsQueryable();
        //var a = data.Select(r => r.Children.SelectMany(c => c.Grandchildren)).ToArray();    // <-- works
        //var b = data.Select("Children.SelectMany(Grandchildren)").ToDynamicArray();         // <-- works

        // LINQ to Entities (fails)
        var c = context.Roots.Select(r => r.Children.SelectMany(c => c.Grandchildren)).ToArray();    // <-- works
        var d = context.Roots.Select("Children.SelectMany(Grandchildren)").ToDynamicArray();         // <-- throws error (works if Children and Grandchildren properties are defined as IEnumerable instead of ICollection)
    }

    public class Root
    {
        public int Id { get; set; }
        public ICollection<Child> Children { get; set; } = new HashSet<Child>();    // <-- dynamic LINQ query works if this and Child.Grandchildren are IEnumerable instead of ICollection
    }

    public class Child
    {
        public int Id { get; set; }
        public ICollection<Grandchild> Grandchildren { get; set; } = new HashSet<Grandchild>(); // <-- dynamic LINQ query works if this and Root.Children are IEnumerable instead of ICollection
    }

    public class Grandchild
    {
        public int Id { get; set; }
    }

    public class ExampleContext : DbContext
    {
        public ExampleContext() : base()
        {
        }

        public ExampleContext(DbContextOptions<ExampleContext> options)
            : base(options)
        {
        }

        public DbSet<Root> Roots { get; set; }
    }
}