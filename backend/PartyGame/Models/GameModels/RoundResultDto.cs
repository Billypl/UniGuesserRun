using PartyGame.Entities;

namespace PartyGame.Models.GameModels
{
    public class RoundResultDto
    {
        public Place OriginalPlace { get; set; }
        public double DistanceDifference { get; set; }
        public int RoundNumber { get; set; }

    }
}
