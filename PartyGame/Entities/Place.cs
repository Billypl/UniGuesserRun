using MongoDB.Bson.Serialization.Attributes;

namespace PartyGame.Entities
{
    public class Place
    {
        [BsonId]
        public int Id { get; set; } 
        public string Name { get; set; } 
        public string Description { get; set; } 
        public Coordinates Coordinates { get; set; }
        public string ImageUrl { get; set; } 
    }
}
