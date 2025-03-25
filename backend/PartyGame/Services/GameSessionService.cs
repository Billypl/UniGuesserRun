using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Repositories;

namespace PartyGame.Services
{
    public interface IGameSessionService
    {
        Task DeleteSessionById(string id);
        Task DeleteSessionByHeader();
        Task<GameSession> GetSessionById(string id);
        Task UpdateGameSession(GameSession session);
        Task AddNewGameSession(GameSession session);
        Task<bool> HasActiveGameSession(string token);

    }

    public class GameSessionService : IGameSessionService
    {
        private readonly IGameSessionRepository _gameSessionRepository;
        private readonly IHttpContextAccessorService _httpContextAccessorService;

        public GameSessionService(IGameSessionRepository gameSessionRepository,IHttpContextAccessorService httpContextAccessorService)
        {
            _gameSessionRepository = gameSessionRepository;
            _httpContextAccessorService = httpContextAccessorService;
        }


        public async Task DeleteSessionById(string id)
        {
            DeleteResult deleteResult = await _gameSessionRepository.DeleteAsync(id);
            if (deleteResult.DeletedCount == 0)
            {
                throw new KeyNotFoundException($"GameSession with id {id} was not found.");
            }
        }

        public async Task DeleteSessionByHeader()
        {
            string id = _httpContextAccessorService.GetGameSessionIdFromHeader();

            await DeleteSessionById(id);
        }

        public async Task<GameSession> GetSessionById(string id)
        {
            GameSession gameSession = await _gameSessionRepository.GetAsync(id);

            if (gameSession is null)
            {
                throw new KeyNotFoundException($"Game session with token {id} was not found");
            }
            return gameSession;
        }

        public async Task UpdateGameSession(GameSession session)
        {
            var existingSession = await _gameSessionRepository.GetAsync(session.Id);

            if (existingSession == null)
            {
                throw new KeyNotFoundException($"GameSession with ID {session.Id} was not found.");
            }

            await _gameSessionRepository.UpdateAsync(session);
        }

        public async Task AddNewGameSession(GameSession session)
        {
            GameSession existedSession = await _gameSessionRepository.GetAsync(session.Id);
            if (existedSession != null)
            {
                throw new InvalidOperationException("A session with the same token already exists.");
            }
            await _gameSessionRepository.CreateAsync(session);
        }

        public async Task<bool> HasActiveGameSession(string id)
        {
            var existingSession = await _gameSessionRepository.GetAsync(id);

            if (existingSession is null)
            {
                return false;
            }

            return true;
        }

    }

   
}
