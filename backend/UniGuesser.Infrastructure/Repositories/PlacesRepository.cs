using Microsoft.EntityFrameworkCore;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Repositories;
using UniGuesser.Domain.ValueObjects.Enumerations;
using UniGuesser.Infrastructure.Persistence;

namespace UniGuesser.Infrastructure.Repositories
{
    public class PlacesRepository : Repository<Place>, IPlacesRepository
    {

        public PlacesRepository(GameDbContext gameDbContext) : base(gameDbContext)
        {
        }

        public override async Task<Place?> GetAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.AuthorPlace)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public override async Task<IEnumerable<Place>> GetAllAsync()
        {
            return await _dbSet
                .Include(p => p.AuthorPlace)
                .ToListAsync();
        }

        public async Task<long> GetPlacesCount()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<List<Place>> GetPlacesByDifficulty(DifficultyLevel difficultyLevel)
        {

            return await _dbSet
                .Where(p => p.DifficultyLevel == difficultyLevel)
                .ToListAsync();
        }

    }
}
