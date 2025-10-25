using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Domain.ValueObjects;

namespace UniGuesser.Application.UseCases.Games.CheckGuess
{
    public record CheckGuessCommand(Guid Guid, Coordinates GuessingCoordinates) : IRequest<RoundResultDto>
    {
        public CheckGuessCommand(string id, Coordinates guessingCoordinates) : this(Guid.Parse(id), guessingCoordinates) { }
    }
}
