namespace UniGuesser.Application.Models.GameModels
{
    public class GameSessionStateDto
    {
        public Guid Id { get; set; }
        public int ActualRoundNumber { get; set; }
        public double GameScore { get; set; }
        public string Difficulty { get; set; }
        public bool IsFinished { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
