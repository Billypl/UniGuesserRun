using AutoMapper;
using MediatR;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.Repositories;

namespace UniGuesser.Application.UseCases.Places.UpdatePlace
{
    public class UpdatePlaceHandler(IPlacesRepository placesRepository, IMapper mapper) : IRequestHandler<UpdatePlaceCommand, Unit>
    {
        public async Task<Unit> Handle(UpdatePlaceCommand request, CancellationToken cancellationToken)
        {
            Place? place = await placesRepository.GetByPublicIdAsync(request.PlaceId);

            if (place is null)
            {
                throw new PlacesExceptions.PlaceNotFoundException(request.PlaceId);
            }

            mapper.Map(request.UpdatePlaceDto, place);
            await placesRepository.UpdateAsync(place);

            return Unit.Value;
        }
    }
}
