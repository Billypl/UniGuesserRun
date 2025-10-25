using AutoMapper;
using MediatR;
using UniGuesser.Application.Models.PlaceModels;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.Repositories;

namespace UniGuesser.Application.UseCases.Places.GetAllPlaces
{
    public class GetAllPlacesHandler(IPlacesRepository placesRepository, IMapper mapper) : IRequestHandler<GetAllPlacesQuery, List<ShowPlaceDto>>
    {
        public async Task<List<ShowPlaceDto>> Handle(GetAllPlacesQuery request, CancellationToken cancellationToken)
        {
            List<Place> places = (await placesRepository.GetAllAsync()).Where(p => p.InQueue == request.PlacesInQueue).ToList();

            if (places == null)
            {
                throw new PlacesExceptions.PlacesNotFoundException();
            }

            List<ShowPlaceDto> placesToShow = mapper.Map<List<ShowPlaceDto>>(places);

            return placesToShow;
        }
    }
}
