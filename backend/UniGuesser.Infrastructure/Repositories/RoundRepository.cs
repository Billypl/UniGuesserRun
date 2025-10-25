using Microsoft.EntityFrameworkCore;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Repositories;
using UniGuesser.Infrastructure.Persistence;

namespace UniGuesser.Infrastructure.Repositories
{

    public class RoundRepository : Repository<Round>, IRoundRepository
    {
        public RoundRepository(GameDbContext context) : base(context)
        {
        }

        public override async Task<Round?> GetAsync(Guid id)
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

    }
}
