using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.DataAccess.Repositories;

public class StarringRepository : BaseRepository<Starring>
{
    public StarringRepository(DbContext baseContext) : base(baseContext)
    {
    }
}