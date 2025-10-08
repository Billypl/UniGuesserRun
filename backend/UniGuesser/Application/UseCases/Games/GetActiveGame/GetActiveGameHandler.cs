using AutoMapper;
using MediatR;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Domain.Middleware.Exceptions;
using UniGuesser.Domain.Services;

namespace UniGuesser.Application.UseCases.Game.GetActiveGame
{
    public class GetActiveGameHandler(IGameSessionRepository gameSessionRepository, IMapper mapper, 
        IHttpContextAccessorService httpContextAccessorService) : IRequestHandler<GetActiveGameQuery, GameSessionStateDto>
    {
        public async Task<GameSessionStateDto> Handle(GetActiveGameQuery request, CancellationToken cancellationToken)
        {
            Guid gameSessionId = httpContextAccessorService.GetUserIdFromHeader();
            GameSession? session = await gameSessionRepository.GetActiveGameSessionByPlayerId(gameSessionId);

            if (session is null)
            {
                throw new GameSessionExceptions.GameNotFoundException(gameSessionId);
            }

            return mapper.Map<GameSessionStateDto>(session);
        }
    }
}
