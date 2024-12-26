using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Services
{
    public class PlaceService
    {
        private readonly GameDbContext _gameDbContext;

        PlaceService(GameDbContext gameDbContext)
        {
            _gameDbContext = gameDbContext;
        }

        public async Task<Place> GetPlaceById(int id)
        {
            var place = await _gameDbContext.Places
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (place == null)
            {
                throw new KeyNotFoundException($"Place with ID {id} was not found.");
            }

            return place;
        }


    }
}
