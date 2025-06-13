

using AutoMapper;
using Microsoft.Extensions.Options;
using PartyGame.Models.GameModels;
using PartyGame.Models.ScoreboardModels;
using PartyGame.Services.GameServices.GameStartStrategies;


namespace PartyGame.Services.GameServices
{

    public interface IGameService
    {
        Task<StartedGameData> StartNewGame(StartDataDto startDataDto);
        Task<RoundResultDto?> CheckGuess(string gameGuid, Coordinates guessingCoordinates);
        Task<GuessingPlaceDto> GetPlaceToGuess(string gameGuid, int roundsNumber);
        Task<FinishedGameDto> FinishGame(string gameGuid);

    }
    
    public class GameService : IGameService
    {

        private readonly GameSettings _gameSettings;
        private readonly IGameStarter _gameStarter;
        private readonly IMapper _mapper;
        private readonly IGameSessionService _gameSessionService;
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        public GameService(
            IOptions<GameSettings> gameSettings,
            IGameStarter gameStarter,
            IMapper mapper,
            IGameSessionService gameSessionService,
            IHttpContextAccessorService httpContextAccessorService)
        {
            _mapper = mapper;
            _gameSessionService = gameSessionService;
            _httpContextAccessorService = httpContextAccessorService;
            _gameSettings = gameSettings.Value;
            _gameStarter = gameStarter;
        }

        public async Task<StartedGameData> StartNewGame(StartDataDto startDataDto)
        {
            return await _gameStarter.StartNewGame(startDataDto);
        }

        public async Task<GuessingPlaceDto> GetPlaceToGuess(string gameGuid, int roundsNumber)
        {
            GameSession session = await _gameSessionService.GetSessionByGuid(gameGuid);

            var guessingPlace = session.GetRoundOrThrow(roundsNumber).PlaceToGuess;

            return _mapper.Map<GuessingPlaceDto>(guessingPlace);
        }

        public async Task<RoundResultDto?> CheckGuess(string gameGuid, Coordinates guessingCoordinates)
        {
            GameSession session = await _gameSessionService.GetSessionByGuid(gameGuid);

            var result = session.CheckGuess(guessingCoordinates, _mapper, _gameSettings.RoundsNumber);
            await _gameSessionService.UpdateGameSession(session);
            return result;
        }

        public async Task<FinishedGameDto> FinishGame(string gameGuid)
        {

            GameSession session = await _gameSessionService.GetSessionByGuid(gameGuid);

            session.EnsureGameFinished(_gameSettings.RoundsNumber);

            string tokenType = _httpContextAccessorService.GetTokenType();

            if (tokenType == "user")
            {
                await _gameSessionService.FinishGame(session.PublicId.ToString());
            }
            else
            {
                await _gameSessionService.DeleteSessionById(session.Id);
            }

            FinishedGameDto finishedGameDto = _mapper.Map<FinishedGameDto>(session);

            return finishedGameDto;
        }

    }
}
