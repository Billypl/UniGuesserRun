using MediatR;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.Repositories;

namespace UniGuesser.Application.UseCases.Places.DeletePlace
{
    public class DeletePlaceHandler(IPlacesRepository placesRepository) : IRequestHandler<DeletePlaceCommand, Unit>
    {
        public async Task<Unit> Handle(DeletePlaceCommand request, CancellationToken cancellationToken)
        {
            Place? place = await placesRepository.GetByPublicIdAsync(request.guid);

            if (place is null)
            {
                throw new PlacesExceptions.PlaceNotFoundException(request.guid);
            }

            var deleteResult = await placesRepository.DeleteAsync(place.Id);

            return Unit.Value;
        }
    }
}
