using CodeFirst.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.DataAccess.Repositories;

public class EmployeesRepository : BaseRepository<Employees>
{
    public EmployeesRepository(DbContext baseContext) : base(baseContext)
    {
    }
}