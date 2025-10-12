

namespace UniGuesser.Domain.Repositories
{

    public interface IRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> CreateManyAsync(IEnumerable<T> entities);
        Task<T?> GetAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<T?> GetByPublicIdAsync(Guid publicId);
        Task<T?> GetByPublicIdAsync(string publicId);
    }

}
