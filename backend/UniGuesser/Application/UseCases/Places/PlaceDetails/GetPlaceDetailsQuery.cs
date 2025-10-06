using MediatR;
using UniGuesser.Application.Models.PlaceModels;

namespace UniGuesser.Application.UseCases.Places.GetPlace
{
    public record GetPlaceDetailsQuery(Guid guid): IRequest<ShowPlaceDto>
    {
        public GetPlaceDetailsQuery(string id) : this(Guid.Parse(id)) { }
    }
}
