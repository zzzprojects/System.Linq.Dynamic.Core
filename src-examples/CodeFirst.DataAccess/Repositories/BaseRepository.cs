using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CodeFirst.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.DataAccess.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    private readonly DbContext _baseContext;
    private readonly DbSet<T> _dbSet;

    protected BaseRepository(DbContext baseContext)
    {
        _baseContext = baseContext;
        _dbSet = _baseContext.Set<T>();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Attach(entity);
        _baseContext.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void Delete(Expression<Func<T, bool>> where)
    {
        var objects = _dbSet.Where(where).AsEnumerable();
        foreach (var obj in objects)
            _dbSet.Remove(obj);
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> GetSingleAsync(Expression<Func<T, bool>> where)
    {
        return await _dbSet.FirstOrDefaultAsync(where);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> where)
    {
        return await _dbSet.Where(where).ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _baseContext.SaveChangesAsync() > 0;
    }
}