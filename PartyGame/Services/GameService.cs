

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
    }

    public class GameService : IGameService
    {
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly PlacesDbContext _dbContext;

        const int ROUNDS_NUMBER = 5; // TODO: Need to get this value from appsettgins.json

        public GameService(IOptions<AuthenticationSettings> authenticationSettings, IHttpContextAccessor httpContextAccessor
            , IMapper mapper,PlacesDbContext dbContext)
        {
            _authenticationSettings = authenticationSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _dbContext = dbContext;

        }

        private string GetTokenFromHeader()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();
            return token;
        }

        private async Task<List<int>> GetRandomIDsOfPlaces(int numberOfRoundsToTake)
        {
            var count = await _dbContext.Places.CountDocumentsAsync(_ => true);

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
                var place = await _dbContext.Places
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
            var place = GetRandomIDsOfPlaces(ROUNDS_NUMBER);
            
            var gameSession = new GameSession
            {
                Id = gameID,
                Token = token,
                IDsPlaces = place.Result,
                ActualRoundNumber = 0,
                ExpirationDate = DateTime.UtcNow.AddMinutes(15)

            };

            //GameSessions.Add(token,gameSession);
            _dbContext.GameSessions.InsertOneAsync(gameSession);

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
            var session = GetSessionByToken(token).Result;

            if (session.ActualRoundNumber != ROUNDS_NUMBER)
                throw new InvalidOperationException(
                    $"The actual round number is ({session.ActualRoundNumber}) and getting other round number is not allowed");

            var guessingPlace = GetPlaceById(session.IDsPlaces[roundsNumber]).Result;

            return _mapper.Map<GuessingPlaceDto>(guessingPlace);
        }

        public RoundResultDto? CheckGuess(Coordinates guessingCoordinates)
        {

            string token = GetTokenFromHeader();
            var session = GetSessionByToken(token).Result;

            if (session.ActualRoundNumber >= ROUNDS_NUMBER)
                throw new InvalidOperationException($"The actual round number ({session.ActualRoundNumber}) exceeds or equals the allowed number of rounds ({ROUNDS_NUMBER}).");

            var guessingPlace = GetPlaceById(session.IDsPlaces[session.ActualRoundNumber]).Result;
            var distanceDifference = CalculateDistanceBetweenCords(guessingPlace.Coordinates, guessingCoordinates);

            var result = new RoundResultDto
            {
                DistanceDifference = distanceDifference,
                OriginalPlace = guessingPlace,
                RoundNumber = session.ActualRoundNumber
            };

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

        public async Task<Place> GetPlaceById(int id)
        {
            var place = await _dbContext.Places
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (place == null)
            {
                throw new KeyNotFoundException($"Place with ID {id} was not found.");
            }

            return place;
        }

        public async Task<GameSession> GetSessionByToken(string token)
        {
            var gameSession = await _dbContext.GameSessions
                .Find(gs => gs.Token == token)
                .FirstOrDefaultAsync();
            if (gameSession == null)
            {
                throw new KeyNotFoundException($"GameSession with token {token} was not found.");
            }

            return gameSession;
        }



    }
}
