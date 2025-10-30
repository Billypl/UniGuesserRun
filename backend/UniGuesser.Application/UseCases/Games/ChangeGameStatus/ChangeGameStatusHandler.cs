using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Services;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.ValueObjects.Enumerations;
using UniGuesser.Infrastructure.Settings;
using IGameSessionRepository = UniGuesser.Infrastructure.IGameSessionRepository;

namespace UniGuesser.Application.UseCases.Games.ChangeGameStatus;

public class ChangeGameStatusHandler(
    IGameSessionRepository gameSessionRepository,
    IMapper mapper,
    IHttpContextAccessorService httpContextAccessorService,
    IOptions<GameSettings> gameSettings
) : IRequestHandler<ChangeGameStatusCommand, FinishedGameDto>
{
    public async Task<FinishedGameDto> Handle(ChangeGameStatusCommand request, CancellationToken cancellationToken)
    {
        var session = await gameSessionRepository.GetAsync(request.Guid);

        if (session == null) throw new GameSessionExceptions.GameNotFoundException(request.Guid);

        if (request.GameStatus == GameStatus.Finished) session.EnsureGameFinished(gameSettings.Value.RoundsNumber);

        var tokenType = httpContextAccessorService.GetTokenType();
        var finishedGameDto = mapper.Map<FinishedGameDto>(session);

        if (tokenType == "user")
            session.GameState = request.GameStatus;
        else
            session.GameState = GameStatus.ToDelete;

        await gameSessionRepository.UpdateAsync(session);

        return finishedGameDto;
    }
}