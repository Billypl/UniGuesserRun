using MongoDB.Driver;

namespace PartyGame.Entities
{
    public class GameDbContext
    {
        private readonly IMongoDatabase _database;

        public GameDbContext(IMongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase("PartyGame"); 

            var collection = _database.GetCollection<GameSession>("GameSessions");

            var indexOptions = new CreateIndexOptions
            {
                ExpireAfter = TimeSpan.Zero 
            };

            var indexKeys = Builders<GameSession>.IndexKeys.Ascending(x => x.ExpirationDate);
            var indexModel = new CreateIndexModel<GameSession>(indexKeys, indexOptions);

            collection.Indexes.CreateOne(indexModel);

        }

        public IMongoCollection<Place> Places => _database.GetCollection<Place>("Places");
        public IMongoCollection<PlaceToCheck> PlacesToCheck => _database.GetCollection<PlaceToCheck>("PlacesToCheckRepository");
        public IMongoCollection<GameSession> GameSessions => _database.GetCollection<GameSession>("GameSessions");
        public IMongoCollection<FinishedGame> GameResults => _database.GetCollection<FinishedGame>("GameResults");
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    }
}