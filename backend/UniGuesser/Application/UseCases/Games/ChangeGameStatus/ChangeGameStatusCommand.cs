using MediatR;
using UniGuesser.Application.Models.Enumerations;
using UniGuesser.Application.Models.GameModels;

namespace UniGuesser.Application.UseCases.Game.FinishGame
{
    public record ChangeGameStatusCommand(Guid Guid,GameStatus GameStatus) : IRequest<FinishedGameDto>
    {
        public ChangeGameStatusCommand(string id, GameStatus gameStatus) : this(Guid.Parse(id), gameStatus) { }
    }
}
