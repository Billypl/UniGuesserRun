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
        StartDataDto StartNewGame();
        string GenerateSessionToken(long gameId);
        GuessResultDto? CheckGuess(GuessDataDto guessData);
    }

    public class GameService : IGameService
    {
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IMapper _mapper;
        private readonly PlacesDbContext _dbContext;
        private static readonly Dictionary<string, GameSession> GameSessions = new();

        public GameService(IOptions<AuthenticationSettings> authenticationSettings, IMapper mapper,PlacesDbContext dbContext)
        {
            _authenticationSettings = authenticationSettings.Value;
            _mapper = mapper;
            _dbContext = dbContext;

        }


        private async Task<Place> GetRandomPlace()
        {
            var count = await _dbContext.Places.CountDocumentsAsync(_ => true);

            if (count == 0)
                return null;

            var randomIndex = new Random().Next(0, (int)count);

            var randomPlace = await _dbContext.Places
                .Find(_ => true)
                .Skip(randomIndex)
                .Limit(1)
                .FirstOrDefaultAsync();

            return randomPlace;
        }

        public StartDataDto StartNewGame()
        {
            // gameID unikalne ID dla danej gry
            // Przydane jesli bedziemy miec historie gier

            var gameID = new Random().Next(1,10000);
            var token = GenerateSessionToken(gameID);
            var place = GetRandomPlace();
          

            var gameSession = new GameSession
            {
                Id = gameID,
                Token = token,
                Place = place.Result
            };

            GameSessions.Add(token,gameSession);

            return _mapper.Map<StartDataDto>(gameSession);
        }

        public string GenerateSessionToken(long gameId)
        {
            var expiration = DateTime.UtcNow.AddDays(_authenticationSettings.JwtExpireDays);
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

       
        public GuessResultDto? CheckGuess(GuessDataDto guessData)
        {
            
                var session = GameSessions[guessData.Token];
                var placeCoordinates = session.Place.Coordinates;

                var latitudeDifference = Math.Abs(placeCoordinates.Latitude - guessData.Coordinates.Latitude);
                var longitudeDifference = Math.Abs(placeCoordinates.Longitude - guessData.Coordinates.Longitude);

                var result = new GuessResultDto
                {
                    DistanceDifference = new Coordinates
                    {
                        Latitude = latitudeDifference,
                        Longitude = longitudeDifference
                    },
                    OriginalPlace = session.Place
                };

                GameSessions.Remove(guessData.Token);
                return result;
                
        }
    }
}
