using MediatR;
using UniGuesser.Application.Models.GameModels;

namespace UniGuesser.Application.UseCases.Game.GetPlaceToGuess
{
    public record GetPlaceToGuessQuery(Guid Guid, int RoundNumber) : IRequest<GuessingPlaceDto>
    {
        public GetPlaceToGuessQuery(string id, int roundNumber) : this(Guid.Parse(id), roundNumber) { }
    }
}
