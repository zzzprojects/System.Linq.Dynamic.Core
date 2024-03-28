using System;
using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeFirst.DataAccess.Configurations;

public class RentalsConfiguration : IEntityTypeConfiguration<Rentals>
{
    public void Configure(EntityTypeBuilder<Rentals> builder)
    {
        builder.HasKey(e => new {e.ClientId, e.CopyId})
            .HasName("rentals_pkey");

        builder.HasOne(e => e.Client)
            .WithMany(e => e.Rentals)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(e => e.ClientId);
        builder.HasOne(e => e.Copy)
            .WithMany(e => e.Rentals)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(e => e.CopyId);

        builder.Property(e => e.ClientId)
            .IsRequired()
            .HasColumnName("client_id");
        builder.Property(e => e.CopyId)
            .IsRequired()
            .HasColumnName("copy_id");
        builder.Property(e => e.DateOfRental)
            .HasColumnName("date_of_rental");
        builder.Property(e => e.DateOfReturn)
            .HasColumnName("date_of_return");

        builder.ToTable("rentals");

        builder.HasData(
            new Rentals(1, 1, Convert.ToDateTime("2005-07-04"), Convert.ToDateTime("2005-07-05")),
            new Rentals(1, 6, Convert.ToDateTime("2005-07-19"), Convert.ToDateTime("2005-07-22")),
            new Rentals(2, 3, Convert.ToDateTime("2005-07-24"), Convert.ToDateTime("2005-07-25")),
            new Rentals(2, 5, Convert.ToDateTime("2005-07-26"), Convert.ToDateTime("2005-07-27")),
            new Rentals(2, 7, Convert.ToDateTime("2005-07-29"), Convert.ToDateTime("2005-07-30")),
            new Rentals(3, 12, Convert.ToDateTime("2005-07-10"), Convert.ToDateTime("2005-07-13")),
            new Rentals(3, 20, Convert.ToDateTime("2005-07-16"), Convert.ToDateTime("2005-07-17")),
            new Rentals(3, 3, Convert.ToDateTime("2005-07-22"), Convert.ToDateTime("2005-07-23")),
            new Rentals(3, 7, Convert.ToDateTime("2005-07-24"), Convert.ToDateTime("2005-07-25")),
            new Rentals(4, 13, Convert.ToDateTime("2005-07-01"), Convert.ToDateTime("2005-07-05")),
            new Rentals(5, 11, Convert.ToDateTime("2005-07-10"), Convert.ToDateTime("2005-07-13")),
            new Rentals(6, 1, Convert.ToDateTime("2005-07-06"), Convert.ToDateTime("2005-07-07")),
            new Rentals(6, 7, Convert.ToDateTime("2005-07-29"), Convert.ToDateTime("2005-07-30")),
            new Rentals(6, 19, Convert.ToDateTime("2005-07-29"), Convert.ToDateTime("2005-07-30")));
    }
}