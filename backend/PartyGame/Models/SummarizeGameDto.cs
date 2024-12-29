using PartyGame.Entities;

namespace PartyGame.Models
{
    public class SummarizeGameDto
    {
        public List<Round> Rounds { get; set; }
        public double Score;
    }
}
