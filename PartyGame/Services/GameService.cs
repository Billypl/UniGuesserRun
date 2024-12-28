

using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PartyGame.Entities;
using PartyGame.Models;
using MongoDB.Driver;



namespace PartyGame.Services
{
  
    public interface IGameService
    {
        string StartNewGame();
        string GenerateSessionToken(long gameId);
        RoundResultDto? CheckGuess(Coordinates guessingCoordinates);

        public GuessingPlaceDto GetPlaceToGuess(int roundsNumber);

        public SummarizeGameDto FinishGame();
    }

    public class GameService : IGameService
    {
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly GameDbContext _gameDbContext;

        private readonly IPlaceService _placeService;
        private readonly ISessionService _sessionService;

        const int ROUNDS_NUMBER = 5; // TODO: Need to get this value from appsettgins.json

        public GameService(IOptions<AuthenticationSettings> authenticationSettings, IHttpContextAccessor httpContextAccessor
            , IMapper mapper,GameDbContext gameDbContext,IPlaceService placeService,
            ISessionService sessionService)
        {
            _authenticationSettings = authenticationSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _gameDbContext = gameDbContext;
            _placeService = placeService;
            _sessionService = sessionService;

        }

        private string GetTokenFromHeader()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (token == null)
            {
                throw new KeyNotFoundException("Token was not found in db");
            }

            return token;
        }

        private async Task<List<int>> GetRandomIDsOfPlaces(int numberOfRoundsToTake)
        {
            var count = await _gameDbContext.Places.CountDocumentsAsync(_ => true);

            if (count == 0)
                return null;

            numberOfRoundsToTake = Math.Min(numberOfRoundsToTake, (int)count);

            var randomIndexes = new HashSet<int>();
            var random = new Random();
            while (randomIndexes.Count < numberOfRoundsToTake)
            {
                randomIndexes.Add(random.Next(0, (int)count));
            }

            var randomPlaces = new List<int>();
            foreach (var index in randomIndexes)
            {
                var place = await _gameDbContext.Places
                    .Find(_ => true)
                    .Skip(index)
                    .Limit(1)
                    .FirstOrDefaultAsync();

                if (place != null)
                {
                    randomPlaces.Add(place.Id);
                }
            }

            return randomPlaces;
        }


        public string StartNewGame()
        {
            var gameID = new Random().Next(1,100000);
            var token = GenerateSessionToken(gameID);
            var places = GetRandomIDsOfPlaces(ROUNDS_NUMBER).Result;

            if (places == null || places.Count < ROUNDS_NUMBER)
            {
                throw new InvalidOperationException("Not enough places was got from db.");
            }

            List<Round> GameRounds = new List<Round>();

            for (int i = 0; i < ROUNDS_NUMBER; i++)
            {
                var newRound = new Round
                {
                    IDPlaceToGuess = places[i],
                    GuessedCoordinates = new Coordinates(),
                    Score = 0
                };

                GameRounds.Add(newRound);
            }
            var gameSession = new GameSession
            {
                Token = token,
                Rounds = GameRounds,
                ActualRoundNumber = 0,
                ExpirationDate = DateTime.UtcNow.AddMinutes(30)

            };

            _gameDbContext.GameSessions.InsertOneAsync(gameSession);

            return token;
        }

        public string GenerateSessionToken(long gameId)
        {
            var expiration = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireMinutes);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, gameId.ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expiration).ToUnixTimeSeconds().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _authenticationSettings.JwtIssuer,
                audience: _authenticationSettings.JwtIssuer,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // TODO: DRY - 2 similiar instructions in below functions 
        public GuessingPlaceDto GetPlaceToGuess(int roundsNumber)
        {
            string token = GetTokenFromHeader();
            var session = _sessionService.GetSessionByToken(token).Result;

            if (session.ActualRoundNumber != roundsNumber)
            {
                throw new InvalidOperationException(
                    $"The actual round number is ({session.ActualRoundNumber}) and getting other round number is not allowed");
            }

            var guessingPlace = _placeService.GetPlaceById(session.Rounds[roundsNumber].IDPlaceToGuess).Result;

            return _mapper.Map<GuessingPlaceDto>(guessingPlace);
        }

        public RoundResultDto? CheckGuess(Coordinates guessingCoordinates)
        {

            string token = GetTokenFromHeader();
            var session = _sessionService.GetSessionByToken(token).Result;

            if (session.ActualRoundNumber >= ROUNDS_NUMBER)
            {
                throw new InvalidOperationException(
                    $"The actual round number ({session.ActualRoundNumber}) exceeds or equals the allowed number of rounds ({ROUNDS_NUMBER}).");
            }

            var guessingPlace = _placeService.GetPlaceById(session.Rounds[session.ActualRoundNumber].IDPlaceToGuess).Result;
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

            _sessionService.UpdateSessionRound(session);

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


        public SummarizeGameDto FinishGame()
        {
            var token = GetTokenFromHeader();
            var session = _sessionService.GetSessionByToken(token).Result;

            var summarize = CreateSummarize(session);


            return summarize;
        }

        private SummarizeGameDto CreateSummarize(GameSession session)
        {
            SummarizeGameDto summarize = new SummarizeGameDto();

            summarize.Rounds = session.Rounds;
            summarize.Score = session.GameScore;

            return summarize;
        }



    }
}
