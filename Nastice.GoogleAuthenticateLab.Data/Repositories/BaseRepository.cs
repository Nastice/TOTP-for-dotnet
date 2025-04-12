using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nastice.GoogleAuthenticateLab.Data.Interfaces;

namespace Nastice.GoogleAuthenticateLab.Data.Repositories;

public class BaseRepository<TEntity> : IRepositoryBase<TEntity> where TEntity : class
{
    private readonly DbContext _context;

    protected BaseRepository(DbContext context)
    {
        _context = context;
    }

    public virtual  IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null)
    {
        var query = _context.Set<TEntity>().AsQueryable();

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return query;
    }

    public virtual  TEntity? Get(Expression<Func<TEntity, bool>> predicate)
        => _context.Set<TEntity>().Where(predicate).FirstOrDefault();

    public virtual  async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate)
        => await _context.Set<TEntity>().Where(predicate).FirstOrDefaultAsync();

    public void Create(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
    }

    public void CreateRange(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().AddRange(entities);
    }

    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }

    public virtual  int SaveChanges(CancellationToken cancellationToken)
        => _context.SaveChanges();

    public virtual async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();
}