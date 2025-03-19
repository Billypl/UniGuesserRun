

using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using PartyGame.Entities;
using MongoDB.Driver;
using PartyGame.Models.GameModels;
using PartyGame.Extensions.Exceptions;
using PartyGame.Models.ScoreboardModels;



namespace PartyGame.Services
{

    public interface IGameService
    {
        Task<string> StartNewGameLogged(StartDataDto startData);
        Task<RoundResultDto?> CheckGuess(Coordinates guessingCoordinates);
        Task<GuessingPlaceDto> GetPlaceToGuess(int roundsNumber);
        Task<FinishedGameDto> FinishGame();
        Task<int> GetActualRoundNumber();
        Task<string> StartNewGameUnlogged(StartDataDto startData);
    }

    public class GameService : IGameService
    {
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IMapper _mapper;
        private readonly IPlaceService _placeService;
        private readonly IGameSessionService _gameSessionService;
        private readonly IGameTokenService _gameTokenService;
        private readonly IHttpContextAccessorService _httpContextAccessorService;

        private readonly IScoreboardService _scoreboardService;

        const int ROUNDS_NUMBER = 5; // TODO: Need to get this value from appsettgins.json
        private readonly DateTime GAME_SESSION_EXPIRATION;

        public GameService(IOptions<AuthenticationSettings> authenticationSettings
            , IMapper mapper,GameDbContext gameDbContext,IPlaceService placeService,
            IGameSessionService gameSessionService, IHttpContextAccessorService httpContextAccessorService,
            IAccountService accountService,IGameTokenService gameTokenService,
            IScoreboardService scoreboardService)
        {
            _authenticationSettings = authenticationSettings.Value;
            _mapper = mapper;
            _placeService = placeService;
            _gameSessionService = gameSessionService;
            _httpContextAccessorService = httpContextAccessorService;
            _gameTokenService = gameTokenService;
            _scoreboardService = scoreboardService;

            GAME_SESSION_EXPIRATION = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireGame);

        }

        public async Task<string> StartNewGameUnlogged(StartDataDto startDataDto)
        {
            if (startDataDto.Nickname is null)
            {
                throw new HttpRequestException("Null cannot be empty when you are not logged in");
            }

            DifficultyLevel difficulty =
              (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startDataDto.Difficulty, ignoreCase: true);

            var newGameToken = _gameTokenService.GenerateGuessSessionToken(startDataDto);

            // NOW ONLY WORKS FOR EASY DIFFICULTY
            List<Round> gameRounds = await GenerateRounds(difficulty);

            GameSession gameSession = GenerateGameSession(newGameToken, gameRounds, startDataDto.Nickname, difficulty);

            await _gameSessionService.AddNewGameSession(gameSession);
            return newGameToken;

        }

        public async Task<string> StartNewGameLogged(StartDataDto startData)
        {
          
            DifficultyLevel difficulty =
                (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startData.Difficulty, ignoreCase: true);

            string newGameToken = _httpContextAccessorService.GetTokenFromHeader();

            List<Round> gameRounds = await GenerateRounds(difficulty);

            string userNickname = _httpContextAccessorService.GetAuthenticatedUserProfile().Nickname;

            GameSession gameSession = GenerateGameSession(newGameToken, gameRounds, userNickname, difficulty);

            await _gameSessionService.AddNewGameSession(gameSession);
            return newGameToken;
        }

        private GameSession GenerateGameSession(string token,List<Round> rounds,
            string nickname,DifficultyLevel difficulty)
        {
            return new GameSession
            {
                Token = token,
                Rounds = rounds,
                ExpirationDate = GAME_SESSION_EXPIRATION,
                Nickname = nickname,
                DifficultyLevel = difficulty.ToString(),
            };
        }

        private async Task<List<Round>> GenerateRounds(DifficultyLevel difficulty)
        {
            List<ObjectId> places = await _placeService.GetRandomIDsOfPlaces(ROUNDS_NUMBER, difficulty);

            if (places.Count() < ROUNDS_NUMBER)
            {
                throw new InvalidOperationException("Not enough places was got from db.");
            }

            List<Round> gameRounds = new List<Round>();

            for (int i = 0; i < ROUNDS_NUMBER; i++)
            {
                var newRound = new Round
                {
                    IDPlaceToGuess = places[i],
                    GuessedCoordinates = new Coordinates(),
                    Score = 0
                };

                gameRounds.Add(newRound);
            }

            return gameRounds;
        }


        public async Task<GuessingPlaceDto> GetPlaceToGuess(int roundsNumber)
        {
            string token = _httpContextAccessorService.GetTokenFromHeader();
            var session = await _gameSessionService.GetSessionByToken(token);

            if (session.ActualRoundNumber != roundsNumber)
            {
                throw new ForbidException(
                    $"The actual round number is ({session.ActualRoundNumber}) and getting other round number is not allowed");
            }

            var guessingPlace = await _placeService.GetPlaceById(session.Rounds[roundsNumber].IDPlaceToGuess.ToString());
           
            return _mapper.Map<GuessingPlaceDto>(guessingPlace);
        }

        public async Task<RoundResultDto?> CheckGuess(Coordinates guessingCoordinates)
        {
            var token = _httpContextAccessorService.GetTokenFromHeader();
            var session = await _gameSessionService.GetSessionByToken(token);

            if (session.ActualRoundNumber >= ROUNDS_NUMBER)
            {
                throw new InvalidOperationException(
                    $"The actual round number ({session.ActualRoundNumber}) exceeds or equals the allowed number of rounds ({ROUNDS_NUMBER}).");
            }

            var guessingPlace =  await _placeService.GetPlaceById(session.Rounds[session.ActualRoundNumber].IDPlaceToGuess.ToString());
            var distanceDifference = CalculateDistanceBetweenCords(guessingPlace.Coordinates, guessingCoordinates);

            var result = new RoundResultDto
            {
                DistanceDifference = distanceDifference,
                OriginalPlace = guessingPlace,
                RoundNumber = session.ActualRoundNumber
            };

            session.Rounds[session.ActualRoundNumber].Score = distanceDifference;
            session.Rounds[session.ActualRoundNumber].GuessedCoordinates = guessingCoordinates;
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
            var token = _httpContextAccessorService.GetTokenFromHeader();
            GameSession session = await _gameSessionService.GetSessionByToken(token);

            if(session is null)
            {
                throw new NotFoundException("Game session with attached token doesnt exist");
            }

            //if(session.ActualRoundNumber != ROUNDS_NUMBER)
            //{ 
            //    throw new Exception($"Game can be finished when you finished the game. {ROUNDS_NUMBER - session.ActualRoundNumber} rounds left")
            //}

            FinishedGame finishedGame = _mapper.Map<FinishedGame>(session);
            finishedGame.UserId = ObjectId.Parse(_httpContextAccessorService.GetAuthenticatedUserProfile().UserId);

            await _scoreboardService.SaveGame(finishedGame);
            await _gameSessionService.DeleteSessionByToken(token);

            return _mapper.Map<FinishedGameDto>(finishedGame);
        }

        public async Task<int> GetActualRoundNumber()
        {
            string token = _httpContextAccessorService.GetTokenFromHeader();
            return (await _gameSessionService.GetSessionByToken(token)).ActualRoundNumber;
        }

    }
}
