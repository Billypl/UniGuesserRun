using MediatR;
using UniGuesser.Application.Models.GameModels;

namespace UniGuesser.Application.UseCases.Game.GetActiveGame
{
    public record GetActiveGameQuery() : IRequest<GameSessionStateDto>;
}
