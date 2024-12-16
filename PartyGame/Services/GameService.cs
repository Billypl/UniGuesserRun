using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PartyGame.Entities;
using PartyGame.Models;

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
        private static readonly Dictionary<string, GameSession> GameSessions = new();
        private readonly List<Place> _places;

        public GameService(IOptions<AuthenticationSettings> authenticationSettings, IMapper mapper)
        {
            _authenticationSettings = authenticationSettings.Value;
            _mapper = mapper;
            _places = new List<Place>
            {
                new Place
                {
                    Id = 1,
                    Name = "Wydział Elektroniki, Telekomunikacji i Informatyki",
                    Description = "Wydział Elektroniki, Telekomunikacji i Informatyki (ETI) Politechniki Gdańskiej " +
                                  "(PG) jest jednym z czołowych wydziałów uczelni, oferującym kształcenie na kierunkach ",
                    Coordinates = new Coordinates { Latitude = 54.37170, Longitude = 18.61242 },
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/4/41/FOT_1171-ETI.jpg"
                },
                new Place
                { 
                    Id = 2,
                    Name = "Gmach Główny",
                    Description = "Zabytkowy budynek w Gdańsku. Mieści się we Wrzeszczu przy ul. Narutowicza 11/12. ",
                    Coordinates = new Coordinates { Latitude = 54.371437068881185, Longitude = 18.619219721970538 },
                    ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/76/Politechnika_gdanska_2012.tif/lossy-page1-800px-Politechnika_gdanska_2012.tif.jpg"
                },
                new Place
                { 
                    Id = 3,
                    Name = "Centrum Sportu Akademickiego",
                    Description = "Centralnym obiektem jest zbudowany w 1962 roku kompleks Akademickiego Ośrodka Sportowego PG.",
                    Coordinates = new Coordinates { Latitude = 54.3693814546564, Longitude = 18.6313515818974 },
                    ImageUrl = "https://pg.edu.pl/files/csa/styles/large/public/2021-07/DSC_0564.jpg?itok=bQFL7JLl"
                }
            };
        }


        private Place GetRandomPlace()
        {
            var random = new Random();
            int index = random.Next(_places.Count);
            return _places[index];
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
                Place = place
            };

            GameSessions[token] = gameSession;

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
                audience: "GameClient",
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public GuessResultDto? CheckGuess(GuessDataDto guessData)
        {
            if (GameSessions.ContainsKey(guessData.Token))
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

            return null;
        }
    }
}
