
using Models.PlaceModels;

namespace Models.GameModels
{
    public class RoundResultDto
    {
        public ShowPlaceDto OriginalPlace { get; set; }
        public double DistanceDifference { get; set; }
        public int RoundNumber { get; set; }

    }
}
