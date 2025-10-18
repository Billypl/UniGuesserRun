using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.Infrastructure.SharedModels.ScoreboardModels
{
    public enum SortDirection
    {
        ASC, DESC


    }
    public class ScoreboardQuery
    {
        public string? SearchNickname { get; set; }
        public DifficultyLevel? DifficultyLevel { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.DESC;



    }



}
