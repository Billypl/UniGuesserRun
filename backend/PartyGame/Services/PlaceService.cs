using AutoMapper;
using PartyGame.Entities;
using PartyGame.Extensions.Exceptions;
using PartyGame.Models.AccountModels;
using PartyGame.Models.PlaceModels;
using PartyGame.Repositories;

namespace PartyGame.Services
{
    public interface IPlaceService
    {
        Task<ShowPlaceDto> GetPlaceByPublicId(string id);
        Task DeletePlaceByPublicId(string id);
        Task UpdatePlaceByPublicId(string id,UpdatePlaceDto updatePlaceDto);
        Task<List<ShowPlaceDto>> GetAllPlaces();
        Task AddNewPlace(NewPlaceDto newPlace);
        Task<List<Place>> GetRandomPlaces(int numberOfRoundsToTake, DifficultyLevel difficultyLevel);
        Task AddNewPlaceToQueue(NewPlaceDto newPlace);
        Task<List<ShowPlaceDto>> GetAllPlacesInQueue();
        Task AcceptPlace(string PlaceToCheckId);
        Task RejectPlace(string PlaceToRejectId);

    }

    public class PlaceService : IPlaceService
    {
        private readonly IPlacesRepository _placesRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly IAccountService _accountService;

        public PlaceService(IPlacesRepository placesRepository,IMapper mapper,
            IHttpContextAccessorService httpContextAccessorService,
            IAccountService accountService)
        {
            _placesRepository = placesRepository;
            _mapper = mapper;
            _httpContextAccessorService = httpContextAccessorService;
            _accountService = accountService;
        }

        public async Task<ShowPlaceDto> GetPlaceByPublicId(string id)
        {
            Place? place = await _placesRepository.GetByPublicIdAsync(id);

            if (place is null)
            {
                throw new NotFoundException($"Place with ID {id} was not found.");
            }

            return _mapper.Map<ShowPlaceDto>(place); 
        }     
        public async Task DeletePlaceByPublicId(string id)
        {

            Place? place = await _placesRepository.GetByPublicIdAsync(id);

            if (place is null)
            {
                throw new NotFoundException($"Place with id {id} doesn't exist in the database");
            }

            var deleteResult = await _placesRepository.DeleteAsync(place.Id);   
        }

        public async Task UpdatePlaceByPublicId(string id,UpdatePlaceDto updatePlaceDto)
        {
            Place? place = await _placesRepository.GetByPublicIdAsync(id);

            if (place is null)
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
            place.InQueue = false;
            await _placesRepository.CreateAsync(place);
        }

        public async Task<List<Place>> GetRandomPlaces(int numberOfRoundsToTake, DifficultyLevel difficultyLevel)
        {
   
            var placesWithDifficulty = 
                (await _placesRepository.GetPlacesByDifficulty(difficultyLevel)).Where(p => p.InQueue == false).ToList();
            if (!placesWithDifficulty.Any())
            {
                return new List<Place>();
            }

            numberOfRoundsToTake = Math.Min(numberOfRoundsToTake, placesWithDifficulty.Count);

            var randomIndexes = new HashSet<int>();
            var random = new Random();

            while (randomIndexes.Count < numberOfRoundsToTake)
            {
                randomIndexes.Add(random.Next(0, placesWithDifficulty.Count));
            }

            var randomPlaces = randomIndexes
                .Select(index => placesWithDifficulty[index])
                .ToList();

             return randomPlaces;
        }

        public async Task AddNewPlaceToQueue(NewPlaceDto newPlace)
        {
            AccountDetailsFromTokenDto authorData = _httpContextAccessorService.GetAuthenticatedUserProfile();

            User user = await _accountService.GetAccountDetailsByPublicId(authorData.UserId);

            Place newPlaceToCheck = new Place
            {
                AuthorId = user.Id,
                CreatedAt = DateTime.Now,
                Name = newPlace.Name,
                Description = newPlace.Description,
                Latitude = newPlace.Coordinates.Latitude,
                Longitude = newPlace.Coordinates.Longitude,
                ImageUrl = newPlace.ImageUrl,
                Alt = newPlace.Alt,
                DifficultyLevel = newPlace.Difficulty,
                AuthorPlace = user,

            };

           await _placesRepository.CreateAsync(newPlaceToCheck);
        }

        public async Task<List<ShowPlaceDto>> GetAllPlacesInQueue()
        {
            IEnumerable<Place> placesTask = 
                (await _placesRepository.GetAllAsync()).Where(p => p.InQueue == true);
            List<ShowPlaceDto> placesDto = _mapper.Map<List<ShowPlaceDto>>(placesTask);
            return placesDto; 
        }
        public async Task AcceptPlace(string placeToCheckId)
        {
            Place? placeToAccept = _placesRepository.GetByPublicIdAsync(placeToCheckId).Result;

            if (placeToAccept == null)
            {
                throw new NotFoundException("Place you want to add to the game doesn't exist");
            }

            placeToAccept.InQueue = false;

            await _placesRepository.UpdateAsync(placeToAccept);
        }

        public async Task RejectPlace(string placeToRejectId)
        {
            Place? place = await _placesRepository.GetByPublicIdAsync(placeToRejectId);

            if (place is null)
            {
                throw new NotFoundException("Place you want to reject doesn't exist");
            }
            var result = await _placesRepository.DeleteAsync(place.Id);

          
        }
    }
}
