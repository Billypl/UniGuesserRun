using AutoMapper;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models;
using UniGuesser.Application.Models.Enumerations;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Models.ScoreboardModels;
using UniGuesser.Domain.Middleware.Exceptions;

namespace UniGuesser.Domain.Services
{
    public interface IGameSessionService
    {
        Task DeleteSessionById(int id);
        Task DeleteSessionByGuid(string guid);
        Task<GameSession> GetSessionById(int id);
        Task<GameSession> GetSessionByGuid(string guid);
        Task UpdateGameSession(GameSession session);
        Task AddNewGameSession(GameSession session);
        Task<bool> HasActiveGameSession(string guid);
        Task<GameSessionStateDto> GetActualGameStateByHeader();
        Task<GameSessionStateDto> GetActualGameState(string guid);
        Task SetGameStatus(GameSession gameSession, GameStatus status);
        Task<FinishedGameDto> GetFinishedGame(string guid);
        Task<PagedResult<FinishedGameDto>> GetGameHistoryPage(ScoreboardQuery scoreboardQuery);
        Task<PagedResult<UserStats>> GetPagedUserStatsResult(ScoreboardQuery scoreboardQuery);
        Task<PagedResult<FinishedGameDto>> GetGameHistoryPageByUser(UserHistoryQuery userHistoryQuery, string userGuid);
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
                throw new GameSessionExceptions.GameNotFoundException(id);
            }
        }

        public async Task DeleteSessionByGuid(string guid)
        {

            GameSession? session = await _gameSessionRepository.GetByPublicIdAsync(guid);

            if (session is null)
            {
                throw new GameSessionExceptions.GameNotFoundException(guid); ;
            }

            await DeleteSessionById(session.Id);
        }

        public async Task<GameSession> GetSessionById(int id)
        {
            GameSession? gameSession = await _gameSessionRepository.GetAsync(id);

            if (gameSession is null)
            {
                throw new GameSessionExceptions.GameNotFoundException(id);
            }
            return gameSession;
        }

        public async Task<GameSession> GetSessionByGuid(string guid)
        {
            GameSession? gameSession = await _gameSessionRepository.GetByPublicIdAsync(guid);

            if(gameSession is null)
            {
                throw new GameSessionExceptions.GameNotFoundException(guid);
            }

            return gameSession;
        }

        public async Task UpdateGameSession(GameSession session)
        {
            var existingSession = await _gameSessionRepository.GetAsync(session.Id);

            if (existingSession == null)
            {
                throw new GameSessionExceptions.GameNotFoundException(session.Id);
            }

            await _gameSessionRepository.UpdateAsync(session);
        }

        public async Task AddNewGameSession(GameSession session)
        {
            GameSession? existedSession = await _gameSessionRepository.GetAsync(session.Id);
            if (existedSession != null)
            {
                throw new GameSessionExceptions.GameAlreadyExistsException();
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

        public async Task<GameSessionStateDto> GetActualGameStateByHeader()
        {
            string gameSessionId = _httpContextAccessorService.GetUserIdFromHeader();
            GameSession? session = await _gameSessionRepository.GetActiveGameSessionByPlayerId(gameSessionId);

            if (session is null)
            {
                throw new GameSessionExceptions.GameNotFoundException(gameSessionId);
            }

            return _mapper.Map<GameSessionStateDto>(session);
        }

        public async Task<GameSessionStateDto> GetActualGameState(string guid)
        {
            GameSession? session = await _gameSessionRepository.GetActiveGameSession(guid);

            if (session is null)
            {
                throw new GameSessionExceptions.GameNotFoundException(guid);
            }

            return _mapper.Map<GameSessionStateDto>(session);
        }

        public async Task SetGameStatus(GameSession gameSession, GameStatus status)
        {
            gameSession.GameState = status;
            await _gameSessionRepository.UpdateAsync(gameSession);
        }

        public async Task<FinishedGameDto> GetFinishedGame(string guid)
        {
            GameSession? gameSession = await _gameSessionRepository.GetByPublicIdAsync(guid);
        
            if(gameSession is null)
            {
                throw new GameSessionExceptions.GameNotFoundException(guid);
            }
        
            if(gameSession.GameState == GameStatus.InProgress)
            {
                throw new GameSessionExceptions.GameNotFinishedException(guid);
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

       

        public async Task<PagedResult<FinishedGameDto>> GetGameHistoryPageByUser(UserHistoryQuery userHistoryQuery, string userGuid)
        {
          var games  = await _gameSessionRepository.GetGameUserHistoryGames(userHistoryQuery, userGuid);

          var pagedGames = games
            .Skip((userHistoryQuery.PageNumber - 1) * userHistoryQuery.PageSize)
            .Take(userHistoryQuery.PageSize)
            .ToList();

          var mappedGames = _mapper.Map<List<FinishedGameDto>>(pagedGames);

          var result = new PagedResult<FinishedGameDto>(mappedGames, games.Count(), userHistoryQuery.PageSize, userHistoryQuery.PageNumber);

          return result;
        }
    }
}

