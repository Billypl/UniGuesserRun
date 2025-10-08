using MediatR;
using UniGuesser.Application.Models.GameModels;

namespace UniGuesser.Application.UseCases.Game.CheckGuess
{
    public record CheckGuessCommand(Guid Guid, Coordinates GuessingCoordinates) : IRequest<RoundResultDto>
    {
        public CheckGuessCommand(string id, Coordinates guessingCoordinates) : this(Guid.Parse(id), guessingCoordinates) { }
    }
}
