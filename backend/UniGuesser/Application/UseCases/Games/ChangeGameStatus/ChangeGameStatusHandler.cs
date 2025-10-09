using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using UniGuesser.Application.Models.Enumerations;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.UseCases.Game.CheckGuess;
using UniGuesser.Domain.Services;

namespace UniGuesser.Application.UseCases.Game.FinishGame
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

            if (tokenType == "user")
            {
                await gameSessionService.SetGameStatus(session, request.GameStatus);
                FinishedGameDto finishedGameDto = mapper.Map<FinishedGameDto>(session);
                return finishedGameDto;
            }
            else
            {
                await gameSessionService.DeleteSessionById(session.Id);
                return null;
            }
        }
    }
}
