using Microsoft.EntityFrameworkCore;
using PartyGame.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;
using PartyGame.Models.AccountModels;
using PartyGame.Services;

namespace PartyGame.Extensions
{
    public class Seeder
    {
        private readonly GameDbContext _gameDbContext;
        private string _filePath;
        private readonly IAccountService _accountService;

        public Seeder(GameDbContext gameDbContext, IAccountService contextAccessorService)
        {
            _gameDbContext = gameDbContext;
            _filePath = "Data/Places.json";
            _accountService = contextAccessorService;
        }

        public async Task Seed()
        {
            var userCount = await _gameDbContext.Users.CountAsync();
            if (userCount == 0)
            {
                await AddUsers();
            }

            var placeCount = await _gameDbContext.Places.CountAsync();

            if (placeCount == 0)
            {
                var places = GetPlaces();
                foreach (var item in places)
                {
                    item.InQueue = false;
                }
                if (places != null)
                {
                    await _gameDbContext.Places.AddRangeAsync(places);
                    await _gameDbContext.SaveChangesAsync();
                }
            }

            
        }

        private async Task AddUsers()
        {
            var admin = new RegisterUserDto
            {
                Nickname = "Admin",
                Password = "AdminAdmin",
                ConfirmPassword = "AdminAdmin",
                Email = "Admin@Admin.com",
            };

            await _accountService.RegisterUser(admin, "Admin");

            var moderator = new RegisterUserDto
            {
                Nickname = "Moderator",
                Password = "ModeratorModerator",
                ConfirmPassword = "ModeratorModerator",
                Email = "Moderator@Moderator.com"
            };

            await _accountService.RegisterUser(moderator, "Moderator");

            var user = new RegisterUserDto
            {
                Nickname = "User",
                Password = "UserUser",
                ConfirmPassword = "UserUser",
                Email = "User@User.com"
            };
            await _accountService.RegisterUser(user, "User");
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
