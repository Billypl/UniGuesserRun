using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.Application.UseCases.Games.ChangeGameStatus
{
    public record ChangeGameStatusCommand(Guid Guid, GameStatus GameStatus) : IRequest<FinishedGameDto>
    {
        public ChangeGameStatusCommand(string id, GameStatus gameStatus) : this(Guid.Parse(id), gameStatus) { }
    }
}
