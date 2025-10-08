using AutoMapper;
using MediatR;
using System;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Domain.Middleware.Exceptions;
using UniGuesser.Domain.Services;

namespace UniGuesser.Application.UseCases.Game.GetActualGameState
{
    public class GetActualGameStateHandler(IGameSessionRepository gameSessionRepository, IMapper mapper,
        IHttpContextAccessorService httpContextAccessorService) : IRequestHandler<GetActualGameStateQuery, GameSessionStateDto>
    {
        public async Task<GameSessionStateDto> Handle(GetActualGameStateQuery request, CancellationToken cancellationToken)
        {
            GameSession? session = await gameSessionRepository.GetActiveGameSession(request.Guid);

            if (session is null)
            {
                throw new GameSessionExceptions.GameNotFoundException(request.Guid);
            }

            return mapper.Map<GameSessionStateDto>(session);
        }
    }
}
