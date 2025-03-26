using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Extensions.Exceptions;
using PartyGame.Models.AccountModels;
using PartyGame.Models.PlaceModels;
using PartyGame.Repositories;

namespace PartyGame.Services
{
    public interface IPlaceService
    {
        Task<ShowPlaceDto> GetPlaceById(string id);
        Task DeletePlaceById(string id);
        Task UpdatePlaceById(string id,UpdatePlaceDto updatePlaceDto);
        Task<List<ShowPlaceDto>> GetAllPlaces();
        Task AddNewPlace(NewPlaceDto newPlace);
        Task<int> GetPlacesCount();
        Task<List<ObjectId>> GetRandomIDsOfPlaces(int numberOfRoundsToTake, DifficultyLevel difficultyLevel);
        Task AddNewPlaceToQueue(NewPlaceDto newPlace);
        Task<List<PlaceToCheckDto>> GetAllPlacesInQueue();
        Task AcceptPlace(string PlaceToCheckId);
        Task RejectPlace(string PlaceToRejectId);

    }

    public class PlaceService : IPlaceService
    {
        private readonly IPlacesRepository _placesRepository;
        private readonly IMapper _mapper;
        private readonly IPlacesToCheckRepository _placesToCheckRepository;
        private readonly IHttpContextAccessorService _httpContextAccessorService;

        public PlaceService(IPlacesRepository placesRepository,IMapper mapper,
            IHttpContextAccessorService httpContextAccessorService, IPlacesToCheckRepository placesToCheckRepository)
        {
            _placesRepository = placesRepository;
            _mapper = mapper;
            _httpContextAccessorService = httpContextAccessorService;
            _placesToCheckRepository = placesToCheckRepository;
        }

        public async Task<ShowPlaceDto> GetPlaceById(string id)
        {
            Place place = await _placesRepository.GetAsync(id);

            if (place == null)
            {
                throw new NotFoundException($"Place with ID {id} was not found.");
            }

            return _mapper.Map<ShowPlaceDto>(place); 
        }     
        public async Task DeletePlaceById(string id)
        {
            DeleteResult deleteResult = await _placesRepository.DeleteAsync(id);

            if (deleteResult.DeletedCount == 0)
            {
                throw new NotFoundException( $"Place with id {id} doesn't exist in the database" );
            }   
        }

        public async Task UpdatePlaceById(string id,UpdatePlaceDto updatePlaceDto)
        {
            Place place = await _placesRepository.GetAsync(id);
            if (place == null)
            {
                throw new NotFoundException($"Place with id {id} doesn't exist in the database and cannot be updated");
            }

            _mapper.Map(updatePlaceDto, place);
            await _placesRepository.UpdateAsync(place);
        }

        public async Task<List<ShowPlaceDto>> GetAllPlaces()
        {
            List<Place> places = (await _placesRepository.GetAllAsync()).ToList();

            if (places == null)
            {
                throw new NotFoundException($"There is no places in db");
            }

            List<ShowPlaceDto> placesToShow = _mapper.Map<List<ShowPlaceDto>>(places);

            return placesToShow;
        }

        public async Task AddNewPlace(NewPlaceDto newPlace)
        {
            Place place = new Place();
            _mapper.Map(newPlace, place);
            await _placesRepository.CreateAsync(place);
        }
        public async Task<int> GetPlacesCount()
        {
            var count = await _placesRepository.GetPlacesCount();
            return (int)count;
        }

        public async Task<List<ObjectId>> GetRandomIDsOfPlaces(int numberOfRoundsToTake, DifficultyLevel difficultyLevel)
        {
   
            var placesWithDifficulty = await _placesRepository.GetPlacesByDifficulty(difficultyLevel);
            if (!placesWithDifficulty.Any())
            {
                return new List<ObjectId>();
            }

            numberOfRoundsToTake = Math.Min(numberOfRoundsToTake, placesWithDifficulty.Count);

            var randomIndexes = new HashSet<int>();
            var random = new Random();

            while (randomIndexes.Count < numberOfRoundsToTake)
            {
                randomIndexes.Add(random.Next(0, placesWithDifficulty.Count));
            }

            var randomPlaces = randomIndexes
                .Select(index => placesWithDifficulty[index].Id)
                .ToList();

             return randomPlaces;
        }

        public async Task AddNewPlaceToQueue(NewPlaceDto newPlace)
        {
            AccountDetailsFromTokenDto authorData = _httpContextAccessorService.GetAuthenticatedUserProfile();

            PlaceToCheck newPlaceToCheck = new PlaceToCheck
            {
                AuthorId = authorData.UserId,
                CreatedAt = DateTime.Now,
                NewPlace = newPlace
            };

           await _placesToCheckRepository.CreateAsync(newPlaceToCheck);
        }

        public async Task<List<PlaceToCheckDto>> GetAllPlacesInQueue()
        {
            IEnumerable<PlaceToCheck> placesTask = await _placesToCheckRepository.GetAllAsync();
            List<PlaceToCheckDto> placesDto = _mapper.Map<List<PlaceToCheckDto>>(placesTask);
            return placesDto; 
        }
        public async Task AcceptPlace(string placeToCheckId)
        {
            PlaceToCheck placeToAccept = _placesToCheckRepository.GetAsync(placeToCheckId).Result;

            if (placeToAccept == null)
            {
                throw new NotFoundException("Place you want to add to the game doesn't exist");

            }

            Place newPlace = _mapper.Map<Place>(placeToAccept.NewPlace);

            await _placesRepository.CreateAsync(newPlace);
            await _placesToCheckRepository.DeleteAsync(placeToCheckId);
        }

        public async Task RejectPlace(string placeToRejectId)
        {
            DeleteResult result = await _placesToCheckRepository.DeleteAsync(placeToRejectId);

            if (result.DeletedCount == 0)
            {
                throw new NotFoundException("Place you want to reject doesn't exist");
            }
        }

    }
}
