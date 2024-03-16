using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.DataAccess.Repositories;

public class RentalsRepository : BaseRepository<Rentals>
{
    public RentalsRepository(DbContext baseContext) : base(baseContext)
    {
    }
}