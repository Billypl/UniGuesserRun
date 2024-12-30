using PartyGame.Entities;

namespace PartyGame.Models
{
    public class RoundResultDto
    {
        public Place OriginalPlace { get; set; } 
        public double DistanceDifference { get; set; }
        public int RoundNumber { get; set; } 

    }
}
