using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeFirst.DataAccess.Configurations;

public class EmployeesConfiguration : IEntityTypeConfiguration<Employees>
{
    public void Configure(EntityTypeBuilder<Employees> builder)
    {
        builder.HasKey(e => e.EmployeeId)
            .HasName("employee_id");
        builder.Property(e => e.EmployeeId)
            .HasColumnName("employee_id");
        builder.Property(e => e.Firstname)
            .IsRequired()
            .HasColumnName("first_name");
        builder.Property(e => e.Lastname)
            .IsRequired()
            .HasColumnName("last_name");
        builder.Property(e => e.Salary)
            .HasColumnName("salary");

        builder.ToTable("employees");

        builder.HasData(
            new Employees
                {EmployeeId = 1, Firstname = "John", Lastname = "Smith", Salary = 150.0f, City = "New York"},
            new Employees
                {EmployeeId = 2, Firstname = "Ben", Lastname = "Johnson", Salary = 250.0f, City = "New York"},
            new Employees
                {EmployeeId = 3, Firstname = "Louis", Lastname = "Armstrong", Salary = 75.0f, City = "New Orleans"},
            new Employees
                {EmployeeId = 4, Firstname = "John", Lastname = "Lennon", Salary = 300.0f, City = "London"},
            new Employees
                {EmployeeId = 5, Firstname = "Peter", Lastname = "Gabriel", Salary = 150.0f, City = "London"}
        );
    }
}