using Microsoft.EntityFrameworkCore;

namespace UniGuesser.Tests.Repositories
{
    public class PlacesRepositoryTests
    {
        private DbContextOptions<GameDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task CreateAsync_ValidPlace_ReturnsPlace()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new PlacesRepository(context);

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

            // Act
            var result = await repository.CreateAsync(place);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(place.Name, result.Name);
            Assert.NotEqual(Guid.Empty, result.PublicId);
        }

        [Fact]
        public async Task GetAsync_ExistingPlace_ReturnsPlace()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new PlacesRepository(context);

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

            var createdPlace = await repository.CreateAsync(place);

            // Act
            var result = await repository.GetAsync(createdPlace.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdPlace.Name, result.Name);
            Assert.Equal(createdPlace.Id, result.Id);
        }

        [Fact]
        public async Task GetAsync_NonExistingPlace_ReturnsNull()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new PlacesRepository(context);

            // Act
            var result = await repository.GetAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_MultiplePlaces_ReturnsAllPlaces()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new PlacesRepository(context);

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

            await repository.CreateAsync(place1);
            await repository.CreateAsync(place2);

            // Act
            var results = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(results);
            Assert.Equal(2, results.Count());
        }

        [Fact]
        public async Task DeleteAsync_ExistingPlace_ReturnsTrue()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new PlacesRepository(context);

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

            var createdPlace = await repository.CreateAsync(place);

            // Act
            var result = await repository.DeleteAsync(createdPlace.Id);

            // Assert
            Assert.True(result);
            var deletedPlace = await repository.GetAsync(createdPlace.Id);
            Assert.Null(deletedPlace);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingPlace_ReturnsFalse()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new PlacesRepository(context);

            // Act
            var result = await repository.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetByPublicIdAsync_ExistingPlace_ReturnsPlace()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new PlacesRepository(context);

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

            var createdPlace = await repository.CreateAsync(place);

            // Act
            var result = await repository.GetByPublicIdAsync(createdPlace.PublicId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdPlace.PublicId, result.PublicId);
            Assert.Equal(createdPlace.Name, result.Name);
        }

        [Fact]
        public async Task GetByPublicIdAsync_WithString_ExistingPlace_ReturnsPlace()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new PlacesRepository(context);

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

            var createdPlace = await repository.CreateAsync(place);

            // Act
            var result = await repository.GetByPublicIdAsync(createdPlace.PublicId.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdPlace.PublicId, result.PublicId);
            Assert.Equal(createdPlace.Name, result.Name);
        }

        [Fact]
        public async Task GetPlacesCount_MultiplePlaces_ReturnsCorrectCount()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new PlacesRepository(context);

            await repository.CreateAsync(new Place
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
            });

            await repository.CreateAsync(new Place
            {
                Name = "Place 2",
                Description = "Description 2",
                Latitude = 51.5074,
                Longitude = -0.1278,
                ImageUrl = "http://test.com/image2.jpg",
                Alt = "Alt 2",
                DifficultyLevel = "normal",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            });

            // Act
            var count = await repository.GetPlacesCount();

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetPlacesByDifficulty_EasyDifficulty_ReturnsOnlyEasyPlaces()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new PlacesRepository(context);

            await repository.CreateAsync(new Place
            {
                Name = "Easy Place",
                Description = "Description 1",
                Latitude = 52.2297,
                Longitude = 21.0122,
                ImageUrl = "http://test.com/image1.jpg",
                Alt = "Alt 1",
                DifficultyLevel = "easy",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            });

            await repository.CreateAsync(new Place
            {
                Name = "Medium Place",
                Description = "Description 2",
                Latitude = 51.5074,
                Longitude = -0.1278,
                ImageUrl = "http://test.com/image2.jpg",
                Alt = "Alt 2",
                DifficultyLevel = "normal",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            });

            await repository.CreateAsync(new Place
            {
                Name = "Hard Place",
                Description = "Description 3",
                Latitude = 40.7128,
                Longitude = -74.0060,
                ImageUrl = "http://test.com/image3.jpg",
                Alt = "Alt 3",
                DifficultyLevel = "hard",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            });

            // Act
            var easyPlaces = await repository.GetPlacesByDifficulty(DifficultyLevel.easy);

            // Assert
            Assert.Single(easyPlaces);
            Assert.Equal("Easy Place", easyPlaces[0].Name);
        }

        [Fact]
        public async Task UpdateAsync_ExistingPlace_UpdatesPlace()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new PlacesRepository(context);

            var place = new Place
            {
                Name = "Original Name",
                Description = "Original Description",
                Latitude = 52.2297,
                Longitude = 21.0122,
                ImageUrl = "http://test.com/image.jpg",
                Alt = "Test Alt",
                DifficultyLevel = "normal",
                InQueue = false,
                CreatedAt = DateTime.UtcNow
            };

            var createdPlace = await repository.CreateAsync(place);

            // Act
            createdPlace.Name = "Updated Name";
            createdPlace.Description = "Updated Description";
            await repository.UpdateAsync(createdPlace);

            // Assert
            var updatedPlace = await repository.GetAsync(createdPlace.Id);
            Assert.NotNull(updatedPlace);
            Assert.Equal("Updated Name", updatedPlace.Name);
            Assert.Equal("Updated Description", updatedPlace.Description);
        }
    }
}
