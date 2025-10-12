using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Domain.ValueObjects;
using UniGuesser.Infrastructure.SharedModels.ScoreboardModels;

namespace UniGuesser.Application.UseCases.GameSessions.GetGameHistoryPageByUser
{
    public record GetGameHistoryPageByUserQuery(UserHistoryQuery ScoreboardQuery, Guid Guid)
        : IRequest<PagedResult<FinishedGameDto>>
    {
        public GetGameHistoryPageByUserQuery(UserHistoryQuery scoreboardQuery, string guid) : this(scoreboardQuery,
            Guid.Parse(guid))
        {

        }
    }
}
