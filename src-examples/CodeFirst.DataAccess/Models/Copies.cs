using System.Collections.Generic;

namespace CodeFirst.DataAccess.Models;

public class Copies
{
    public int CopyId { get; set; }
    public bool Available { get; set; }
    public int MovieId { get; set; }

    // navigational properties
    // one copy has only one movie
    public virtual Movies Movie { get; set; }

    // to maintain many to many copies - rentals
    public virtual ICollection<Rentals> Rentals { get; set; }

    public Copies()
    {
    }

    public Copies(int copyId, int movieId, bool available)
    {
        CopyId = copyId;
        MovieId = movieId;
        Available = available;
    }
}