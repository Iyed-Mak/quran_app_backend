using Microsoft.EntityFrameworkCore;
using QuranSchool.Api.Data;
using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;

namespace QuranSchool.Api.Repositories.Implementations;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : class, IEntity
{
    protected readonly AppDbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public virtual async Task<T?> GetByIdAsync(int id)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

    public virtual async Task<List<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

    public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public virtual void Update(T entity)
    {
        var tracked = _context.ChangeTracker.Entries<T>()
            .FirstOrDefault(e => e.Entity.Id == entity.Id);
        if (tracked is not null)
        {
            _context.Entry(tracked.Entity).State = EntityState.Detached;
        }
        _dbSet.Update(entity);
    }

    public virtual void Delete(T entity) => _dbSet.Remove(entity);

    public virtual async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
