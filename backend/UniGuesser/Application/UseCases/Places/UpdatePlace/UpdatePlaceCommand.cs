using MediatR;
using UniGuesser.Application.Models.PlaceModels;

namespace UniGuesser.Application.UseCases.Places.UpdatePlace
{
    public record UpdatePlaceCommand(UpdatePlaceDto UpdatePlaceDto, Guid PlaceId) : IRequest<Unit>
    {
        public UpdatePlaceCommand(UpdatePlaceDto updatePlaceDto,string id) : this(updatePlaceDto,Guid.Parse(id)) { }
    }
}
