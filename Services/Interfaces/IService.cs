using QuranSchool.Api.Models;

namespace QuranSchool.Api.Services.Interfaces;

public interface IService<T> where T : class, IEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
