using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeFirst.DataAccess.Configurations;

public class MoviesConfiguration : IEntityTypeConfiguration<Movies>
{
    public void Configure(EntityTypeBuilder<Movies> builder)
    {
        builder.HasKey(k => k.MovieId)
            .HasName("movies_pkey");

        // one to many movie - copies
        builder.HasMany(e => e.Copies)
            .WithOne(e => e.Movie);

        builder.Property(e => e.MovieId)
            .ValueGeneratedOnAdd()
            .HasColumnName("movie_id");

        builder.Property(e => e.Title)
            .HasColumnName("title");

        builder.Property(e => e.AgeRestriction)
            .HasColumnName("age_restriction");

        builder.Property(e => e.Price)
            .HasColumnName("price");

        builder.Property(e => e.Year)
            .HasColumnName("year");

        builder.ToTable("movies");

        builder.HasData(
            new Movies(1, "Star Wars Episode IV: A New Hope", 1979, 12, 10f),
            new Movies(2, "Ghostbusters", 1984, 12, 5.5f),
            new Movies(3, "Terminator", 1984, 15, 8.5f),
            new Movies(4, "Taxi Driver", 1976, 17, 5f),
            new Movies(5, "Platoon", 1986, 18, 5f),
            new Movies(6, "Frantic", 1988, 15, 8.5f),
            new Movies(7, "Ronin", 1998, 13, 9.5f),
            new Movies(8, "Analyze This", 1999, 16, 10.5f),
            new Movies(9, "Leon: the Professional", 1994, 16, 8.5f),
            new Movies(10, "Mission Impossible", 1996, 13, 8.5f));
    }
}