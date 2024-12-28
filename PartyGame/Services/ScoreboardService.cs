using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Repositories;

namespace PartyGame.Services
{
    public interface IScoreboardService
    {
        void AddNewGame(string nickname);
        Task<List<FinishedGame>> GetAllGames();
    }

    public class ScoreboardService : IScoreboardService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGameSessionRepository _gameSessionRepository;
        private readonly IScoreboardRepository _scoreboardRepository;

        const int ROUNDS_NUMBER = 5; // TODO: Need to get this value from appsettgins.json

        public ScoreboardService(
            IHttpContextAccessor httpContextAccessor,
            IGameSessionRepository gameSessionRepository,
            IScoreboardRepository scoreboardRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _gameSessionRepository = gameSessionRepository;
            _scoreboardRepository = scoreboardRepository;
        }

        private string GetTokenFromHeader()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (token == null)
            {
                throw new KeyNotFoundException($"Token was not found");
            }

            return token;
        }

        public void AddNewGame(string nickname)
        {

            var token = GetTokenFromHeader();
            var session =_gameSessionRepository.GetSessionByToken(token).Result;

            if (session == null)
            {
                throw new KeyNotFoundException($"Session was not found");
            }

            if (session.ActualRoundNumber != ROUNDS_NUMBER)
            {
                throw new Exception($"Game was not finished");
            }

            FinishedGame newFinishedGame = new FinishedGame
            {
                Nickname = nickname,
                FinalScore = session.GameScore,
                Rounds = session.Rounds,
            };

            _scoreboardRepository.AddNewGame(newFinishedGame);
        }

        public async Task<List<FinishedGame>> GetAllGames()
        {
            var games = await _scoreboardRepository.GetAllGames();

            if (games == null)
            {
                throw new KeyNotFoundException($"There no games in history");
            }

            return games;
        }

    }
}
