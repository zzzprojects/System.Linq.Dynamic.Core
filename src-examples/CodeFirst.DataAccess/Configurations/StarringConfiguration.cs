using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeFirst.DataAccess.Configurations;

public class StarringConfiguration : IEntityTypeConfiguration<Starring>
{
    public void Configure(EntityTypeBuilder<Starring> builder)
    {
        builder.HasKey(e => new {e.ActorId, e.MovieId});

        // configure many to many: movies - actors
        builder.HasOne(e => e.Movie)
            .WithMany(e => e.Starring)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(e => e.MovieId);
        builder.HasOne(e => e.Actor)
            .WithMany(e => e.Starring)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(e => e.ActorId);

        builder.Property(e => e.MovieId)
            .IsRequired()
            .HasColumnName("movie_id");

        builder.Property(e => e.ActorId)
            .IsRequired()
            .HasColumnName("actor_id");

        builder.ToTable("starring");

        builder.HasData(new Starring(2, 1),
            new Starring(3, 1),
            new Starring(4, 1),
            new Starring(5, 1),
            new Starring(6, 1),
            new Starring(7, 1),
            new Starring(8, 1),
            new Starring(1, 3),
            new Starring(9, 3),
            new Starring(10, 3),
            new Starring(11, 2),
            new Starring(12, 2),
            new Starring(13, 2),
            new Starring(14, 4),
            new Starring(15, 4),
            new Starring(16, 4),
            new Starring(17, 4),
            new Starring(18, 5),
            new Starring(19, 5),
            new Starring(20, 5),
            new Starring(21, 6),
            new Starring(22, 6),
            new Starring(14, 7),
            new Starring(23, 7),
            new Starring(14, 8),
            new Starring(24, 8),
            new Starring(25, 8),
            new Starring(23, 9),
            new Starring(27, 9),
            new Starring(23, 10),
            new Starring(28, 10));
    }
}