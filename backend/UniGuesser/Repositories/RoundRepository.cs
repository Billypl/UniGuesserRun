using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories;

namespace Repositories
{
    public interface IRoundRepository:IRepository<Round>
    {

    }

    public class RoundRepository : Repository<Round>,IRoundRepository
    {
        public RoundRepository(GameDbContext context) : base(context)
        {
        }

        public override async Task<Round?> GetAsync(int id)
        {
            return await _dbSet
                .Include(r => r.PlaceToGuess)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public override async Task<IEnumerable<Round>> GetAllAsync()
        {
            return await _dbSet
                .Include(r => r.PlaceToGuess)
                .ToListAsync();
        }

        public override async Task<Round?> GetByPublicIdAsync(Guid publicId)
        {
            return await _dbSet
                .Include(r => r.PlaceToGuess)
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Guid") == publicId);
        }

        public override async Task<Round?> GetByPublicIdAsync(string publicId)
        {
            return await _dbSet
                .Include(r => r.PlaceToGuess)
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Guid") == Guid.Parse(publicId));
        }
    }
}
