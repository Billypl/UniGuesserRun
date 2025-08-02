using PartyGame.Models.GameModels;

namespace PartyGame.Models.ScoreboardModels
{
    public class UserStats
    {
        public string Guid { get; set; }
        public string Nickname { get; set; }
        public int GamePlayed { get; set; }
        public double AverageScore { get; set; }
    }
}
