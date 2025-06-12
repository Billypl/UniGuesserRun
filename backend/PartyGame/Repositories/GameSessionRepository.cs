using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PartyGame.Entities;
using PartyGame.Models.ScoreboardModels;
using PartyGame.Repositories.PartyGame.Repositories;

namespace PartyGame.Repositories
{
    public interface IGameSessionRepository:IRepository<GameSession>
    {
        Task<bool> DeleteGameSessionByPlayerId(int userId);
        Task<List<UserStats>> GetUsersStats(ScoreboardQuery scoreboardQuery);
        Task<List<GameSession>> GetGameHistoryPage(ScoreboardQuery scoreboardQuery);

        Task<GameSession?> GetActiveGameSession(string guid);
    }

    public class GameSessionRepository : Repository<GameSession>,IGameSessionRepository
    {
        public GameSessionRepository(GameDbContext context) : base(context)
        {
          
        }

        public override async Task<GameSession?> GetAsync(int id)
        {
            return await _dbSet
                .Include(g => g.Player)
                .Include(g => g.Rounds)
                  .ThenInclude(r => r.PlaceToGuess)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public override async Task<IEnumerable<GameSession>> GetAllAsync()
        {
            return await _dbSet
                .Include(g => g.Player)
                .Include(g => g.Rounds)
                .ToListAsync();
        }

        public override async Task<GameSession?> GetByPublicIdAsync(Guid publicId)
        {
            return await _dbSet
                .Include(g => g.Player)
                .Include(g => g.Rounds)
                    .ThenInclude(r => r.PlaceToGuess)
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "PublicId") == publicId);
        }

        public override async Task<GameSession?> GetByPublicIdAsync(string publicId)
        {
            return await _dbSet
                .Include(g => g.Player)
                .Include(g => g.Rounds)
                  .ThenInclude(r => r.PlaceToGuess)
                .FirstOrDefaultAsync(e => EF.Property<Guid>(e, "PublicId") == Guid.Parse(publicId));
        }

        public async Task<bool> DeleteGameSessionByPlayerId(int userId)
        {
            var entities = await _dbSet.Where(gs => gs.UserId == userId).ToListAsync();
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<GameSession>> GetGameHistoryPage(ScoreboardQuery scoreboardQuery)
        {
            // Startujemy z zapytaniem bazowym
            var query = _dbSet.AsQueryable()
                  .Where(gs => gs.UserId != null && gs.IsFinished);

            // Filtracja po poziomie trudności
            if (!string.IsNullOrEmpty(scoreboardQuery.DifficultyLevel))
            {
                query = query.Where(gs => gs.Difficulty.Contains(scoreboardQuery.DifficultyLevel));
            }

            // Filtracja po nickname
            if (!string.IsNullOrEmpty(scoreboardQuery.SearchNickname))
            {
                query = query
                    .Where(gs => gs.Player != null && gs.Player.Nickname.Equals(scoreboardQuery.SearchNickname, StringComparison.OrdinalIgnoreCase));
            }

            query = scoreboardQuery.SortDirection == SortDirection.ASC
                ? query.OrderBy(gs => gs.ExpirationDate)
                : query.OrderByDescending(gs => gs.ExpirationDate);

            var pagedScores = await query
               .Skip((scoreboardQuery.PageNumber - 1) * scoreboardQuery.PageSize)
               .Take(scoreboardQuery.PageSize)
               .ToListAsync();

            return pagedScores;
        }

        public async Task<List<UserStats>> GetUsersStats(ScoreboardQuery scoreboardQuery)
        {
            // Startujemy z zapytaniem bazowym
            var query = _dbSet.AsQueryable();

            // Filtracja po poziomie trudności
            if (!string.IsNullOrEmpty(scoreboardQuery.DifficultyLevel))
            {
                query = query.Where(gs => gs.Difficulty.Contains(scoreboardQuery.DifficultyLevel));
            }

            // Pobieramy dane z bazy i grupujemy wyniki
            List<UserStats> stats = await query
                .Where(gs => gs.UserId != null && gs.IsFinished)
                .GroupBy(gs => new { gs.UserId, gs.Player!.Nickname, gs.Player.PublicId })
                .Select(g => new UserStats
                {
                    Guid = g.Key.PublicId.ToString(),
                    Nickname = g.Key.Nickname,
                    GamePlayed = g.Count(),
                    AverageScore = g.Average(gs => gs.GameScore)
                })
                .ToListAsync();

            // Filtracja po nickname
            if (!string.IsNullOrEmpty(scoreboardQuery.SearchNickname))
            {
                stats = stats
                    .Where(gs => gs.Nickname.Contains(scoreboardQuery.SearchNickname, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Sortowanie po liczbie rozegranych gier
            stats = scoreboardQuery.SortDirection == SortDirection.ASC
                ? stats.OrderBy(gs => gs.GamePlayed).ToList()
                : stats.OrderByDescending(gs => gs.GamePlayed).ToList();

            // Paginacja


            return stats;
        }

        public async Task<GameSession?> GetActiveGameSession(string guid)
        {
           return await _dbSet.Where(g => g.IsFinished == false).FirstOrDefaultAsync(g => g.PublicId.ToString() == guid);
        }
    }
}
