namespace CodeFirst.DataAccess.Models;

public class Starring
{
    public int ActorId { get; set; }
    public int MovieId { get; set; }
        
    // navigational properties
    public virtual Movies Movie { get; set; }
    public virtual Actors Actor { get; set; }

    public Starring()
    {
    }

    public Starring(int actorId, int movieId)
    {
        ActorId = actorId;
        MovieId = movieId;
    }
}