using Microsoft.EntityFrameworkCore;
using PartyGame.Entities;
using System.Text.Json;
using MongoDB.Driver;

namespace PartyGame
{
    public class Seeder
    {
        private readonly GameDbContext _gameDbContext;
        private string _filePath;
        public Seeder(GameDbContext gameDbContext)
        {
            _gameDbContext = gameDbContext;
            _filePath = "Data/Places.json";
        }

        public async Task Seed()
        {
            var placeCount = await _gameDbContext.Places.CountDocumentsAsync(Builders<Place>.Filter.Empty);

            if (placeCount == 0)
            {
                var places = GetPlaces();
                if (places != null)
                {
                    await _gameDbContext.Places.InsertManyAsync(places);
                }
            }
        }

        private IEnumerable<Place>? GetPlaces()
        {
            string jsonString = File.ReadAllText(_filePath);

            var places = JsonSerializer.Deserialize<List<Place>>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return places;
        }
    }
}
