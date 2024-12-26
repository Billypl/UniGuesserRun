using System.Runtime.InteropServices.JavaScript;
using MongoDB.Bson.Serialization.Attributes;

namespace PartyGame.Entities
{
    public class GameSession
    {
        [BsonId]
        public long Id { get; set; }
        public string Token { get; set; }
        public List<int> IDsPlaces { get; set; }
        public int ActualRoundNumber { get; set; }

        public DateTime ExpirationDate { get; set;}
    }
}
