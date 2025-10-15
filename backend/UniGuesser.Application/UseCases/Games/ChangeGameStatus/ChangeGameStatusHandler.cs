using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Services;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.ValueObjects.Enumerations;
using UniGuesser.Infrastructure.Settings;

namespace UniGuesser.Application.UseCases.Games.ChangeGameStatus
{
    public class ChangeGameStatusHandler(
        IGameSessionService gameSessionService,
        IMapper mapper,
        IHttpContextAccessorService httpContextAccessorService,
        IOptions<GameSettings> gameSettings
    ) : IRequestHandler<ChangeGameStatusCommand, FinishedGameDto>
    {
        public async Task<FinishedGameDto> Handle(ChangeGameStatusCommand request, CancellationToken cancellationToken)
        {
            GameSession session = await gameSessionService.GetSessionByGuid(request.Guid);

            if (request.GameStatus == GameStatus.Finished)
            {
                session.EnsureGameFinished(gameSettings.Value.RoundsNumber);
            }

            string tokenType = httpContextAccessorService.GetTokenType();
            FinishedGameDto finishedGameDto = mapper.Map<FinishedGameDto>(session);
            if (tokenType == "user")
            {
                await gameSessionService.SetGameStatus(session, request.GameStatus);
            }
            else
            {
                await gameSessionService.SetGameStatus(session, GameStatus.ToDelete);
            }
            return finishedGameDto;
        }
    }
}
