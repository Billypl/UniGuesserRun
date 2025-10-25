using AutoMapper;
using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Services;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Infrastructure;
using IGameSessionRepository = UniGuesser.Infrastructure.IGameSessionRepository;

namespace UniGuesser.Application.UseCases.Games.GetActualGameState
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
