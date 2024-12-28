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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GameSessionService(IGameSessionRepository gameSessionRepository, IHttpContextAccessor httpContextAccessor)
        {
            _gameSessionRepository = gameSessionRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTokenFromHeader()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (token == null)
            {
                throw new KeyNotFoundException("Token was not found in header");
            }

            return token;
        }

        public async Task<GameSession> GetSessionByToken()
        {
            var token = GetTokenFromHeader();

            var gameSession = await _gameSessionRepository.GetGameSessionByToken(token);

            if (gameSession == null)
            {
                throw new KeyNotFoundException($"GameSession with token {token} was not found.");
            }

            return gameSession;
        }

        public async void DeleteSessionByToken()
        {
            var token = GetTokenFromHeader();

            var deleteResult = await _gameSessionRepository.DeleteSessionByToken(token);
            if (deleteResult.DeletedCount == 0)
            {
                throw new KeyNotFoundException($"GameSession with token {token} was not found.");
            }
        }

        public async void UpdateGameSession(GameSession session)
        {
            var updateResult = await _gameSessionRepository.UpdateGameSession(session);
            if (updateResult.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"GameSession with ID {session.Id} was not found.");
            }
        }

        public async Task<GameSession> GetGameSessionByToken()
        {
            var token = GetTokenFromHeader();

            var gameSession = await _gameSessionRepository.GetGameSessionByToken(token);

            if (gameSession == null)
            {
                throw new KeyNotFoundException($"Session with token ${token} was not found");
            }

            return gameSession;
        }

        public async void AddNewGameSession(GameSession session)
        {
            var result = _gameSessionRepository.GetGameSessionByToken(session.Token).Result;
            if (result != null)
            {
                throw new InvalidOperationException("A session with the same token already exists.");
            }

            _gameSessionRepository.AddNewGameSession(session);
        }


    }

   
}
