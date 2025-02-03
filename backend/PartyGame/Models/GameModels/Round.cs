namespace PartyGame.Models.GameModels
{
    public class Round
    {
        public int IDPlaceToGuess { get; set; }
        public Coordinates GuessedCoordinates { get; set; }
        public double Score { get; set; }
    }

}
