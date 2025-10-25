using MediatR;
using UniGuesser.Domain.ValueObjects;
using UniGuesser.Infrastructure.SharedModels.ScoreboardModels;

namespace UniGuesser.Application.UseCases.GameSessions.GetScoreboardPage
{
    public record GetScoreboardPageQuery(ScoreboardQuery ScoreboardQuery) : IRequest<PagedResult<UserStats>>;
}
