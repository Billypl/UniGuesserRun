using AutoMapper;
using Entities;
using Models.AccountModels;
using Models.PlaceModels;
using Repositories;
using static UniGuesser.Middleware.Exceptions.PlacesExceptions;

namespace Services
{
    public interface IPlaceQueueService
    {
        Task AddNewPlaceToQueue(NewPlaceDto newPlace);
        Task<List<ShowPlaceDto>> GetAllPlacesInQueue();
        Task AcceptPlace(string placeToCheckId);
        Task RejectPlace(string placeToRejectId);
    }

    public class PlaceQueueService : IPlaceQueueService
    {

        private readonly IPlacesRepository _placesRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly IAccountService _accountService;

        public PlaceQueueService(IPlacesRepository placesRepository, IMapper mapper,
            IHttpContextAccessorService httpContextAccessorService,
            IAccountService accountService)
        {
            _placesRepository = placesRepository;
            _mapper = mapper;
            _httpContextAccessorService = httpContextAccessorService;
            _accountService = accountService;
        }

        public async Task AddNewPlaceToQueue(NewPlaceDto newPlace)
        {
            AccountDetailsFromTokenDto authorData = _httpContextAccessorService.GetAuthenticatedUserProfile();

            User user = await _accountService.GetAccountDetailsByPublicId(authorData.Guid);

            Place newPlaceToCheck = _mapper.Map<Place>(newPlace);
            newPlaceToCheck.AuthorId = user.Id;
            newPlaceToCheck.CreatedAt = DateTime.Now;
            newPlaceToCheck.AuthorPlace = user;
            newPlaceToCheck.InQueue = true;

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
                throw new PlaceNotFoundInQueueException(placeToCheckId);
            }

            placeToAccept.InQueue = false;

            await _placesRepository.UpdateAsync(placeToAccept);
        }

        public async Task RejectPlace(string placeToRejectId)
        {
            Place? place = await _placesRepository.GetByPublicIdAsync(placeToRejectId);

            if (place is null)
            {
                throw new PlaceNotFoundInQueueException(placeToRejectId);
            }
            var result = await _placesRepository.DeleteAsync(place.Id);

        }


    }
}
