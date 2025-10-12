using MediatR;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.Repositories;

namespace UniGuesser.Application.UseCases.Places.ChangeQueueStatus
{
    public class ChangeQueueStatusHandler(IPlacesRepository placesRepository) : IRequestHandler<ChangeQueueStatusCommand, Unit>
    {
        public async Task<Unit> Handle(ChangeQueueStatusCommand request, CancellationToken cancellationToken)
        {
            Place? placeToAccept = placesRepository.GetByPublicIdAsync(request.Guid).Result;

            if (placeToAccept == null)
            {
                throw new NotFoundException("Place you want to add to the game doesn't exist");
            }
            placeToAccept.InQueue = request.InQueueStatus;

            await placesRepository.UpdateAsync(placeToAccept);

            return Unit.Value;
        }
    }
}
