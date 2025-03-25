using PartyGame.Models.GameModels;

namespace PartyGame.Models.ScoreboardModels
{
    public class FinishedGameToTable
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public double FinalScore { get; set; }
        public string DifficultyLevel { get; set; }
    }
}
