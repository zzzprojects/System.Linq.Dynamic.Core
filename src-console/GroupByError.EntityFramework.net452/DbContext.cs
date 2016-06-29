using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace GroupByError
{
    [DbConfigurationType(typeof(CodeConfig))]
    public class MyDbContext : DbContext
    {
        public DbSet<Element> Elements { get; set; }

        public MyDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=TestEFCore;Integrated Security=True;MultipleActiveResultSets=True");
        //}
    }

    public class CodeConfig : DbConfiguration
    {
        public CodeConfig()
        {
            SetProviderServices("System.Data.SqlClient", System.Data.Entity.SqlServer.SqlProviderServices.Instance);
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