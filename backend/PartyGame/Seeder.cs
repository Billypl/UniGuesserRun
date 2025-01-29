using Microsoft.EntityFrameworkCore;
using PartyGame.Entities;
using System.Text.Json;
using MongoDB.Driver;
using System.Text.Json.Serialization;
using System.Data;

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

            var roleCount = await _gameDbContext.Roles.CountDocumentsAsync(Builders<Role>.Filter.Empty);

            if (roleCount == 0)
            {
                var roles = GetRoles();
                if (roles != null)
                {
                    await _gameDbContext.Roles.InsertManyAsync(roles);
                }
            }
        }



        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "User"
                },
                new Role()
                {
                    Name = "Moderator"
                }
                ,
                new Role()
                {
                    Name = "Admin"
                }
            };

            return roles;
        }

        private IEnumerable<Place>? GetPlaces()
        {
            string jsonString = File.ReadAllText(_filePath);

            var places = JsonSerializer.Deserialize<List<Place>>(jsonString, new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            });

            return places;
        }
    }
}
