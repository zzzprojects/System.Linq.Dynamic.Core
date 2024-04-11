using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeFirst.DataAccess.Configurations;

public class CopiesConfiguration : IEntityTypeConfiguration<Copies>
{
    public void Configure(EntityTypeBuilder<Copies> builder)
    {
        builder.HasKey(e => e.CopyId)
            .HasName("copies_pkey");

        builder.Property(e => e.MovieId)
            .IsRequired()
            .HasColumnName("movie_id");

        builder.Property(e => e.CopyId)
            .HasColumnName("copy_id");

        builder.Property(e => e.Available)
            .HasColumnName("available");

        builder.ToTable("copies");
        builder.HasData(new Copies(1, 1, true),
            new Copies(2, 1, false),
            new Copies(3, 2, true),
            new Copies(4, 3, true),
            new Copies(5, 3, false),
            new Copies(6, 3, true),
            new Copies(7, 4, true),
            new Copies(8, 5, false),
            new Copies(9, 6, true),
            new Copies(10, 6, false),
            new Copies(11, 6, true),
            new Copies(12, 7, true),
            new Copies(13, 7, true),
            new Copies(14, 8, false),
            new Copies(15, 9, true),
            new Copies(16, 10, true),
            new Copies(17, 10, false),
            new Copies(18, 10, true),
            new Copies(19, 10, true),
            new Copies(20, 10, true));
    }
}