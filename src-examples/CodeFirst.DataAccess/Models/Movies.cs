using System.Collections.Generic;

namespace CodeFirst.DataAccess.Models;

public class Movies
{
    public int MovieId { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public int AgeRestriction { get; set; }
    public float Price { get; set; }

    public Movies()
    {
    }

    public Movies(int movieId, string title, int year, int ageRestriction, float price)
    {
        MovieId = movieId;
        Title = title;
        Year = year;
        AgeRestriction = ageRestriction;
        Price = price;
    }

    // navigational properties
    // one movie may have many copies
    public virtual ICollection<Copies> Copies { get; set; }
    // this is composite key to get many to many movies - actors
    public virtual ICollection<Starring> Starring { get; set; }

    public override string ToString()
    {
        return $"{MovieId}, {Title}, {Year}, {Price}";
    }
}