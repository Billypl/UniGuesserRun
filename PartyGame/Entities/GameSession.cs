using MongoDB.Bson.Serialization.Attributes;

namespace PartyGame.Entities
{
    public class GameSession
    {
        [BsonId]
        public long Id { get; set; }
        public string Token { get; set; }
        public Place Place { get; set; }
    }
}
