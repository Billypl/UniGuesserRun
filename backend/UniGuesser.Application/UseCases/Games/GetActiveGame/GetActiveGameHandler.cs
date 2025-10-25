using AutoMapper;
using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Services;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Infrastructure;

namespace UniGuesser.Application.UseCases.Games.GetActiveGame
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
