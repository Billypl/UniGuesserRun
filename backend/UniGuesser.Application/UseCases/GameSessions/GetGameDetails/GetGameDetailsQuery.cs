using MediatR;
using UniGuesser.Application.Models.GameModels;

namespace UniGuesser.Application.UseCases.GameSessions.GetGameDetails;

public record GetGameDetailsQuery(Guid Guid) : IRequest<FinishedGameDto>
{
    public GetGameDetailsQuery(string id) : this(Guid.Parse(id))
    {
    }
}