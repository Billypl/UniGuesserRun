using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories;
using UniGuesser.Application.Models.Enumerations;
using UniGuesser.Application.Models.ScoreboardModels;
using UniGuesser.Domain.Entities;

namespace UniGuesser.Adapters.Outbound.Repositories
{
    public interface IGameSessionRepository : IRepository<GameSession>
    {
        Task<bool> DeleteGameSessionByPlayerId(int userId);
        Task<GameSession?> GetActiveGameSessionByPlayerId(Guid userGuid);
        Task<List<UserStats>> GetUsersStats(ScoreboardQuery scoreboardQuery);
        Task<List<GameSession>> GetGameHistoryPage(ScoreboardQuery scoreboardQuery);
        Task<GameSession?> GetActiveGameSession(Guid guid);
        Task<List<GameSession>> GetGameUserHistoryGames(UserHistoryQuery userHistoryQuery, Guid userGuid);
    }

    public class GameSessionRepository : Repository<GameSession>, IGameSessionRepository
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

        public async Task<GameSession?> GetActiveGameSessionByPlayerId(Guid userGuid)
        {
            return await _dbSet
                .Include(gs => gs.Player)
                .Include(gs => gs.Rounds)
                .FirstOrDefaultAsync(gs =>
                    gs.Player != null && gs.Player.PublicId == userGuid &&
                    gs.GameState == GameStatus.InProgress);
        }

        public async Task<List<GameSession>> GetGameHistoryPage(ScoreboardQuery scoreboardQuery)
        {
            var query = _dbSet.AsQueryable()
                .Where(gs => gs.UserId != null && gs.GameState != GameStatus.InProgress);

            if (!string.IsNullOrEmpty(scoreboardQuery.DifficultyLevel))
            {
                query = query.Where(gs => gs.Difficulty.Contains(scoreboardQuery.DifficultyLevel));
            }

            if (!string.IsNullOrEmpty(scoreboardQuery.SearchNickname))
            {
                query = query
                    .Where(gs => gs.Player != null && gs.Player.Nickname.Equals(scoreboardQuery.SearchNickname,
                        StringComparison.OrdinalIgnoreCase));
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
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(scoreboardQuery.DifficultyLevel))
            {
                query = query.Where(gs => gs.Difficulty.Contains(scoreboardQuery.DifficultyLevel));
            }

            List<UserStats> stats = await query
                .Where(gs => gs.UserId != null && gs.GameState != GameStatus.InProgress)
                .GroupBy(gs => new { gs.UserId, gs.Player!.Nickname, gs.Player.PublicId })
                .Select(g => new UserStats
                {
                    Guid = g.Key.PublicId.ToString(),
                    Nickname = g.Key.Nickname,
                    GamePlayed = g.Count(),
                    AverageScore = g.Average(gs => gs.GameScore)
                })
                .ToListAsync();

            if (!string.IsNullOrEmpty(scoreboardQuery.SearchNickname))
            {
                stats = stats
                    .Where(gs =>
                        gs.Nickname.Contains(scoreboardQuery.SearchNickname, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            stats = scoreboardQuery.SortDirection == SortDirection.ASC
                ? stats.OrderBy(gs => gs.GamePlayed).ToList()
                : stats.OrderByDescending(gs => gs.GamePlayed).ToList();

            return stats;
        }

        public async Task<GameSession?> GetActiveGameSession(Guid guid)
        {
            return await _dbSet.Where(g => g.GameState == GameStatus.InProgress)
                .FirstOrDefaultAsync(g => g.PublicId == guid);
        }

        public async Task<List<GameSession>> GetGameUserHistoryGames(UserHistoryQuery userHistoryQuery, Guid userGuid)
        {
 

            var query = _dbSet
                .Where(gs => gs.Player.PublicId == userGuid && gs.GameState != GameStatus.InProgress);

            if (!string.IsNullOrEmpty(userHistoryQuery.DifficultyLevel))
            {
                query = query.Where(gs => gs.Difficulty != null &&
                                          gs.Difficulty.Contains(userHistoryQuery.DifficultyLevel));
            }

            

            query = userHistoryQuery.SortDirection == SortDirection.ASC
                ? query.OrderBy(gs => gs.ExpirationDate)
                : query.OrderByDescending(gs => gs.ExpirationDate);

            //int skip = Math.Max(userHistoryQuery.PageNumber - 1, 0) * userHistoryQuery.PageSize;

            query = query.Include(gs => gs.Rounds);

            return await query
                //.Skip(skip)
                //.Take(userHistoryQuery.PageSize)
                .ToListAsync();
        }
    }
}   