using AutoMapper;
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
        Task<List<FinishedGameDto>> GetAllGames();
        Task<PagedResult<FinishedGameToTable>> GetFinishedGamesInScoreboard(ScoreboardQuery scoreboardQuery);
        Task<PagedResult<FinishedGameToTable>> GetGameHistoryPage(ScoreboardQuery scoreboardQuery);
        Task SaveGame(FinishedGame finishedGameDto);
        Task<FinishedGameDto> GetGameResult(string repository);
        
    }

    public class GameResultService : IScoreboardService
    {
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly IScoreboardRepository _scoreboardRepository;
        private readonly IScoreboardRepository _gameHistoryRepository;
        private readonly IMapper _mapper;

        public GameResultService(
        IHttpContextAccessorService httpContextAccessorService,
        [FromKeyedServices("GameResults")] IScoreboardRepository scoreboardRepository,
        [FromKeyedServices("GameHistory")] IScoreboardRepository gameHistoryRepository,
        IMapper mapper)
        {
            _httpContextAccessorService = httpContextAccessorService;
            _scoreboardRepository = scoreboardRepository;
            _gameHistoryRepository = gameHistoryRepository;
            _mapper = mapper;
        }

        public async Task<List<FinishedGameDto>> GetAllGames()
        {
            IEnumerable<FinishedGame> games = await _scoreboardRepository.GetAllAsync();

            if (games == null)
            {
                throw new NotFoundException($"There no games in history");
            }

            return _mapper.Map<IEnumerable<FinishedGameDto>>(games).ToList();
        }

        public async Task<PagedResult<FinishedGameToTable>> GetFinishedGamesInScoreboard(ScoreboardQuery scoreboardQuery)
        {
            List<FinishedGame> games = await _scoreboardRepository.GetGames(scoreboardQuery);
            int totalScores =  (await _scoreboardRepository.GetAllAsync()).Count();

            var mappedGames = _mapper.Map<List<FinishedGameToTable>>(games);

            var result = new PagedResult<FinishedGameToTable>(mappedGames,totalScores, scoreboardQuery.PageSize, scoreboardQuery.PageNumber);
           
            return result;
        }

        public async Task<PagedResult<FinishedGameToTable>> GetGameHistoryPage(ScoreboardQuery scoreboardQuery)
        {

            List<FinishedGame> games = await _gameHistoryRepository.GetGames(scoreboardQuery);
            int totalScores = (await _gameHistoryRepository.GetAllAsync()).Count();

            var result = new PagedResult<FinishedGame>(games, totalScores, scoreboardQuery.PageSize, scoreboardQuery.PageNumber);
            return _mapper.Map<PagedResult<FinishedGameToTable>>(result);

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
            else if (finishedGame.FinalScore < previousBest.FinalScore)
            {
                await _scoreboardRepository.DeleteAsync(previousBest.Id.ToString());
                await _scoreboardRepository.CreateAsync(finishedGame);
            }
        }

        public async Task<FinishedGameDto> GetGameResult(string gameId)
        {
            FinishedGame finishedGam = await _gameHistoryRepository.GetAsync(gameId);

            if(finishedGam is null )
            {
                throw new NotFoundException($"There is no game in history with id: {gameId}");
            }

            return _mapper.Map<FinishedGameDto>(finishedGam);
        }
    }
}
