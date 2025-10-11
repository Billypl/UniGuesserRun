using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Models.ScoreboardModels;
using UniGuesser.Application.ValueObjects;

namespace UniGuesser.Application.UseCases.GameSessions.GetUserHistoryPage
{
    public record GetGameHistoryPageQuery(ScoreboardQuery ScoreboardQuery) : IRequest<PagedResult<FinishedGameDto>>;
}
