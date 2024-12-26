using MongoDB.Driver;

namespace PartyGame.Entities
{
    public class PlacesDbContext
    {
        private readonly IMongoDatabase _database;

        public PlacesDbContext(IMongoClient mongoClient)
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
        public IMongoCollection<GameSession> GameSessions => _database.GetCollection<GameSession>("GameSessions");
    }
}