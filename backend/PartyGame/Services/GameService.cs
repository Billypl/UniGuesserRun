

using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PartyGame.Entities;
using MongoDB.Driver;
using PartyGame.Models.GameModels;



namespace PartyGame.Services
{

    public interface IGameService
    {
        string StartNewGame(StartDataDto startData);
        RoundResultDto? CheckGuess(Coordinates guessingCoordinates);
        public GuessingPlaceDto GetPlaceToGuess(int roundsNumber);
        public SummarizeGameDto FinishGame();
        public void DeleteGame();
        public int GetActualRoundNumber();
    }

    public class GameService : IGameService
    {
        private readonly AuthenticationGameSettings _authenticationSettings;
        private readonly IMapper _mapper;

        private readonly IPlaceService _placeService;
        private readonly IGameSessionService _gameSessionService;

        const int ROUNDS_NUMBER = 5; // TODO: Need to get this value from appsettgins.json

        public GameService(IOptions<AuthenticationGameSettings> authenticationSettings
            , IMapper mapper,GameDbContext gameDbContext,IPlaceService placeService,
            IGameSessionService gameSessionService)
        {
            _authenticationSettings = authenticationSettings.Value;
            _mapper = mapper;
            _placeService = placeService;
            _gameSessionService = gameSessionService;

        }

        public string StartNewGame(StartDataDto startData)
        {
            DifficultyLevel difficulty =
                (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startData.Difficulty, ignoreCase: true);

            var token = GenerateSessionToken(startData);
            // NOW ONLY WORKS FOR EASY DIFFICULTY
            var places = _placeService.GetRandomIDsOfPlaces(ROUNDS_NUMBER, difficulty).Result;
            
            if (places.Count < ROUNDS_NUMBER)
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
                ExpirationDate = DateTime.UtcNow.AddMinutes(30),
                Nickname = startData.Nickname,
                DifficultyLevel = difficulty
            };

            _gameSessionService.AddNewGameSession(gameSession);
            return token;
        }

        private string GenerateSessionToken(StartDataDto startDataDto)
        {
            var expiration = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireMinutes);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, startDataDto.Nickname),
                new Claim("difficulty", startDataDto.Difficulty), 
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

        public GuessingPlaceDto GetPlaceToGuess(int roundsNumber)
        {
            var session = _gameSessionService.GetSessionByToken().Result;

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
            var session = _gameSessionService.GetSessionByToken().Result;

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

            _gameSessionService.UpdateGameSession(session);

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
            var session = _gameSessionService.GetSessionByToken().Result;
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
        public void DeleteGame()
        {
            _gameSessionService.DeleteSessionByToken();
        }

        public int GetActualRoundNumber()
        {
            return _gameSessionService.GetSessionByToken().Result.ActualRoundNumber;
        }
    }
}
