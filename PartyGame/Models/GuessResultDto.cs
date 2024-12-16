using PartyGame.Entities;

namespace PartyGame.Models
{
    public class GuessResultDto
    {
        public Place OriginalPlace { get; set; } 
        public Coordinates DistanceDifference { get; set; }

    }
}
