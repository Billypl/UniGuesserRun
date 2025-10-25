using AutoMapper;
using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Domain.Repositories;
using UniGuesser.Domain.ValueObjects;
using UniGuesser.Infrastructure;
using UniGuesser.Infrastructure.Repositories;

namespace UniGuesser.Application.UseCases.GameSessions.GetGameHistoryPageByUser
{
    public class GetGameHistoryPageByUserHandler(IGameSessionRepository gameSessionRepository, IMapper mapper) : IRequestHandler<GetGameHistoryPageByUserQuery, PagedResult<FinishedGameDto>>
    {
        public async Task<PagedResult<FinishedGameDto>> Handle(GetGameHistoryPageByUserQuery request, CancellationToken cancellationToken)
        {
            var games = await gameSessionRepository.GetGameUserHistoryGames(request.ScoreboardQuery, request.Guid);

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
