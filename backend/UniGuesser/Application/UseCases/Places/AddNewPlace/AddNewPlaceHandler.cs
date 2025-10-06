using AutoMapper;
using MediatR;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Application.Models.PlaceModels;
using UniGuesser.Application.UseCases.Accounts.AccountDetails;

namespace UniGuesser.Application.UseCases.Places.AddNewPlace
{
    public class AddNewPlaceHandle(IPlacesRepository placesRepository, IMapper mapper) : IRequestHandler<AddNewPlaceCommand, Unit>
    {

        public async Task<Unit> Handle(AddNewPlaceCommand newPlace, CancellationToken cancellationToken)
        {
            Place place = new Place();
            mapper.Map(newPlace.newPlaceDto, place);
            place.InQueue = newPlace.inQueue;
            await placesRepository.CreateAsync(place);

            return Unit.Value;
        }
    }
}
