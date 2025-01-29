using MongoDB.Bson;

namespace PartyGame.Entities
{
    public class Role
    {
        ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}
