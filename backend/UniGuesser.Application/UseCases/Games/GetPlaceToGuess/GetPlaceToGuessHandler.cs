using AutoMapper;
using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Services;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Infrastructure;
using IGameSessionRepository = UniGuesser.Infrastructure.IGameSessionRepository;

namespace UniGuesser.Application.UseCases.Games.GetPlaceToGuess
{
    public class GetPlaceToGuessHandler(IGameSessionRepository gameSessionRepository, IMapper mapper) : IRequestHandler<GetPlaceToGuessQuery, GuessingPlaceDto>
    {
        public async Task<GuessingPlaceDto> Handle(GetPlaceToGuessQuery request, CancellationToken cancellationToken)
        {
            GameSession? session = await gameSessionRepository.GetAsync(request.Guid);

            if (session is null)
            {
                throw new GameSessionExceptions.GameNotFoundException(request.Guid);
            }

            var guessingPlace = session.GetRoundOrThrow(request.RoundNumber).PlaceToGuess;

            return mapper.Map<GuessingPlaceDto>(guessingPlace);
        }
    }
}
