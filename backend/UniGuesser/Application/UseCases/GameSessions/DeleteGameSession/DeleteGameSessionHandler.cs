using MediatR;
using System;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Domain.Middleware.Exceptions;

namespace UniGuesser.Application.UseCases.GameSessions.DeleteGameSession
{
    public class DeleteGameSessionHandler(IGameSessionRepository gameSessionRepository) : IRequestHandler<DeleteGameSessionCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteGameSessionCommand request, CancellationToken cancellationToken)
        {
            GameSession? session = await gameSessionRepository.GetByPublicIdAsync(request.Guid);

            if (session is null)
            {
                throw new GameSessionExceptions.GameNotFoundException(request.Guid); ;
            }

            var deleteResult = await gameSessionRepository.DeleteAsync(session.Id);
            if (deleteResult == false)
            {
                throw new GameSessionExceptions.GameNotFoundException(session.Id);
            }

            
            return Unit.Value;
        }
    }
}
