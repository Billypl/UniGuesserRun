

using AutoMapper;
using Microsoft.Extensions.Options;
using PartyGame.Entities;
using PartyGame.Models.GameModels;
using PartyGame.Extensions.Exceptions;
using PartyGame.Models.ScoreboardModels;
using PartyGame.Models.AccountModels;
using PartyGame.Models.TokenModels;
using PartyGame.Models.PlaceModels;
using PartyGame.Services.StartGame;
using PartyGame.Settings;


namespace PartyGame.Services
{

    public interface IGameService
    {
        Task<string> StartNewGame(StartDataDto startDataDto);
        Task<RoundResultDto?> CheckGuess(Coordinates guessingCoordinates);
        Task<GuessingPlaceDto> GetPlaceToGuess(int roundsNumber);
        Task<FinishedGameDto> FinishGame();
     
    }

    public class GameService : IGameService
    {

        private readonly AuthenticationSettings _authenticationSettings;
        private readonly GameSettings _gameSettings;
        private readonly IGameStarter _gameStarter;
        private readonly IMapper _mapper;
        private readonly IGameSessionService _gameSessionService;
        private readonly IHttpContextAccessorService _httpContextAccessorService;

        

        public GameService(
            IOptions<AuthenticationSettings> authenticationSettings, 
            IOptions<GameSettings> gameSettings,
            IGameStarter gameStarter,
            IMapper mapper,
            IGameSessionService gameSessionService,
            IHttpContextAccessorService httpContextAccessorService )
        {
            _mapper = mapper;
            _gameSessionService = gameSessionService;
            _httpContextAccessorService = httpContextAccessorService;
            _authenticationSettings = authenticationSettings.Value;
            _gameSettings = gameSettings.Value;
            _gameStarter = gameStarter;
        }

        public async Task<string> StartNewGame(StartDataDto startDataDto)
        {
         return await _gameStarter.StartNewGame(startDataDto);
        }

        public async Task<GuessingPlaceDto> GetPlaceToGuess(int roundsNumber)
        {
            string id = _httpContextAccessorService.GetUserIdFromHeader();
            GameSession session = await _gameSessionService.GetSessionByGuid(id);

            if (session.ActualRoundNumber != roundsNumber)
            {
                throw new ForbidException(
                    $"The actual round number is ({session.ActualRoundNumber}) and getting other round number is not allowed");
            }

            var guessingPlace = session.Rounds[roundsNumber].PlaceToGuess;
           
            return _mapper.Map<GuessingPlaceDto>(guessingPlace);
        }

        public async Task<RoundResultDto?> CheckGuess(Coordinates guessingCoordinates)
        {
            var gameId = _httpContextAccessorService.GetUserIdFromHeader();
            GameSession session = await _gameSessionService.GetSessionByGuid(gameId);

            if (session.ActualRoundNumber >= _gameSettings.RoundsNumber)
            {
                throw new InvalidOperationException(
                    $"The actual round number ({session.ActualRoundNumber}) exceeds or equals the allowed number of rounds ({_gameSettings.RoundsNumber}).");
            }

            var guessingPlace = session.Rounds[session.ActualRoundNumber].PlaceToGuess;
            var distanceDifference = CalculateDistanceBetweenCords(new Coordinates 
            { Latitude = guessingPlace.Latitude, Longitude = guessingPlace.Longitude}, guessingCoordinates);

            var result = new RoundResultDto
            {
                DistanceDifference = distanceDifference,
                OriginalPlace = _mapper.Map<ShowPlaceDto>(guessingPlace),
                RoundNumber = session.ActualRoundNumber
            };

            session.Rounds[session.ActualRoundNumber].Score = distanceDifference;
            session.Rounds[session.ActualRoundNumber].Latitude = guessingCoordinates.Latitude;
            session.Rounds[session.ActualRoundNumber].Longitude = guessingCoordinates.Longitude;
            session.ActualRoundNumber++;
            session.GameScore += distanceDifference;

            await _gameSessionService.UpdateGameSession(session);

            return result;
        }
        private double CalculateDistanceBetweenCords(Coordinates first, Coordinates second)
        {
            const double EarthRadiusMeters = 6371000.0; 

            double ConvertToRadians(double degrees) => degrees * Math.PI / 180.0;

            double deltaLatitude = ConvertToRadians(second.Latitude - first.Latitude);
            double deltaLongitude = ConvertToRadians(second.Longitude - first.Longitude);

            double firstLatitudeRadians = ConvertToRadians(first.Latitude);
            double secondLatitudeRadians = ConvertToRadians(second.Latitude);

            double a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
                       Math.Cos(firstLatitudeRadians) * Math.Cos(secondLatitudeRadians) *
                       Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusMeters * c;

        }

        public async Task<FinishedGameDto> FinishGame()
        {
            string userGuid = _httpContextAccessorService.GetUserIdFromHeader();
            GameSession session = await _gameSessionService.GetSessionByGuid(userGuid);

            if (session.ActualRoundNumber != _gameSettings.RoundsNumber)
            {
                throw new Exception($"Game can be finished when you finished the game. {_gameSettings.RoundsNumber - session.ActualRoundNumber} rounds left");
            } 

            string tokenType = _httpContextAccessorService.GetTokenType();

            if (tokenType == "user")
            {
                await _gameSessionService.FinishGame(session.PublicId.ToString());
            }
            else
            {
                await _gameSessionService.DeleteSessionById(session.Id);
            }

            FinishedGameDto finishedGameDto = new FinishedGameDto()
            {
                Id = session.PublicId.ToString(),
                UserId = session.Player?.PublicId.ToString(),
                Nickname = session.Player?.Nickname,
                FinalScore = session.GameScore,
                Rounds = session.Rounds,
                Difficulty = session.Difficulty
            };

            return finishedGameDto;
        }

    }
}
