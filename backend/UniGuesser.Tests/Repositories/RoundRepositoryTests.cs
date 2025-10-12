using Microsoft.EntityFrameworkCore;

namespace UniGuesser.Tests.Repositories
{
    public class RoundRepositoryTests
    {
        private DbContextOptions<GameDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task CreateAsync_ValidRound_ReturnsRound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new RoundRepository(context);

            var place = new Place
            {
                Name = "Test Place",
                Description = "Test Description",
                Latitude = 52.2297,
                Longitude = 21.0122,
                ImageUrl = "http://test.com/image.jpg",
                Alt = "Test Alt",
                DifficultyLevel = "normal",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            };
            await context.Places.AddAsync(place);
            await context.SaveChangesAsync();

            var round = new Round
            {
                Latitude = 52.0,
                Longitude = 21.0,
                Score = 100.0,
                GameSessionId = 1,
                PlaceId = place.Id
            };

            // Act
            var result = await repository.CreateAsync(round);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(round.Latitude, result.Latitude);
            Assert.NotEqual(Guid.Empty, result.PublicId);
        }

        [Fact]
        public async Task GetAsync_ExistingRound_ReturnsRoundWithPlace()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new RoundRepository(context);

            var place = new Place
            {
                Name = "Test Place",
                Description = "Test Description",
                Latitude = 52.2297,
                Longitude = 21.0122,
                ImageUrl = "http://test.com/image.jpg",
                Alt = "Test Alt",
                DifficultyLevel = "normal",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            };
            await context.Places.AddAsync(place);
            await context.SaveChangesAsync();

            var round = new Round
            {
                Latitude = 52.0,
                Longitude = 21.0,
                Score = 100.0,
                GameSessionId = 1,
                PlaceId = place.Id
            };
            var createdRound = await repository.CreateAsync(round);

