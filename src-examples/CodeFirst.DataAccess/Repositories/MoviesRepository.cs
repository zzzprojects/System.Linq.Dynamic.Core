using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.DataAccess.Repositories;

public class MoviesRepository : BaseRepository<Movies>
{
    public MoviesRepository(DbContext baseContext) : base(baseContext)
    {
    }
}