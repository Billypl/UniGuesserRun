using MongoDB.Bson;

namespace PartyGame.Models.GameModels
{
    public class Round
    {
        public string IDPlaceToGuess { get; set; }
        public Coordinates GuessedCoordinates { get; set; }
        public double Score { get; set; }
    }

}
