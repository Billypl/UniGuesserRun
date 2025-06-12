
namespace PartyGame.Models.GameModels
{
    public class GameSessionStateDto
    {
        public string Id { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int ActualRoundNumber { get; set; }
        public string DifficultyLevel { get; set; }

    }
}
