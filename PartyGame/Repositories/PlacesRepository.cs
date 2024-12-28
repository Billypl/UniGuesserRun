using AutoMapper;
using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Repositories
{
    public interface IPlacesRepository
    {
        Task<Place> GetPlaceById(int id);
        Task<List<Place>> GetAllPlaces();
        void AddNewPlace(Place newPlace);
        Task<long> GetPlacesCount();
        Task<Place> GetPlaceByIndex(int index);
    }

    public class PlacesRepository : IPlacesRepository
    {
        private readonly GameDbContext _gameDbContext;


        public PlacesRepository(GameDbContext gameDbContext, IMapper mapper)
        {
            _gameDbContext = gameDbContext;
        }

        public async Task<Place> GetPlaceById(int id)
        {
            return await _gameDbContext.Places
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Place>> GetAllPlaces()
        {
            return await _gameDbContext.Places.Find(FilterDefinition<Place>.Empty).ToListAsync();
        }

        public async void AddNewPlace(Place newPlace)
        {
            await _gameDbContext.Places.InsertOneAsync(newPlace);
        }

        public async Task<long> GetPlacesCount()
        {
            return await _gameDbContext.Places.CountDocumentsAsync(FilterDefinition<Place>.Empty);
        }

        public async Task<Place> GetPlaceByIndex(int index)
        {
            return await _gameDbContext.Places
                .Find(_ => true)
                .Skip(index)
                .Limit(1)
                .FirstOrDefaultAsync();
        }


    }
}
