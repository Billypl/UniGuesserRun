using System.Runtime.InteropServices.JavaScript;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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

    }
}
