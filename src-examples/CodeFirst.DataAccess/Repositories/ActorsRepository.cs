using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.DataAccess.Repositories;

public class ActorsRepository : BaseRepository<Actors>
{
    public ActorsRepository(DbContext baseContext) : base(baseContext)
    {
    }
}