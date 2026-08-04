using QuranSchool.Api.Models;
using QuranSchool.Api.Repositories.Interfaces;
using QuranSchool.Api.Services.Interfaces;

namespace QuranSchool.Api.Services.Implementations;

public class Service<T>(IRepository<T> repository) : IService<T> where T : class, IEntity
{
    protected readonly IRepository<T> _repository = repository;

    public virtual async Task<T?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

    public virtual async Task<List<T>> GetAllAsync() => await _repository.GetAllAsync();

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _repository.Update(entity);
        await _repository.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            return;
        }

        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }
}
