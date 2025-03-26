using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using PartyGame.Models.GameModels;

namespace PartyGame.Models.PlaceModels
{
    public class ShowPlaceDto
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Coordinates Coordinates { get; set; }
        public string ImageUrl { get; set; }
        public string Alt { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? AuthorId { get; set; }
    }
}
