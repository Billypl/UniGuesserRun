using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Services;
using UniGuesser.Domain.Entities;
using UniGuesser.Infrastructure.Settings;

namespace UniGuesser.Application.UseCases.Games.CheckGuess
{
    public class CheckGuessHandler(IGameSessionService gameSessionService, IMapper mapper, IOptions<GameSettings> gameSettings) : IRequestHandler<CheckGuessCommand, RoundResultDto>
    {
        public async Task<RoundResultDto> Handle(CheckGuessCommand request, CancellationToken cancellationToken)
        {
            GameSession session = await gameSessionService.GetSessionByGuid(request.Guid);

            var result = await gameSessionService.CheckGuess(session.Id, request.GuessingCoordinates, gameSettings.Value.RoundsNumber);
            await gameSessionService.UpdateGameSession(session);
            return result;
        }
    }
}
