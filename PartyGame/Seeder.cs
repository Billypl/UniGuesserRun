using Microsoft.EntityFrameworkCore;
using PartyGame.Entities;
using System.Text.Json;

namespace PartyGame
{
    public class Seeder
    {
        private readonly PlacesDbContext _dbContext;
        private string _filePath;
        public Seeder(PlacesDbContext dbContext )
        {
            _dbContext = dbContext;
            _filePath = "Data/Places.json";
        }


        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Places.Any())
                {
                    var places = GetPlaces();
                    _dbContext.Places.AddRange( places );
                    _dbContext.SaveChanges();
                }
            }
        }


        private IEnumerable<Place> GetPlaces()
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
