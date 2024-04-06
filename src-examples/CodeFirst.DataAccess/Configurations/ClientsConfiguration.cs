using System;
using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeFirst.DataAccess.Configurations;

public class ClientsConfiguration : IEntityTypeConfiguration<Clients>
{
    public void Configure(EntityTypeBuilder<Clients> builder)
    {
        builder.HasKey(e => e.ClientId)
            .HasName("client_pkey");
        builder.Property(e => e.ClientId)
            .HasColumnName("client_id");
        builder.Property(e => e.Firstname)
            .HasColumnName("first_name");
        builder.Property(e => e.Lastname)
            .HasColumnName("last_name");
        builder.Property(e => e.Birthday)
            .HasColumnName("birthday");

        builder.ToTable("clients");

        builder.HasData(
            new Clients(1, "Hank", "Hill", Convert.ToDateTime("1954-04-19")),
            new Clients(2, "Brian", "Griffin", Convert.ToDateTime("2011-09-11")),
            new Clients(3, "Gary", "Goodspeed", Convert.ToDateTime("1989-03-12")),
            new Clients(4, "Bob", "Belcher", Convert.ToDateTime("1977-01-23")),
            new Clients(5, "Lisa", "Simpson", Convert.ToDateTime("2012-05-09")),
            new Clients(6, "Rick", "Sanchez", Convert.ToDateTime("1965-03-17")));
    }
}