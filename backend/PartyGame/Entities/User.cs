using MongoDB.Bson;

namespace PartyGame.Entities
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Nickname { get;set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Role { get; set; } = "User";
    }
}
