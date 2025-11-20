using MediatR;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Infrastructure;

namespace UniGuesser.Application.UseCases.GameSessions.DeleteGameSession;

public class DeleteGameSessionHandler(IGameSessionRepository gameSessionRepository)
    : IRequestHandler<DeleteGameSessionCommand, Unit>
{
    public async Task<Unit> Handle(DeleteGameSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await gameSessionRepository.GetAsync(request.Guid);

        if (session is null)
        {
            throw new GameSessionExceptions.GameNotFoundException(request.Guid);
            ;
        }

        var deleteResult = await gameSessionRepository.DeleteAsync(session.Id);
        if (deleteResult == false) throw new GameSessionExceptions.GameNotFoundException(session.Id);


        return Unit.Value;
    }
}