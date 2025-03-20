namespace PartyGame.Models.ScoreboardModels
{
    public class GameHistoryQuery
    {
        public string? DifficultyLevel { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.DESC;
    }
}
