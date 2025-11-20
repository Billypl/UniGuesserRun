using MediatR;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.Repositories;

namespace UniGuesser.Application.UseCases.Places.ChangeQueueStatus;

public class ChangeQueueStatusHandler(IPlacesRepository placesRepository)
    : IRequestHandler<ChangeQueueStatusCommand, Unit>
{
    public async Task<Unit> Handle(ChangeQueueStatusCommand request, CancellationToken cancellationToken)
    {
        var placeToAccept = placesRepository.GetAsync(request.Guid).Result;

        if (placeToAccept == null) throw new PlacesExceptions.PlaceNotFoundException(request.Guid);
        placeToAccept.InQueue = request.InQueueStatus;

        await placesRepository.UpdateAsync(placeToAccept);

        return Unit.Value;
    }
}