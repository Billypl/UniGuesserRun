using MediatR;
using UniGuesser.Application.Models.ScoreboardModels;
using UniGuesser.Application.ValueObjects;

namespace UniGuesser.Application.UseCases.GameSessions.GetScoreboardPage
{
    public record GetScoreboardPageQuery(ScoreboardQuery ScoreboardQuery) : IRequest<PagedResult<UserStats>>;
}
