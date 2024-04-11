using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.DataAccess.Repositories;

public class ClientsRepository : BaseRepository<Clients>
{
    public ClientsRepository(DbContext baseContext) : base(baseContext)
    {
    }
}