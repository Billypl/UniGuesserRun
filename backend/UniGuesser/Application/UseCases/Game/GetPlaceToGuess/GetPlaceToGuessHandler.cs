using AutoMapper;
using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.UseCases.Game.StartNewGame;
using UniGuesser.Domain.Services;

namespace UniGuesser.Application.UseCases.Game.GetPlaceToGuess
{
    public class GetPlaceToGuessHandler(IGameSessionService gameSessionService, IMapper mapper) : IRequestHandler<GetPlaceToGuessQuery, GuessingPlaceDto>
    {
        public async Task<GuessingPlaceDto> Handle(GetPlaceToGuessQuery request, CancellationToken cancellationToken)
        {
            GameSession session = await gameSessionService.GetSessionByGuid(request.Guid);

            var guessingPlace = session.GetRoundOrThrow(request.RoundNumber).PlaceToGuess;

            return mapper.Map<GuessingPlaceDto>(guessingPlace);
        }
    }
}
