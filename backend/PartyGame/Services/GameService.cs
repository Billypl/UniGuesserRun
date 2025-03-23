

using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using PartyGame.Entities;
using MongoDB.Driver;
using PartyGame.Models.GameModels;
using PartyGame.Extensions.Exceptions;
using PartyGame.Models.ScoreboardModels;
using PartyGame.Models.AccountModels;
using PartyGame.Models.TokenModels;



namespace PartyGame.Services
{

    public interface IGameService
    {
        Task<string> StartNewGame(StartDataDto startDataDto);
        Task<string> StartNewGameLogged(StartDataDto startData);
        Task<string> StartNewGameUnlogged(StartDataDto startData);
        Task<RoundResultDto?> CheckGuess(Coordinates guessingCoordinates);
        Task<GuessingPlaceDto> GetPlaceToGuess(int roundsNumber);
        Task<FinishedGameDto> FinishGame();
        Task<int> GetActualRoundNumber();
     
    }

    public class GameService : IGameService
    {
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IMapper _mapper;
        private readonly IPlaceService _placeService;
        private readonly IGameSessionService _gameSessionService;
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly ITokenService _accountTokenService;

        private readonly IScoreboardService _scoreboardService;

        const int ROUNDS_NUMBER = 5; // TODO: Need to get this value from appsettgins.json
        private readonly DateTime GAME_SESSION_EXPIRATION;

        public GameService(
            IOptions<AuthenticationSettings> authenticationSettings, 
            IMapper mapper,
            GameDbContext gameDbContext,
            IPlaceService placeService,
            IGameSessionService gameSessionService,
            IHttpContextAccessorService httpContextAccessorService,
            IAccountService accountService,
            IScoreboardService scoreboardService,
            ITokenService accountTokenService)
        {
            _authenticationSettings = authenticationSettings.Value;
            _mapper = mapper;
            _placeService = placeService;
            _gameSessionService = gameSessionService;
            _httpContextAccessorService = httpContextAccessorService;
            _scoreboardService = scoreboardService;
            _accountTokenService = accountTokenService;

            GAME_SESSION_EXPIRATION = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireGame);

        }

        public async Task<string> StartNewGame(StartDataDto startDataDto)
        {
            string? tokenType = _httpContextAccessorService.GetTokenTypeSafe();

            return tokenType == "user"
         ? await StartNewGameLogged(startDataDto)
         : await StartNewGameUnlogged(startDataDto);
        }

        public async Task<string> StartNewGameUnlogged(StartDataDto startDataDto)
        {
            if (startDataDto.Nickname is null)
            {
                throw new HttpRequestException("Nickname cannot be empty when you are not logged in (invalid token)");
            }
         
            DifficultyLevel difficulty =
              (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startDataDto.Difficulty, ignoreCase: true);

            ObjectId idForGuest = ObjectId.GenerateNewId();

            GuestTokenDataDto guestTokenData = new GuestTokenDataDto
            {
                GameSessionId = idForGuest.ToString(),
                Nickname = startDataDto.Nickname,
                Difficulty = startDataDto.Difficulty
            };

            string newGameToken = _accountTokenService.GenerateGuestToken(guestTokenData);

            List<Round> gameRounds = await GenerateRounds(difficulty);
            GameSession gameSession = GenerateGameSession(idForGuest, gameRounds, startDataDto.Nickname, difficulty);

            await _gameSessionService.AddNewGameSession(gameSession);
            return newGameToken;

        }

        public async Task<string> StartNewGameLogged(StartDataDto startData)
        {
          
            DifficultyLevel difficulty =
                (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startData.Difficulty, ignoreCase: true);

            List<Round> gameRounds = await GenerateRounds(difficulty);

            AccountDetailsFromTokenDto accountDetails = _httpContextAccessorService.GetAuthenticatedUserProfile();
            GameSession gameSession = GenerateGameSession(ObjectId.Parse(accountDetails.UserId), gameRounds, accountDetails.Nickname, difficulty);

            await _gameSessionService.AddNewGameSession(gameSession);
            return _httpContextAccessorService.GetTokenFromHeader();
        }

        private GameSession GenerateGameSession(ObjectId id,List<Round> rounds,
            string nickname,DifficultyLevel difficulty)
        {
            return new GameSession
            {
                Id = id,
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
                    IDPlaceToGuess = places[i].ToString(),
                    GuessedCoordinates = new Coordinates(),
                    Score = 0
                };

                gameRounds.Add(newRound);
            }

            return gameRounds;
        }


        public async Task<GuessingPlaceDto> GetPlaceToGuess(int roundsNumber)
        {
            string id = _httpContextAccessorService.GetGameSessionIdFromHeader();
            var session = await _gameSessionService.GetSessionById(id);

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
            var gameId = _httpContextAccessorService.GetGameSessionIdFromHeader();
            var session = await _gameSessionService.GetSessionById(gameId);

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
            var gameId = _httpContextAccessorService.GetGameSessionIdFromHeader();
            GameSession session = await _gameSessionService.GetSessionById(gameId);

            if (session.ActualRoundNumber != ROUNDS_NUMBER)
            {
                throw new Exception($"Game can be finished when you finished the game. {ROUNDS_NUMBER - session.ActualRoundNumber} rounds left");
            } 

            FinishedGame finishedGame = _mapper.Map<FinishedGame>(session);

            string tokenType = _httpContextAccessorService.GetTokenType();

            if (tokenType == "user")
            {
                finishedGame.UserId = ObjectId.Parse(_httpContextAccessorService.GetAuthenticatedUserProfile().UserId);
                await _scoreboardService.SaveGame(finishedGame);
            }            
            await _gameSessionService.DeleteSessionById(gameId);

            return _mapper.Map<FinishedGameDto>(finishedGame);
        }

        public async Task<int> GetActualRoundNumber()
        {
            string token = _httpContextAccessorService.GetTokenFromHeader();
            return (await _gameSessionService.GetSessionById(token)).ActualRoundNumber;
        }

    }
}
