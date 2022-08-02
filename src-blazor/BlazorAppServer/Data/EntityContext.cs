using Microsoft.EntityFrameworkCore;

namespace BlazorAppServer.Data;

public class EntityContext : DbContext
{
    public virtual DbSet<Contract> Contracts { get; set; }

    public EntityContext()
    {

    }

    public EntityContext(DbContextOptions<EntityContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Contracts;Integrated Security=True;Connect Timeout=30;TrustServerCertificate=True");
        }
    }
}