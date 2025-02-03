using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PartyGame.Models.AccountModels;
using PartyGame.Models.PlaceModels;

namespace PartyGame.Entities
{
    public class PlaceToCheck
    { 
        public ObjectId Id { get; set; }
        public NewPlaceDto NewPlace { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
