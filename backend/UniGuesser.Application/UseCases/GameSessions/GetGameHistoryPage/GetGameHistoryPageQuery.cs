using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Domain.ValueObjects;
using UniGuesser.Infrastructure.SharedModels.ScoreboardModels;

namespace UniGuesser.Application.UseCases.GameSessions.GetGameHistoryPage;

public record GetGameHistoryPageQuery(ScoreboardQuery ScoreboardQuery) : IRequest<PagedResult<FinishedGameDto>>;