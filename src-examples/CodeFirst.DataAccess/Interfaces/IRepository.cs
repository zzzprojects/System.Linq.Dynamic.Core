using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CodeFirst.DataAccess.Interfaces;

public interface IRepository<T>
{
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    void Delete(Expression<Func<T, bool>> where);
    Task<T> GetByIdAsync(int id);
    Task<T> GetSingleAsync(Expression<Func<T, bool>> where);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> where);
    Task<bool> SaveChangesAsync();
}