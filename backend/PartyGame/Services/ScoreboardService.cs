using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Extensions.Exceptions;
using PartyGame.Models;
using PartyGame.Models.GameModels;
using PartyGame.Models.ScoreboardModels;
using PartyGame.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace PartyGame.Services
{
    public interface IScoreboardService
    {
        Task<List<FinishedGame>> GetAllGames();
        Task<PagedResult<FinishedGame>> GetFinishedGamesInScoreboard(ScoreboardQuery scoreboardQuery);

        Task SaveGame(FinishedGame finishedGameDto);
        
    }

    public class ScoreboardService : IScoreboardService
    {
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly IScoreboardRepository _scoreboardRepository;
        private readonly IScoreboardRepository _gameHistoryRepository;

        public ScoreboardService(
        IHttpContextAccessorService httpContextAccessorService,
        [FromKeyedServices("GameResults")] IScoreboardRepository scoreboardRepository,
        [FromKeyedServices("GameHistory")] IScoreboardRepository gameHistoryRepository)
        {
            _httpContextAccessorService = httpContextAccessorService;
            _scoreboardRepository = scoreboardRepository;
            _gameHistoryRepository = gameHistoryRepository;
        }

        public async Task<List<FinishedGame>> GetAllGames()
        {
            IEnumerable<FinishedGame> games = await _scoreboardRepository.GetAllAsync();

            if (games == null)
            {
                throw new NotFoundException($"There no games in history");
            }

            return games.ToList();
        }

        public async Task<PagedResult<FinishedGame>> GetFinishedGamesInScoreboard(ScoreboardQuery scoreboardQuery)
        {
            List<FinishedGame> games = await _scoreboardRepository.GetGames(scoreboardQuery);
            int totalScores =  (await _scoreboardRepository.GetAllAsync()).Count();

            var result = new PagedResult<FinishedGame>(games,totalScores, scoreboardQuery.PageSize, scoreboardQuery.PageNumber);
            return result;
        }

        public async Task SaveGame(FinishedGame finishedGame)
        {
            finishedGame.Id = ObjectId.GenerateNewId();

            // Tworzenie zapytania w celu uzyskania najlepszej gry użytkownika z scoreboard
            ScoreboardQuery scoreboardQuery = new ScoreboardQuery
            {
                DifficultyLevel = finishedGame.DifficultyLevel,
                SortDirection = Models.ScoreboardModels.SortDirection.DESC,
                PageNumber = 1,
                PageSize = 5,
                SearchNickname = finishedGame.Nickname
            };

            // Pobieranie najlepszej gry (najwyższy wynik) z tablicy wyników
            var games = await _scoreboardRepository.GetGames(scoreboardQuery);
            FinishedGame previousBest = games.FirstOrDefault();

            await _gameHistoryRepository.CreateAsync(finishedGame);
            await SaveBestGameInScoreboard(finishedGame, previousBest);
            
        }

        private async Task SaveBestGameInScoreboard(FinishedGame finishedGame,FinishedGame? previousBest)
        {
            if (previousBest == null) // to znaczy ze gra jest 1 
            {
                await _scoreboardRepository.CreateAsync(finishedGame);
            }
            else if (finishedGame.FinalScore > previousBest.FinalScore)
            {
                await _scoreboardRepository.DeleteAsync(previousBest.Id.ToString());
                await _scoreboardRepository.CreateAsync(finishedGame);
            }
        }







    }
}
