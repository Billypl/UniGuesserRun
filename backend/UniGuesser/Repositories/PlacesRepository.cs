using Microsoft.EntityFrameworkCore;
using PartyGame.Entities;
using PartyGame.Repositories.PartyGame.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PartyGame.Repositories
{
    public interface IPlacesRepository : IRepository<Place>
    {
        Task<long> GetPlacesCount();
        Task<List<Place>> GetPlacesByDifficulty(DifficultyLevel difficultyLevel);
    }

    public class PlacesRepository : Repository<Place>, IPlacesRepository
    {

        public PlacesRepository(GameDbContext gameDbContext) : base(gameDbContext)
        {
        }

        public override async Task<Place?> GetAsync(int id)
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

        public override async Task<Place?> GetByPublicIdAsync(Guid publicId)
        {
            return await _dbSet
                .Include(p => p.AuthorPlace)
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "PublicId") == publicId);
        }

        public override async Task<Place?> GetByPublicIdAsync(string publicId)
        {
            return await _dbSet
                .Include(p => p.AuthorPlace)
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "PublicId") == Guid.Parse(publicId));
        }

        public async Task<long> GetPlacesCount()
        {
            return await _dbSet.CountAsync(); 
        }

        public async Task<List<Place>> GetPlacesByDifficulty(DifficultyLevel difficultyLevel)
        {

            string difficulty = difficultyLevel.ToString();

            return await _dbSet
                .Where(p => p.DifficultyLevel == difficulty)
                .ToListAsync();
        }

    }
}
