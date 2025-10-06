using AutoMapper;
using MediatR;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models.PlaceModels;
using UniGuesser.Application.UseCases.Places.GetAllPlaces;
using UniGuesser.Application.UseCases.Places.GetPlace;
using static UniGuesser.Domain.Middleware.Exceptions.PlacesExceptions;

namespace UniGuesser.Application.UseCases.Places.PlaceDetails
{
    public class GetPlacesDetailsHandler(IPlacesRepository placesRepository, IMapper mapper) : IRequestHandler<GetPlaceDetailsQuery, ShowPlaceDto>
    {
        public async Task<ShowPlaceDto> Handle(GetPlaceDetailsQuery request, CancellationToken cancellationToken)
        {
            Place? place = await placesRepository.GetByPublicIdAsync(request.guid);

            if (place is null)
            {
                throw new PlaceNotFoundException(request.guid);
            }

            return mapper.Map<ShowPlaceDto>(place);
        }
    }
}
