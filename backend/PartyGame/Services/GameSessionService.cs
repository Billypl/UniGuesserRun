using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using PartyGame.Entities;
using PartyGame.Extensions.Exceptions;
using PartyGame.Models;
using PartyGame.Models.GameModels;
using PartyGame.Models.ScoreboardModels;
using PartyGame.Repositories;

namespace PartyGame.Services
{
    public interface IGameSessionService
    {
        Task DeleteSessionById(int id);
        Task DeleteSessionByHeader();
        Task<GameSession> GetSessionById(int id);
        Task<GameSession> GetSessionByGuid(string guid);
        Task UpdateGameSession(GameSession session);
        Task AddNewGameSession(GameSession session);
        Task<bool> HasActiveGameSession(string guid);
        Task<GameSessionStateDto> GetActualGameState();
        Task<GameSessionStateDto> GetActualGameState(string guid);
        Task FinishGame(string guid);
        Task<FinishedGameDto> GetFinishedGame(string guid);
        Task<PagedResult<FinishedGameDto>> GetGameHistoryPage(ScoreboardQuery scoreboardQuery);
        Task<PagedResult<UserStats>> GetPagedUserStatsResult(ScoreboardQuery scoreboardQuery);

    }

    public class GameSessionService : IGameSessionService
    {
        private readonly IGameSessionRepository _gameSessionRepository;
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly IMapper _mapper;

        public GameSessionService(IGameSessionRepository gameSessionRepository,IHttpContextAccessorService httpContextAccessorService,
            IMapper mapper)
        {
            _gameSessionRepository = gameSessionRepository;
            _httpContextAccessorService = httpContextAccessorService;
            _mapper = mapper;
        }


        public async Task DeleteSessionById(int id)
        {
            var deleteResult = await _gameSessionRepository.DeleteAsync(id);
            if (deleteResult == false)
            {
                throw new NotFoundException($"GameSession with id {id} was not found.");
            }
        }

        public async Task DeleteSessionByHeader()
        {
            string guid = _httpContextAccessorService.GetUserIdFromHeader();

            GameSession? session = await _gameSessionRepository.GetByPublicIdAsync(guid);

            if (session is null)
            {
                throw new NotFoundException($"GameSession with id {guid} was not found.");
            }

            await DeleteSessionById(session.Id);
        }

        public async Task<GameSession> GetSessionById(int id)
        {
            GameSession? gameSession = await _gameSessionRepository.GetAsync(id);

            if (gameSession is null)
            {
                throw new KeyNotFoundException($"Game session with id {id} was not found");
            }
            return gameSession;
        }

        public async Task<GameSession> GetSessionByGuid(string guid)
        {
            GameSession? gameSession = await _gameSessionRepository.GetByPublicIdAsync(guid);

            if(gameSession is null)
            {
                throw new KeyNotFoundException($"Game session with id {guid} was not found");
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
            GameSession? existedSession = await _gameSessionRepository.GetAsync(session.Id);
            if (existedSession != null)
            {
                throw new InvalidOperationException("A session with the same token already exists.");
            }
            await _gameSessionRepository.CreateAsync(session);
        }

        public async Task<bool> HasActiveGameSession(string guid)
        {
            var existingSession = await _gameSessionRepository.GetActiveGameSessionByPlayerId(guid);

            if (existingSession is null)
            {
                return false;
            }

            return true;
        }

        public async Task<GameSessionStateDto> GetActualGameState()
        {
            string gameSessionId = _httpContextAccessorService.GetUserIdFromHeader();
            GameSession? session = await _gameSessionRepository.GetActiveGameSessionByPlayerId(gameSessionId);

            if (session is null)
            {
                throw new KeyNotFoundException($"GameSession with ID {session.Id} was not found.");
            }

            return _mapper.Map<GameSessionStateDto>(session);
        }

        public async Task<GameSessionStateDto> GetActualGameState(string guid)
        {
            GameSession? session = await _gameSessionRepository.GetActiveGameSession(guid);

            if (session is null)
            {
                throw new KeyNotFoundException($"GameSession with ID {session.Id} was not found.");
            }

            return _mapper.Map<GameSessionStateDto>(session);
        }

        public async Task FinishGame(string guid)
        {
            GameSession? gameSession = await _gameSessionRepository.GetActiveGameSession(guid);
            gameSession.IsFinished = true;
            gameSession.PublicId = Guid.NewGuid();

            await _gameSessionRepository.UpdateAsync(gameSession);
        }

        public async Task<FinishedGameDto> GetFinishedGame(string guid)
        {
            GameSession? gameSession = await _gameSessionRepository.GetByPublicIdAsync(guid);
        
            if(gameSession is null)
            {
                throw new NotFoundException($"Game with id ${guid} does not exist");
            }
        
            if(gameSession.IsFinished == false)
            {
                throw new Exception($"Game is not finished and cannot be showed");
            }
            return _mapper.Map<FinishedGameDto>(gameSession);     
        }

        public async Task<PagedResult<FinishedGameDto>> GetGameHistoryPage(ScoreboardQuery scoreboardQuery)
        {
            var games = await _gameSessionRepository.GetGameHistoryPage(scoreboardQuery);

            var pagedGames = games
            .Skip((scoreboardQuery.PageNumber - 1) * scoreboardQuery.PageSize)
            .Take(scoreboardQuery.PageSize)
            .ToList();

            var mappedGames = _mapper.Map<List<FinishedGameDto>>(pagedGames);

            var result = new PagedResult<FinishedGameDto>(mappedGames, games.Count(), scoreboardQuery.PageSize, scoreboardQuery.PageNumber);

            return result;
        }

        public async Task<PagedResult<UserStats>> GetPagedUserStatsResult(ScoreboardQuery scoreboardQuery)
        {
          var games = await _gameSessionRepository.GetUsersStats(scoreboardQuery);

          var pagedGames = games
            .Skip((scoreboardQuery.PageNumber - 1) * scoreboardQuery.PageSize)
            .Take(scoreboardQuery.PageSize)
            .ToList();

            var result = new PagedResult<UserStats>(pagedGames, games.Count(), scoreboardQuery.PageSize, scoreboardQuery.PageNumber);
          return result;
        }


    }
}

