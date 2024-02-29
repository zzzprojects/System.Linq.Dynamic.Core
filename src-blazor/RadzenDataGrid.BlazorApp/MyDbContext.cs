using BlazorApp1.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PriceListService>(entity =>
        {
            entity.OwnsOne(e => e.ServiceBase, sb =>
            {
                sb.Property(p => p.Index);
                sb.Property(p => p.Code);
                sb.Property(p => p.ServiceName);
            });
        });
    }

    public DbSet<PriceListService> PriceListServices { get; set; } = null!;
}