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
                    .ThenInclude(p => p.AuthorPlace)
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "PublicId") == publicId);
        }

        public override async Task<Round?> GetByPublicIdAsync(string publicId)
        {
            return await _dbSet
                .Include(r => r.PlaceToGuess)
                    .ThenInclude(p => p.AuthorPlace)

                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "PublicId") == Guid.Parse(publicId));
        }
    }
}
