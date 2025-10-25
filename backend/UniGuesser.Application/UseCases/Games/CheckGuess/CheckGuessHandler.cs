using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Models.PlaceModels;
using UniGuesser.Application.Services;
using UniGuesser.Domain.Entities;
using UniGuesser.Infrastructure;
using UniGuesser.Infrastructure.Settings;
using IGameSessionRepository = UniGuesser.Infrastructure.IGameSessionRepository;

namespace UniGuesser.Application.UseCases.Games.CheckGuess
{
    public class CheckGuessHandler(IGameSessionRepository gameSessionRepository, IMapper mapper, IOptions<GameSettings> gameSettings) : IRequestHandler<CheckGuessCommand, RoundResultDto>
    {
        public async Task<RoundResultDto> Handle(CheckGuessCommand request, CancellationToken cancellationToken)
        {
            var session = await gameSessionRepository.GetActiveGameSession(request.Guid);

            var distance = session.CheckGuess(request.GuessingCoordinates, gameSettings.Value.RoundsNumber);

            await gameSessionRepository.UpdateAsync(session);

            return new RoundResultDto
            {
                DistanceDifference = distance,
                RoundNumber = session.ActualRoundNumber - 1,
                OriginalPlace = mapper.Map<ShowPlaceDto>(session.Rounds[session.ActualRoundNumber - 1].PlaceToGuess)
            };
        }
    }
}
