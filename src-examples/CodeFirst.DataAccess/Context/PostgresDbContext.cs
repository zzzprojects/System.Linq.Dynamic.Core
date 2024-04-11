using CodeFirst.DataAccess.Configurations;
using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.DataAccess.Context;

public class PostgresDbContext : DbContext
{
    public PostgresDbContext(DbContextOptions options) : base(options)
    {
    }
        
    public DbSet<Movies> Movies { get; set; }
    public DbSet<Copies> Copies { get; set; }
    public DbSet<Starring> Starring { get; set; }
    public DbSet<Actors> Actors { get; set; }
    public DbSet<Rentals> Rentals { get; set; }
    public DbSet<Employees> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MoviesConfiguration());
        modelBuilder.ApplyConfiguration(new CopiesConfiguration());
        modelBuilder.ApplyConfiguration(new ActorsConfiguration());
        modelBuilder.ApplyConfiguration(new StarringConfiguration());
        modelBuilder.ApplyConfiguration(new RentalsConfiguration());
        modelBuilder.ApplyConfiguration(new ClientsConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeesConfiguration());
    }
}