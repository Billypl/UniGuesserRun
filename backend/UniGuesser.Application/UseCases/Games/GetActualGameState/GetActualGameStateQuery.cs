using MediatR;
using UniGuesser.Application.Models.GameModels;

namespace UniGuesser.Application.UseCases.Games.GetActualGameState
{
    public record GetActualGameStateQuery(Guid Guid) : IRequest<GameSessionStateDto>
    {
        public GetActualGameStateQuery(string guid) : this(Guid.Parse(guid)) { }
    }
}
