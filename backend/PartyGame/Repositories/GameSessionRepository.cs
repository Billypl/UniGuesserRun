using AutoMapper;
using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Repositories
{
    public interface IGameSessionRepository
    {
        Task<GameSession> GetSessionByToken(string token);
        Task<DeleteResult> DeleteSessionByToken(string token);
        Task<UpdateResult> UpdateGameSession(GameSession session);
        Task<GameSession> GetGameSessionByToken(string token);
        void AddNewGameSession(GameSession session);
    }

    public class GameSessionRepository : IGameSessionRepository
    {
        private readonly GameDbContext _gameDbContext;

        public GameSessionRepository(GameDbContext gameDbContext)
        {
            _gameDbContext = gameDbContext;
        }
        public async Task<GameSession> GetSessionByToken(string token)
        {
            return await _gameDbContext.GameSessions
                .Find(gs => gs.Token == token)
                .FirstOrDefaultAsync();
        }

        public async Task<DeleteResult> DeleteSessionByToken(string token)
        {
            return await _gameDbContext.GameSessions.DeleteOneAsync(gs => gs.Token == token);
        }

        public async Task<UpdateResult> UpdateGameSession(GameSession session)
        {
            var filter = Builders<GameSession>.Filter.Eq(s => s.Id, session.Id);
            var update = Builders<GameSession>.Update
                .Set(s => s.Rounds, session.Rounds)
                .Set(s => s.ActualRoundNumber, session.ActualRoundNumber)
                .Set(s => s.GameScore, session.GameScore)
                .Set(s => s.ExpirationDate, session.ExpirationDate)
                .Set(s => s.Token, session.Token)
                .Set(s => s.Id, session.Id)
                .Set(s => s.DifficultyLevel, session.DifficultyLevel);
            
            return await _gameDbContext.GameSessions.UpdateOneAsync(filter, update);
        }

        public async Task<GameSession> GetGameSessionByToken(string token)
        {
            return await _gameDbContext.GameSessions
                .Find(s => s.Token == token)
                .FirstOrDefaultAsync();
        }

        public async void AddNewGameSession(GameSession session)
        { 
            await _gameDbContext.GameSessions.InsertOneAsync(session);
        }

    }
}
