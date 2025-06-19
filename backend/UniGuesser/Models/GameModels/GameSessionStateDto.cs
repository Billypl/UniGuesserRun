namespace PartyGame.Models.GameModels
{
    public class GameSessionStateDto
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; }
        public int ActualRoundNumber { get; set; }
        public double GameScore { get; set; }
        public string Difficulty { get; set; }
        public bool IsFinished { get; set; }
    }
}