            // Act
            var result = await repository.GetAsync(createdRound.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdRound.Id, result.Id);
            Assert.NotNull(result.PlaceToGuess);
            Assert.Equal(place.Name, result.PlaceToGuess.Name);
        }

        [Fact]
        public async Task GetAsync_NonExistingRound_ReturnsNull()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new RoundRepository(context);

            // Act
            var result = await repository.GetAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_MultipleRounds_ReturnsAllRoundsWithPlaces()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new RoundRepository(context);

            var place1 = new Place
            {
                Name = "Place 1",
                Description = "Description 1",
                Latitude = 52.2297,
                Longitude = 21.0122,
                ImageUrl = "http://test.com/image1.jpg",
                Alt = "Alt 1",
                DifficultyLevel = "easy",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            };

            var place2 = new Place
            {
                Name = "Place 2",
                Description = "Description 2",
                Latitude = 51.5074,
                Longitude = -0.1278,
                ImageUrl = "http://test.com/image2.jpg",
                Alt = "Alt 2",
                DifficultyLevel = "hard",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            };

            await context.Places.AddAsync(place1);
            await context.Places.AddAsync(place2);
            await context.SaveChangesAsync();

            var round1 = new Round
            {
                Latitude = 52.0,
                Longitude = 21.0,
                Score = 100.0,
                GameSessionId = 1,
                PlaceId = place1.Id
            };

            var round2 = new Round
            {
                Latitude = 51.0,
                Longitude = -0.1,
                Score = 200.0,
                GameSessionId = 1,
                PlaceId = place2.Id
            };

            await repository.CreateAsync(round1);
            await repository.CreateAsync(round2);

            // Act
            var results = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(results);
            Assert.Equal(2, results.Count());
            Assert.All(results, r => Assert.NotNull(r.PlaceToGuess));
        }

        [Fact]
        public async Task UpdateAsync_ExistingRound_UpdatesRound()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new RoundRepository(context);

            var place = new Place
            {
                Name = "Test Place",
                Description = "Test Description",
                Latitude = 52.2297,
                Longitude = 21.0122,
                ImageUrl = "http://test.com/image.jpg",
                Alt = "Test Alt",
                DifficultyLevel = "normal",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            };
            await context.Places.AddAsync(place);
            await context.SaveChangesAsync();

            var round = new Round
            {
                Latitude = 52.0,
                Longitude = 21.0,
                Score = 100.0,
                GameSessionId = 1,
                PlaceId = place.Id
            };
            var createdRound = await repository.CreateAsync(round);

            // Act
            createdRound.Score = 150.0;
            createdRound.Latitude = 53.0;
            await repository.UpdateAsync(createdRound);

            // Assert
            var updatedRound = await repository.GetAsync(createdRound.Id);
            Assert.NotNull(updatedRound);
            Assert.Equal(150.0, updatedRound.Score);
            Assert.Equal(53.0, updatedRound.Latitude);
        }

        [Fact]
        public async Task DeleteAsync_ExistingRound_ReturnsTrue()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new RoundRepository(context);

            var place = new Place
            {
                Name = "Test Place",
                Description = "Test Description",
                Latitude = 52.2297,
                Longitude = 21.0122,
                ImageUrl = "http://test.com/image.jpg",
                Alt = "Test Alt",
                DifficultyLevel = "normal",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            };
            await context.Places.AddAsync(place);
            await context.SaveChangesAsync();

            var round = new Round
            {
                Latitude = 52.0,
                Longitude = 21.0,
                Score = 100.0,
                GameSessionId = 1,
                PlaceId = place.Id
            };
            var createdRound = await repository.CreateAsync(round);

            // Act
            var result = await repository.DeleteAsync(createdRound.Id);

            // Assert
            Assert.True(result);
            var deletedRound = await repository.GetAsync(createdRound.Id);
            Assert.Null(deletedRound);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingRound_ReturnsFalse()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new RoundRepository(context);

            // Act
            var result = await repository.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetByPublicIdAsync_ExistingRound_ReturnsRoundWithPlace()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new RoundRepository(context);

            var place = new Place
            {
                Name = "Test Place",
                Description = "Test Description",
                Latitude = 52.2297,
                Longitude = 21.0122,
                ImageUrl = "http://test.com/image.jpg",
                Alt = "Test Alt",
                DifficultyLevel = "normal",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            };
            await context.Places.AddAsync(place);
            await context.SaveChangesAsync();

            var round = new Round
            {
                Latitude = 52.0,
                Longitude = 21.0,
                Score = 100.0,
                GameSessionId = 1,
                PlaceId = place.Id
            };
            var createdRound = await repository.CreateAsync(round);

            // Act
            var result = await repository.GetByPublicIdAsync(createdRound.PublicId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdRound.PublicId, result.PublicId);
            Assert.NotNull(result.PlaceToGuess);
            Assert.Equal(place.Name, result.PlaceToGuess.Name);
        }

        [Fact]
        public async Task GetByPublicIdAsync_WithString_ExistingRound_ReturnsRoundWithPlace()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new RoundRepository(context);

            var place = new Place
            {
                Name = "Test Place",
                Description = "Test Description",
                Latitude = 52.2297,
                Longitude = 21.0122,
                ImageUrl = "http://test.com/image.jpg",
                Alt = "Test Alt",
                DifficultyLevel = "normal",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            };
            await context.Places.AddAsync(place);
            await context.SaveChangesAsync();

            var round = new Round
            {
                Latitude = 52.0,
                Longitude = 21.0,
                Score = 100.0,
                GameSessionId = 1,
                PlaceId = place.Id
            };
            var createdRound = await repository.CreateAsync(round);

            // Act
            var result = await repository.GetByPublicIdAsync(createdRound.PublicId.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdRound.PublicId, result.PublicId);
            Assert.NotNull(result.PlaceToGuess);
            Assert.Equal(place.Name, result.PlaceToGuess.Name);
        }
    }
}
