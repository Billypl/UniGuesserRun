using MediatR;
using UniGuesser.Application.Models;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Models.ScoreboardModels;

namespace UniGuesser.Application.UseCases.GameSessions.GetUserHistoryPage
{
    public record GetGameHistoryPageQuery(ScoreboardQuery ScoreboardQuery) : IRequest<PagedResult<FinishedGameDto>>;
}
