using Microsoft.Extensions.Options;
using PartyGame.Entities;
using PartyGame.Settings;

namespace PartyGame.Services.GameServices.GameStartStrategies
{
    public interface IGameRoundsGenerator
    {
        Task<List<Round>> GenerateRounds(DifficultyLevel difficulty);
    }

    public class GameRoundsGenerator : IGameRoundsGenerator
    {
        private readonly IPlaceService _placeService;
        private readonly GameSettings _gameSettings;


        public GameRoundsGenerator(IPlaceService placeService, IOptions<GameSettings> gameSettings)
        {
            _placeService = placeService;
            _gameSettings = gameSettings.Value;
        }

        public async Task<List<Round>> GenerateRounds(DifficultyLevel difficulty)
        {
            List<Place> places = await _placeService.GetRandomPlaces(_gameSettings.RoundsNumber, difficulty);

            if (places.Count < _gameSettings.RoundsNumber)
            {
                throw new InvalidOperationException("Not enough places retrieved from database.");
            }

            return places.Select(p => new Round
            {
                PlaceId = p.Id,
                PlaceToGuess = p,
                Score = 0
            }).ToList();
        }
    }

}
