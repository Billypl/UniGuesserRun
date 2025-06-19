using AutoMapper;
using PartyGame.Extensions;
using PartyGame.Models;
using PartyGame.Models.GameModels;

namespace PartyGame.Services.GameServices
{
    public interface IGameGeolocationService
    {
       Task<double> GetDistanceFromPlace(Coordinates actualPosition);
    }

    public class GameGeolocationService : IGameGeolocationService
    {

        private readonly IMapper _mapper;
        private readonly IGameSessionService _gameSessionService;
        private readonly IHttpContextAccessorService _httpContextAccessorService;

        public GameGeolocationService(
            IMapper mapper,
            IGameSessionService gameSessionService,
            IHttpContextAccessorService httpContextAccessorService
        )
        {
            _mapper = mapper;
            _gameSessionService = gameSessionService;
            _httpContextAccessorService = httpContextAccessorService;
        }

        public async Task<double> GetDistanceFromPlace(Coordinates actualPosition)
        {
            string id = _httpContextAccessorService.GetUserIdFromHeader();
            GameSession session = await _gameSessionService.GetSessionByGuid(id);

            if (session.GameMode != GameMode.Geolocation)
            {
                throw new Exception("Wrong game mode");
            }

            var actualRound = session.Rounds[session.ActualRoundNumber];

            var distance = DistanceCalculator.CalculateDistanceBetweenCords(new Coordinates
                { Latitude = actualRound.PlaceToGuess.Latitude, Longitude = actualRound.PlaceToGuess.Longitude }, actualPosition);

            return distance;
        }

    }
}
