using PartyGame.Entities;

namespace PartyGame.Models.GameModels
{
    public class SummarizeGameDto
    {
        public List<Round> Rounds { get; set; }
        public double Score { get; set; }
    }
}
