using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Repositories;

namespace PartyGame.Services
{
    public interface IGameSessionService
    {
        Task<GameSession> GetSessionByToken();
        void DeleteSessionByToken();
        void UpdateGameSession(GameSession session);
        void AddNewGameSession(GameSession session);
    }

    public class GameSessionService : IGameSessionService
    {
        private readonly IGameSessionRepository _gameSessionRepository;
        private readonly IHttpContextAccessorService _httpContextAccessorService;

        public GameSessionService(IGameSessionRepository gameSessionRepository, IHttpContextAccessorService httpContextAccessorService)
        {
            _gameSessionRepository = gameSessionRepository;
            _httpContextAccessorService = httpContextAccessorService;
        }

        public async Task<GameSession> GetSessionByToken()
        {
            string token = _httpContextAccessorService.GetTokenFromHeader();
            GameSession gameSession = await _gameSessionRepository.GetGameSessionByToken(token);
            if (gameSession == null)
            {
                throw new KeyNotFoundException($"GameSession with token {token} was not found.");
            }

            return gameSession;
        }

        public async void DeleteSessionByToken()
        {
            string token = _httpContextAccessorService.GetTokenFromHeader();
            DeleteResult deleteResult = await _gameSessionRepository.DeleteSessionByToken(token);
            if (deleteResult.DeletedCount == 0)
            {
                throw new KeyNotFoundException($"GameSession with token {token} was not found.");
            }
        }

        public async void UpdateGameSession(GameSession session)
        {
            UpdateResult updateResult = await _gameSessionRepository.UpdateGameSession(session);
            if (updateResult.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"GameSession with ID {session.Id} was not found.");
            }
        }

        public void AddNewGameSession(GameSession session)
        {
            GameSession existedSession = _gameSessionRepository.GetGameSessionByToken(session.Token).Result;
            if (existedSession != null)
            {
                throw new InvalidOperationException("A session with the same token already exists.");
            }
            _gameSessionRepository.AddNewGameSession(session);
        }


    }

   
}
