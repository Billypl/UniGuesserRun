using System.Runtime.InteropServices.JavaScript;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using PartyGame.Models.GameModels;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DifficultyLevel
{
    [JsonPropertyName("easy")]
    Easy,

    [JsonPropertyName("normal")]
    Normal,

    [JsonPropertyName("hard")]
    Hard
}

namespace PartyGame.Entities
{
    public class GameSession
    {
        public ObjectId Id { get; set; }
        public string Token { get; set; }
        public List<Round> Rounds { get; set; }
        public DateTime ExpirationDate { get; set;}
        public int ActualRoundNumber { get; set; }
        public double GameScore { get; set; }
        public string Nickname { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }

    }
}
