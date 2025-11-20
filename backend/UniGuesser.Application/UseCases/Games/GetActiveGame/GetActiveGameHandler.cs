using AutoMapper;
using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Services;
using UniGuesser.Domain.Exceptions;
using IGameSessionRepository = UniGuesser.Infrastructure.IGameSessionRepository;

namespace UniGuesser.Application.UseCases.Games.GetActiveGame;

public class GetActiveGameHandler(
    IGameSessionRepository gameSessionRepository,
    IMapper mapper,
    IHttpContextAccessorService httpContextAccessorService) : IRequestHandler<GetActiveGameQuery, GameSessionStateDto>
{
    public async Task<GameSessionStateDto> Handle(GetActiveGameQuery request, CancellationToken cancellationToken)
    {
        var gameSessionId = httpContextAccessorService.GetUserIdFromHeader();
        var session = await gameSessionRepository.GetActiveGameSessionByPlayerId(gameSessionId);

        if (session is null) throw new GameSessionExceptions.GameNotFoundException(gameSessionId);

        return mapper.Map<GameSessionStateDto>(session);
    }
}