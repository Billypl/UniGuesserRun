using AutoMapper;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Models;
using PartyGame.Repositories;

namespace PartyGame.Services
{
    public interface IPlaceService
    {
        Task<Place> GetPlaceById(int id);
        Task<List<Place>> GetAllPlaces();
        void AddNewPlace(NewPlaceDto newPlace);
        Task<int> GetPlacesCount();
        Task<List<int>> GetRandomIDsOfPlaces(int numberOfRoundsToTake);
    }

    public class PlaceService : IPlaceService
    {
        private readonly IPlacesRepository _placesRepository;
        private readonly IMapper _mapper;

        public PlaceService(IPlacesRepository placesRepository,IMapper mapper)
        {
            _placesRepository = placesRepository;
            _mapper = mapper;
        }

        public async Task<Place> GetPlaceById(int id)
        {
            var place = await _placesRepository.GetPlaceById(id);

            if (place == null)
            {
                throw new KeyNotFoundException($"Place with ID {id} was not found.");
            }

            return place;
        }

        public async Task<List<Place>> GetAllPlaces()
        {
            var places = await _placesRepository.GetAllPlaces();

            if (places == null)
            {
                throw new KeyNotFoundException($"There is no places in db");
            }

            return places;
        }

        public void AddNewPlace(NewPlaceDto newPlace)
        {
            Place place = new Place();
            _mapper.Map(newPlace, place);
            place.Id = GetPlacesCount().Result;
            
            _placesRepository.AddNewPlace(place);
        }
        public async Task<int> GetPlacesCount()
        {
            var count = await _placesRepository.GetPlacesCount();
            return (int)count;
        }

        public async Task<List<int>> GetRandomIDsOfPlaces(int numberOfRoundsToTake)
        {
            var count = await _placesRepository.GetPlacesCount();
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
                var place = await _placesRepository.GetPlaceByIndex(index);

                if (place != null)
                {
                    randomPlaces.Add(place.Id);
                }
            }

            return randomPlaces;
        }


    }
}
