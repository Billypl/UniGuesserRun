using MediatR;
using UniGuesser.Application.Services;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.Repositories;
using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.Application.UseCases.Places.DeletePlace;

public class DeletePlaceHandler(IPlacesRepository placesRepository, IFileService fileService)
    : IRequestHandler<DeletePlaceCommand, Unit>
{
    public async Task<Unit> Handle(DeletePlaceCommand request, CancellationToken cancellationToken)
    {
        var place = await placesRepository.GetAsync(request.guid);

        if (place is null) throw new PlacesExceptions.PlaceNotFoundException(request.guid);

        if (place.ImageType == ImageType.File) fileService.DeleteFile(place.ImageUrl);

        var deleteResult = await placesRepository.DeleteAsync(place.Id);

        return Unit.Value;
    }
}