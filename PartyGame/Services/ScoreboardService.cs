using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Services
{
    public interface IScoreboardService
    {
        string GetTokenFromHeader();
        Task<int> AddNewGame(string nickname);
        Task<List<FinishedGame>> GetAllGames();
    }

    public class ScoreboardService : IScoreboardService
    {
        private readonly GameDbContext _gameDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISessionService _sessionService;

        const int ROUNDS_NUMBER = 5; // TODO: Need to get this value from appsettgins.json


        public ScoreboardService(GameDbContext gameDbContext,
            IHttpContextAccessor httpContextAccessor,
            ISessionService sessionService)
        {
            _gameDbContext = gameDbContext;
            _httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;
        }

        public string GetTokenFromHeader()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();
            return token;
        }


        public async Task<int> AddNewGame(string nickname)
        {
            var token = GetTokenFromHeader();
            if (token == null)
            {
                throw new KeyNotFoundException($"Token was not found");
            }

            var session =_sessionService.GetSessionByToken(token).Result;

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

            _gameDbContext.GameResults.InsertOneAsync(newFinishedGame);

            return 1;
        }

        public async Task<List<FinishedGame>> GetAllGames()
        {
            var games = await _gameDbContext.GameResults.Find(FilterDefinition<FinishedGame>.Empty).ToListAsync();

            return games;
        }

    }
}
