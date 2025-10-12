using UniGuesser.Application.Models.PlaceModels;

namespace UniGuesser.Application.Models.GameModels
{
    public class RoundResultDto
    {
        public ShowPlaceDto OriginalPlace { get; set; }
        public double DistanceDifference { get; set; }
        public int RoundNumber { get; set; }

    }
}
