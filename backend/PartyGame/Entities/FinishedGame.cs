using MongoDB.Bson;
using PartyGame.Models.GameModels;

namespace PartyGame.Entities
{
    public class FinishedGame
    {
        public ObjectId Id { get; set; }
        public string Nickname { get; set; }
        public double FinalScore { get; set; }
        public List<Round> Rounds { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }

    }
}
