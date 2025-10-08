using MediatR;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Application.Models.ScoreboardModels;

namespace UniGuesser.Application.UseCases.GameSessions.GetScoreboardPage
{
    public class GetScoreboardPageHandler(IGameSessionRepository gameSessionRepository) : IRequestHandler<GetScoreboardPageQuery, PagedResult<UserStats>>
    {
        public async Task<PagedResult<UserStats>> Handle(GetScoreboardPageQuery request, CancellationToken cancellationToken)
        {
            var games = await gameSessionRepository.GetUsersStats(request.ScoreboardQuery);

            var pagedGames = games
                .Skip((request.ScoreboardQuery.PageNumber - 1) * request.ScoreboardQuery.PageSize)
                .Take(request.ScoreboardQuery.PageSize)
                .ToList();

            var result = new PagedResult<UserStats>(pagedGames, games.Count(), request.ScoreboardQuery.PageSize, request.ScoreboardQuery.PageNumber);
            return result;
        }
    }
}
