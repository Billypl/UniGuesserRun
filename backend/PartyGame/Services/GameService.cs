

using AutoMapper;
using Microsoft.Extensions.Options;
using PartyGame.Entities;
using PartyGame.Models.GameModels;
using PartyGame.Extensions.Exceptions;
using PartyGame.Models.ScoreboardModels;
using PartyGame.Models.AccountModels;
using PartyGame.Models.TokenModels;
using PartyGame.Models.PlaceModels;



namespace PartyGame.Services
{

    public interface IGameService
    {
        string StartNewGame(StartDataDto startDataDto);
        Task<string> StartNewGameLogged(StartDataDto startData);
        Task<string> StartNewGameUnlogged(StartDataDto startData);
        Task<RoundResultDto?> CheckGuess(Coordinates guessingCoordinates);
        Task<GuessingPlaceDto> GetPlaceToGuess(int roundsNumber);
        Task<FinishedGameDto> FinishGame();
     
    }

    public class GameService : IGameService
    {
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IMapper _mapper;
        private readonly IPlaceService _placeService;
        private readonly IGameSessionService _gameSessionService;
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly ITokenService _accountTokenService;
        private readonly IAccountService _accountService;
        private readonly IRoundService _roundService;

        const int ROUNDS_NUMBER = 5; // TODO: Need to get this value from appsettgins.json
        private readonly DateTime GAME_SESSION_EXPIRATION;

        public GameService(
            IOptions<AuthenticationSettings> authenticationSettings, 
            IMapper mapper,
            IPlaceService placeService,
            IGameSessionService gameSessionService,
            IHttpContextAccessorService httpContextAccessorService,
            IAccountService accountService,
            ITokenService accountTokenService,
            IRoundService roundService)
        {
            _authenticationSettings = authenticationSettings.Value;
            _mapper = mapper;
            _placeService = placeService;
            _gameSessionService = gameSessionService;
            _httpContextAccessorService = httpContextAccessorService;
            _accountTokenService = accountTokenService;
            _accountService = accountService;
            _roundService = roundService;

            GAME_SESSION_EXPIRATION = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireGame);

        }

        public string StartNewGame(StartDataDto startDataDto)
        { 
            string? tokenType = _httpContextAccessorService.GetTokenTypeSafe();
            string? playerGuid = _httpContextAccessorService.GetUserIdFromHeaderSafe();

            if (playerGuid is not null && _gameSessionService.HasActiveGameSession(playerGuid).Result)
            {
                throw new InvalidOperationException($"Game for id:{playerGuid} already exists");
            }
            
            return tokenType == "user"
         ? StartNewGameLogged(startDataDto).Result
         : StartNewGameUnlogged(startDataDto).Result;
        }

        public async Task<string> StartNewGameUnlogged(StartDataDto startDataDto)
        {
            if (startDataDto.Nickname is null)
            {
                throw new HttpRequestException("Nickname cannot be empty when you are not logged in (invalid token)");
            }
         
            DifficultyLevel difficulty =
              (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startDataDto.Difficulty, ignoreCase: true);

            Guid GuestGuid = Guid.NewGuid();
          
            GuestTokenDataDto guestTokenData = new GuestTokenDataDto
            {
                Nickname = startDataDto.Nickname,
                Difficulty = startDataDto.Difficulty,
                GameSessionId = GuestGuid.ToString(),
            };

            string newGameToken = _accountTokenService.GenerateGuestToken(guestTokenData);

            List<Round> gameRounds = await GenerateRounds(difficulty);
            GameSession gameSession = new GameSession
            {
                PublicId = GuestGuid,
                Rounds = gameRounds,
                ExpirationDate = GAME_SESSION_EXPIRATION,
                Difficulty = difficulty.ToString(),
            };

            foreach (Round gameRound in gameRounds)
            {
                gameRound.GameSession = gameSession;
            }

            await _gameSessionService.AddNewGameSession(gameSession);
            return newGameToken;

        }

        public async Task<string> StartNewGameLogged(StartDataDto startData)
        {
          
            DifficultyLevel difficulty =
                (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startData.Difficulty, ignoreCase: true);

            List<Round> gameRounds = await GenerateRounds(difficulty);

            AccountDetailsFromTokenDto accountDetails = _httpContextAccessorService.GetAuthenticatedUserProfile();

            var user = (await _accountService.GetAccountDetailsByPublicId(accountDetails.UserId));

            GameSession gameSession = new GameSession
            {
                PublicId = Guid.Parse(accountDetails.UserId),
                Rounds = gameRounds,
                ExpirationDate = GAME_SESSION_EXPIRATION,
                UserId = user.Id,
                Player = user,
                Difficulty = difficulty.ToString(),
            };

            foreach (Round gameRound in gameRounds)
            {
                gameRound.GameSession = gameSession;
            }

            await _gameSessionService.AddNewGameSession(gameSession);
            return _httpContextAccessorService.GetTokenFromHeader();
        }

        private async Task<List<Round>> GenerateRounds(DifficultyLevel difficulty)
        {
            List<Place> places = await _placeService.GetRandomPlaces(ROUNDS_NUMBER, difficulty);

            if (places.Count() < ROUNDS_NUMBER)
            {
                throw new InvalidOperationException("Not enough places was got from db.");
            }

            List<Round> gameRounds = new List<Round>();

            for (int i = 0; i < ROUNDS_NUMBER; i++)
            {
                var newRound = new Round
                {
                    PlaceId = places[i].Id,
                    PlaceToGuess = places[i],
                    Score = 0
                };

                gameRounds.Add(newRound);
            }

            return gameRounds;
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

            if (session.ActualRoundNumber >= ROUNDS_NUMBER)
            {
                throw new InvalidOperationException(
                    $"The actual round number ({session.ActualRoundNumber}) exceeds or equals the allowed number of rounds ({ROUNDS_NUMBER}).");
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
            string UserGuid = _httpContextAccessorService.GetUserIdFromHeader();
            GameSession session = await _gameSessionService.GetSessionByGuid(UserGuid);

            if (session.ActualRoundNumber != ROUNDS_NUMBER)
            {
                throw new Exception($"Game can be finished when you finished the game. {ROUNDS_NUMBER - session.ActualRoundNumber} rounds left");
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
