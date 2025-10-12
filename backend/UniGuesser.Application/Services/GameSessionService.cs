using AutoMapper;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Models.PlaceModels;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.ValueObjects;
using UniGuesser.Domain.ValueObjects.Enumerations;
using UniGuesser.Infrastructure;

namespace UniGuesser.Application.Services
{
    public interface IGameSessionService
    {
        Task DeleteSessionById(int id);
        Task<GameSession> GetSessionByGuid(Guid guid);
        Task UpdateGameSession(GameSession session);
        Task AddNewGameSession(GameSession session);
        Task<bool> HasActiveGameSession(Guid guid);
        Task SetGameStatus(GameSession gameSession, GameStatus status);
        Task<bool> HasActiveGameSession(Guid? playerGuid);
        Task<RoundResultDto> CheckGuess(Guid sessionId, Coordinates guess, int totalRounds);
    }

    public class GameSessionService : IGameSessionService
    {
        private readonly IGameSessionRepository _gameSessionRepository;
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly IMapper _mapper;

        public GameSessionService(IGameSessionRepository gameSessionRepository,
            IHttpContextAccessorService httpContextAccessorService,
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

        public async Task<bool> HasActiveGameSession(Guid guid)
        {
            var existingSession = await _gameSessionRepository.GetActiveGameSessionByPlayerId(guid);

            if (existingSession is null)
            {
                return false;
            }

            return true;
        }

        public async Task SetGameStatus(GameSession gameSession, GameStatus status)
        {
            gameSession.GameState = status;
            await _gameSessionRepository.UpdateAsync(gameSession);
        }

        public async Task<GameSession> GetSessionByGuid(Guid guid)
        {
            var session = await _gameSessionRepository.GetActiveGameSession(guid);
            if (session is null)
            {
                throw new GameSessionExceptions.GameNotFoundException(guid);
            }

            return session;
        }

        public async Task<bool> HasActiveGameSession(Guid? playerGuid)
        {
            if (playerGuid == null)
            {
                return false;
            }

            var existingSession = await _gameSessionRepository.GetActiveGameSessionByPlayerId(playerGuid.Value);
            if (existingSession is null)
            {
                return false;
            }

            return true;


        }

        public async Task<RoundResultDto> CheckGuess(Guid sessionId, Coordinates guess, int totalRounds)
        {
            var session = await _gameSessionRepository.GetActiveGameSession(sessionId);
            var distance = session.CheckGuess(guess, totalRounds);

            return new RoundResultDto
            {
                DistanceDifference = distance,
                RoundNumber = session.ActualRoundNumber - 1,
                OriginalPlace = _mapper.Map<ShowPlaceDto>(session.Rounds[session.ActualRoundNumber - 1].PlaceToGuess)
            };
        }
    }
}