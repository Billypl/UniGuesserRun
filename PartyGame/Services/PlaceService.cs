using AutoMapper;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Models;

namespace PartyGame.Services
{
    public interface IPlaceService
    {
        Task<Place> GetPlaceById(int id);
        Task<List<Place>> GetAllPlaces();
        Task<int> AddNewPlace(NewPlaceDto newPlace);
    }

    public class PlaceService : IPlaceService
    {
        private readonly GameDbContext _gameDbContext;
        private readonly IMapper _mapper;

        public PlaceService(GameDbContext gameDbContext,IMapper mapper)
        {
            _gameDbContext = gameDbContext;
            _mapper = mapper;
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

        public async Task<List<Place>> GetAllPlaces()
        {
            var places = await _gameDbContext.Places.Find(FilterDefinition<Place>.Empty).ToListAsync();

            if (places == null)
            {
                throw new KeyNotFoundException($"There is no places in db");
            }

            return places;
        }

        public async Task<int> AddNewPlace(NewPlaceDto newPlace)
        {
            Place place = new Place();
            _mapper.Map(newPlace, place);
            place.Id = GetPlacesCount().Result;

            await _gameDbContext.Places.InsertOneAsync(place); 

            return 1; 
        }
        public async Task<int> GetPlacesCount()
        {
            var count = await _gameDbContext.Places.CountDocumentsAsync(FilterDefinition<Place>.Empty);
            return (int)count;
        }


    }
}
