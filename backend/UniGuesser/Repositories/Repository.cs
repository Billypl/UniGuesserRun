
namespace PartyGame.Repositories
{
    using global::PartyGame.Entities;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    namespace PartyGame.Repositories
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

        public abstract class Repository<T> : IRepository<T> where T : class
        {
            protected readonly GameDbContext _context;
            protected readonly DbSet<T> _dbSet;

            public Repository(GameDbContext context)
            {
                _context = context;
                _dbSet = context.Set<T>();
            }

            public async Task<T> CreateAsync(T entity)
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }

            public async Task<IEnumerable<T>> CreateManyAsync(IEnumerable<T> entities)
            {
                await _dbSet.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                return entities;
            }

            public virtual async Task<T?> GetAsync(int id)
            {
                return await _dbSet.FindAsync(id);
            }

            public virtual async Task<IEnumerable<T>> GetAllAsync()
            {
                return await _dbSet.ToListAsync();
            }

            public async Task UpdateAsync(T entity)
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }

            public async Task<bool> DeleteAsync(int id)
            {
                var entity = await GetAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                    await _context.SaveChangesAsync();
                    return true; 
                }
                return false; 
            }
            public virtual async Task<T?> GetByPublicIdAsync(Guid publicId)
            {
                return await _dbSet.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "PublicId") == publicId);
            }

            public virtual async Task<T?> GetByPublicIdAsync(string publicId)
            {
                var result = await _dbSet.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "PublicId") == Guid.Parse(publicId));
                return result; 
            }
        }
    }

}
