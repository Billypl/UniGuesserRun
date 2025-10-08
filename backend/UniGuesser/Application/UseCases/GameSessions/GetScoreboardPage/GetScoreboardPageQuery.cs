using MediatR;
using UniGuesser.Application.Models;
using UniGuesser.Application.Models.ScoreboardModels;

namespace UniGuesser.Application.UseCases.GameSessions.GetScoreboardPage
{
    public record GetScoreboardPageQuery(ScoreboardQuery ScoreboardQuery) : IRequest<PagedResult<UserStats>>;
}
