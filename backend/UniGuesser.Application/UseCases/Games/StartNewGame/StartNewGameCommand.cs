using MediatR;
using UniGuesser.Application.Models.GameModels;

namespace UniGuesser.Application.UseCases.Games.StartNewGame
{
    public record StartNewGameCommand(StartDataDto startDataDto) : IRequest<StartedGameData>;
}
