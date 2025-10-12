using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Repositories;
using UniGuesser.Infrastructure.SharedModels.ScoreboardModels;

namespace UniGuesser.Infrastructure
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

}