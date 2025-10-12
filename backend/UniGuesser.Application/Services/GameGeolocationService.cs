using AutoMapper;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.Services;
using UniGuesser.Domain.ValueObjects;
using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.Application.Services
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
            Guid id = _httpContextAccessorService.GetUserIdFromHeader();
            GameSession session = await _gameSessionService.GetSessionByGuid(id);

            if (session.GameMode != GameMode.Geolocation)
            {
                throw new GameExceptions.WrongGameModeException(session.GameMode.ToString());
            }

            var actualRound = session.Rounds[session.ActualRoundNumber];

            var distance = DistanceCalculator.CalculateDistanceBetweenCords(new Coordinates
            { Latitude = actualRound.PlaceToGuess.Latitude, Longitude = actualRound.PlaceToGuess.Longitude }, actualPosition);

            return distance;
        }

    }
}
