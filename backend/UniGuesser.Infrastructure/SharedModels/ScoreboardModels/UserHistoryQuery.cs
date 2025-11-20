using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.Infrastructure.SharedModels.ScoreboardModels;

public class UserHistoryQuery
{
    public DifficultyLevel? DifficultyLevel { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.DESC;
}