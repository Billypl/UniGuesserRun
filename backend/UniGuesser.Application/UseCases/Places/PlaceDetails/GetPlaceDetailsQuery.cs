using MediatR;
using UniGuesser.Application.Models.PlaceModels;

namespace UniGuesser.Application.UseCases.Places.PlaceDetails;

public record GetPlaceDetailsQuery(Guid guid) : IRequest<ShowPlaceDto>
{
    public GetPlaceDetailsQuery(string id) : this(Guid.Parse(id))
    {
    }
}