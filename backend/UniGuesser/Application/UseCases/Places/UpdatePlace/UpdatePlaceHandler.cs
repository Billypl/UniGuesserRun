using AutoMapper;
using Azure.Core;
using MediatR;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models.PlaceModels;
using UniGuesser.Application.UseCases.Places.GetPlace;
using static UniGuesser.Domain.Middleware.Exceptions.PlacesExceptions;

namespace UniGuesser.Application.UseCases.Places.UpdatePlace
{
    public class UpdatePlaceHandler(IPlacesRepository placesRepository, IMapper mapper) : IRequestHandler<UpdatePlaceCommand, Unit>
    {
        public async Task<Unit> Handle(UpdatePlaceCommand request, CancellationToken cancellationToken)
        {
            Place? place = await placesRepository.GetByPublicIdAsync(request.PlaceId);

            if (place is null)
            {
                throw new PlaceNotFoundException(request.PlaceId);
            }

            mapper.Map(request.UpdatePlaceDto, place);
            await placesRepository.UpdateAsync(place);

            return Unit.Value;
        }
    }
}
