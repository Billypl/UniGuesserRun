using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Application.UseCases.Accounts.Register;
using UniGuesser.Domain.Entities;
using UniGuesser.Infrastructure.Persistence;

namespace UniGuesser.API.Extensions
{
    public class Seeder
    {
        private readonly GameDbContext _gameDbContext;
        private string _filePath;
        private IMediator _mediator;

        public Seeder(GameDbContext gameDbContext, IMediator mediator)
        {
            _gameDbContext = gameDbContext;
            _filePath = Path.Combine(AppContext.BaseDirectory, "Data", "Places.json");
            _mediator = mediator;
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
                Role = "Admin"
            };

            var commandAdmin = new RegisterAccountCommand(admin);
            await _mediator.Send(commandAdmin);

            var moderator = new RegisterUserDto
            {
                Nickname = "Moderator",
                Password = "ModeratorModerator",
                ConfirmPassword = "ModeratorModerator",
                Email = "Moderator@Moderator.com",
                Role = "Moderator"
            };

            var commandModerator = new RegisterAccountCommand(moderator);
            await _mediator.Send(commandModerator);

            var user = new RegisterUserDto
            {
                Nickname = "User",
                Password = "UserUser",
                ConfirmPassword = "UserUser",
                Email = "User@User.com"
            };

            var commandUser = new RegisterAccountCommand(user);
            await _mediator.Send(commandUser);
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
