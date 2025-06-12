
using PartyGame.Entities;
using PartyGame.Models.GameModels;

namespace PartyGame.Models.ScoreboardModels
{
    public class FinishedGameDto
    {
        public string Id { get; set; }
        public string? UserId { get; set; }
        public string? Nickname { get; set; }
        public double FinalScore { get; set; }
        public List<Round> Rounds { get; set; }
        public string Difficulty { get; set; }

    }
}
