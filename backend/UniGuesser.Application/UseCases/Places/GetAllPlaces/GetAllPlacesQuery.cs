using MediatR;
using UniGuesser.Application.Models.PlaceModels;

namespace UniGuesser.Application.UseCases.Places.GetAllPlaces
{
    public record GetAllPlacesQuery(bool PlacesInQueue) : IRequest<List<ShowPlaceDto>>;
}
