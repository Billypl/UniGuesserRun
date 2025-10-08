using MediatR;

namespace UniGuesser.Application.UseCases.GameSessions.DeleteGameSession
{
    public record DeleteGameSessionCommand(Guid Guid) : IRequest<Unit>
    {
        public DeleteGameSessionCommand(string id) : this(Guid.Parse(id)) { }
    }
}
