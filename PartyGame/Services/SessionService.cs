using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Services
{
    public interface ISessionService
    {
        Task<GameSession> GetSessionByToken(string token);
        void DeleteSessionByToken(string token);
        void UpdateSessionRound(GameSession session);
    }

    public class SessionService : ISessionService
    {
        private readonly GameDbContext _gameDbContext;

        public SessionService(GameDbContext dbContext)
        {
            _gameDbContext = dbContext;
        }

        public async Task<GameSession> GetSessionByToken(string token)
        {
            var gameSession = await _gameDbContext.GameSessions
                .Find(gs => gs.Token == token)
                .FirstOrDefaultAsync();
            if (gameSession == null)
            {
                throw new KeyNotFoundException($"GameSession with token {token} was not found.");
            }

            return gameSession;
        }

        public async void DeleteSessionByToken(string token)
        {
            var deleteResult = await _gameDbContext.GameSessions.DeleteOneAsync(gs => gs.Token == token);

            if (deleteResult.DeletedCount == 0)
            {
                throw new KeyNotFoundException($"GameSession with token {token} was not found.");
            }
        }

        public async void UpdateSessionRound(GameSession session)
        {
            var filter = Builders<GameSession>.Filter.Eq(s => s.Id, session.Id);
            var update = Builders<GameSession>.Update
                .Set(s => s.Rounds, session.Rounds)
                .Set(s => s.ActualRoundNumber, session.ActualRoundNumber);

            var updateResult = await _gameDbContext.GameSessions.UpdateOneAsync(filter, update);

            if (updateResult.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"GameSession with ID {session.Id} was not found.");
            }
        }

    }

   
}
