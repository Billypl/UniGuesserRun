using MediatR;

namespace UniGuesser.Application.UseCases.Places.DeletePlace
{
    public record DeletePlaceCommand(Guid guid): IRequest<Unit>
    {
        public DeletePlaceCommand(string id) : this(Guid.Parse(id)) { }
    }
}
