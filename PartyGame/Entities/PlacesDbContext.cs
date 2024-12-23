using MongoDB.Driver;

namespace PartyGame.Entities
{
    public class PlacesDbContext
    {
        private readonly IMongoDatabase _database;

        public PlacesDbContext(IMongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase("PartyGame"); // Nazwa bazy danych
        }

        public IMongoCollection<Place> Places => _database.GetCollection<Place>("Places");
        public IMongoCollection<GameSession> GameSessions => _database.GetCollection<GameSession>("GameSessions");
    }
}