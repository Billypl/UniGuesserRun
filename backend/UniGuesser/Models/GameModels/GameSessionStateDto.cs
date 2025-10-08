namespace Models.GameModels
{
    public class GameSessionStateDto
    {
        public string Id { get; set; }
        public Guid PublicId { get; set; }
        public int ActualRoundNumber { get; set; }
        public double GameScore { get; set; }
        public string Difficulty { get; set; }
        public bool IsFinished { get; set; }
    }
}
