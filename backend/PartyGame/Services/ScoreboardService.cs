using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Repositories;

namespace PartyGame.Services
{
    public interface IScoreboardService
    {
        void AddNewGame();
        Task<List<FinishedGame>> GetAllGames();
    }

    public class ScoreboardService : IScoreboardService
    {
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly IGameSessionRepository _gameSessionRepository;
        private readonly IScoreboardRepository _scoreboardRepository;

        const int ROUNDS_NUMBER = 5; // TODO: Need to get this value from appsettgins.json

        public ScoreboardService(
            IHttpContextAccessorService httpContextAccessorService,
            IGameSessionRepository gameSessionRepository,
            IScoreboardRepository scoreboardRepository)
        {
            _httpContextAccessorService = httpContextAccessorService;
            _gameSessionRepository = gameSessionRepository;
            _scoreboardRepository = scoreboardRepository;
        }

        public void AddNewGame()
        {

            string token = _httpContextAccessorService.GetTokenFromHeader();
            GameSession session =_gameSessionRepository.GetSessionByToken(token).Result;

            if (session == null)
            {
                throw new KeyNotFoundException($"Session was not found");
            }

            if (session.ActualRoundNumber != ROUNDS_NUMBER)
            {
                throw new Exception($"Game was not finished and cannot be saved");
            }

            FinishedGame newFinishedGame = new FinishedGame
            {
                Nickname = session.Nickname,
                FinalScore = session.GameScore,
                Rounds = session.Rounds,
                DifficultyLevel = session.DifficultyLevel
            };

            _scoreboardRepository.AddNewGame(newFinishedGame);
        }

        public async Task<List<FinishedGame>> GetAllGames()
        {
            List<FinishedGame> games = await _scoreboardRepository.GetAllGames();

            if (games == null)
            {
                throw new KeyNotFoundException($"There no games in history");
            }

            return games;
        }

       

    }
}
