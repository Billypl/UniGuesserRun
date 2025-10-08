using AutoMapper;
using MediatR;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.UseCases.Accounts.AccountDetails;
using UniGuesser.Domain.Services;

namespace UniGuesser.Application.UseCases.Game.CheckGuess
{
    public class CheckGuessHandler(IGameSessionService gameSessionService, IMapper mapper) : IRequestHandler<CheckGuessCommand, RoundResultDto>
    {
        public async Task<RoundResultDto> Handle(CheckGuessCommand request, CancellationToken cancellationToken)
        {
            GameSession session = await gameSessionService.GetSessionByGuid(request.Guid);

            var result = session.CheckGuess(request.GuessingCoordinates, mapper, _gameSettings.RoundsNumber);
            await gameSessionService.UpdateGameSession(session);
            return result;
        }
    }
}
