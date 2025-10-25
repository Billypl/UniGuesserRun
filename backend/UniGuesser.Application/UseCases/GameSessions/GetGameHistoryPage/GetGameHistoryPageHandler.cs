using AutoMapper;
using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Domain.Repositories;
using UniGuesser.Domain.ValueObjects;
using UniGuesser.Infrastructure;
using UniGuesser.Infrastructure.Repositories;

namespace UniGuesser.Application.UseCases.GameSessions.GetGameHistoryPage
{
    public class GetGameHistoryPageHandler(IGameSessionRepository gameSessionRepository, IMapper mapper) : IRequestHandler<GetGameHistoryPageQuery, PagedResult<FinishedGameDto>>
    {
        public async Task<PagedResult<FinishedGameDto>> Handle(GetGameHistoryPageQuery request, CancellationToken cancellationToken)
        {
            var games = await gameSessionRepository.GetGameHistoryPage(request.ScoreboardQuery);

            var pagedGames = games
                .Skip((request.ScoreboardQuery.PageNumber - 1) * request.ScoreboardQuery.PageSize)
                .Take(request.ScoreboardQuery.PageSize)
                .ToList();

            var mappedGames = mapper.Map<List<FinishedGameDto>>(pagedGames);

            var result = new PagedResult<FinishedGameDto>(mappedGames, games.Count(), request.ScoreboardQuery.PageSize, request.ScoreboardQuery.PageNumber);

            return result;
        }
    }
}
