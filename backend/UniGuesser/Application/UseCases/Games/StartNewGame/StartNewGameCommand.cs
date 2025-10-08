using Azure.Core;
using MediatR;
using UniGuesser.Application.Models.GameModels;

namespace UniGuesser.Application.UseCases.Game.StartNewGame
{
    public record StartNewGameCommand(StartDataDto startDataDto) : IRequest<StartedGameData>;
}
