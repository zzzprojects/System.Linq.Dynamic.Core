using System;
using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeFirst.DataAccess.Configurations;

public class ActorsConfiguration : IEntityTypeConfiguration<Actors>
{
    public void Configure(EntityTypeBuilder<Actors> builder)
    {
        builder.HasKey(e => e.ActorId)
            .HasName("actor_pkey");
        builder.Property(e => e.ActorId)
            .HasColumnName("actor_id");
        builder.Property(e => e.Firstname)
            .HasColumnName("first_name");
        builder.Property(e => e.Lastname)
            .HasColumnName("last_name");
        builder.Property(e => e.Birthday)
            .HasColumnName("birthday");
        builder.ToTable("actors");

        builder.HasData(
            new Actors(1, "Arnold", "Schwarzenegger", Convert.ToDateTime("1947-07-30")),
            new Actors(2, "Anthony", "Daniels", Convert.ToDateTime("1946-02-21")),
            new Actors(3, "Harrison", "Ford", Convert.ToDateTime("1942-07-13")),
            new Actors(4, "Carrie", "Fisher", Convert.ToDateTime("1956-10-21")),
            new Actors(5, "Alec", "Guiness", Convert.ToDateTime("1914-04-02")),
            new Actors(6, "Peter", "Cushing", Convert.ToDateTime("1913-05-26")),
            new Actors(7, "David", "Prowse", Convert.ToDateTime("1944-05-19")),
            new Actors(8, "Peter", "Mayhew", Convert.ToDateTime("1935-07-01")),
            new Actors(9, "Michael", "Biehn", Convert.ToDateTime("1956-07-31")),
            new Actors(10, "Linda", "Hamilton", Convert.ToDateTime("1956-09-26")),
            new Actors(11, "Bill", "Murray", Convert.ToDateTime("1950-09-21")),
            new Actors(12, "Dan", "Aykroyd", Convert.ToDateTime("1952-07-01")),
            new Actors(13, "Sigourney", "Weaver", Convert.ToDateTime("1949-10-08")),
            new Actors(14, "Robert", "De Niro", Convert.ToDateTime("1943-08-17")),
            new Actors(15, "Jodie", "Foster", Convert.ToDateTime("1962-11-19")),
            new Actors(16, "Harvey", "Keitel", Convert.ToDateTime("1939-05-13")),
            new Actors(17, "Cybill", "Shepherd", Convert.ToDateTime("1950-02-18")),
            new Actors(18, "Tom", "Berenger", Convert.ToDateTime("1949-05-31")),
            new Actors(19, "Willem", "Dafoe", Convert.ToDateTime("1955-07-22")),
            new Actors(20, "Charlie", "Sheen", Convert.ToDateTime("1965-09-03")),
            new Actors(21, "Harrison", "Ford", Convert.ToDateTime("1942-07-13")),
            new Actors(22, "Emmanuelle", "Seigner", Convert.ToDateTime("1966-06-22")),
            new Actors(23, "Jean", "Reno", Convert.ToDateTime("1948-07-30")),
            new Actors(24, "Billy", "Crystal", Convert.ToDateTime("1948-03-14")),
            new Actors(25, "Lisa", "Kudrow", Convert.ToDateTime("1963-07-30")),
            new Actors(26, "Gary", "Oldman", Convert.ToDateTime("1958-03-21")),
            new Actors(27, "Natalie", "Portman", Convert.ToDateTime("1981-06-09")),
            new Actors(28, "Tom", "Cruise", Convert.ToDateTime("1962-07-03")));
    }
}