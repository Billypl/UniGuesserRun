namespace UniGuesser.Application.Models.ScoreboardModels
{
    public class UserHistoryQuery
    {
        public string? DifficultyLevel { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.DESC;

    }

}
