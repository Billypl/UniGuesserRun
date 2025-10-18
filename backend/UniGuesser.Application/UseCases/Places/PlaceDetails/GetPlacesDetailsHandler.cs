using AutoMapper;
using MediatR;
using UniGuesser.Application.Models.PlaceModels;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.Repositories;

namespace UniGuesser.Application.UseCases.Places.PlaceDetails
{
    public class GetPlacesDetailsHandler(IPlacesRepository placesRepository, IMapper mapper) : IRequestHandler<GetPlaceDetailsQuery, ShowPlaceDto>
    {
        public async Task<ShowPlaceDto> Handle(GetPlaceDetailsQuery request, CancellationToken cancellationToken)
        {
            Place? place = await placesRepository.GetAsync(request.guid);

            if (place is null)
            {
                throw new PlacesExceptions.PlaceNotFoundException(request.guid);
            }

            return mapper.Map<ShowPlaceDto>(place);
        }
    }
}
