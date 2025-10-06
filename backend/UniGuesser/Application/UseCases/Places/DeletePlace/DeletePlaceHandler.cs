using AutoMapper;
using MediatR;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models.PlaceModels;
using UniGuesser.Application.UseCases.Places.AddNewPlace;
using static UniGuesser.Domain.Middleware.Exceptions.PlacesExceptions;

namespace UniGuesser.Application.UseCases.Places.DeletePlace
{
    public class DeletePlaceHandler(IPlacesRepository placesRepository, IMapper mapper) : IRequestHandler<DeletePlaceCommand, Unit>
    {
        public async Task<Unit> Handle(DeletePlaceCommand request, CancellationToken cancellationToken)
        {
            Place? place = await placesRepository.GetByPublicIdAsync(request.guid);

            if (place is null)
            {
                throw new PlaceNotFoundException(request.guid);
            }

            var deleteResult = await placesRepository.DeleteAsync(place.Id);

            return Unit.Value;
        }
    }
}
