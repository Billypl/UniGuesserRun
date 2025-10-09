using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Models.ScoreboardModels;
using UniGuesser.Application.Models;

namespace UniGuesser.Application.UseCases.GameSessions.GetGameHistoryPageByUser
{
    public record GetGameHistoryPageByUserQuery(UserHistoryQuery ScoreboardQuery,Guid Guid)
        : IRequest<PagedResult<FinishedGameDto>>
    {
        public GetGameHistoryPageByUserQuery(UserHistoryQuery scoreboardQuery, string guid) : this(scoreboardQuery,
            Guid.Parse(guid))
        {

        }
    }
}
