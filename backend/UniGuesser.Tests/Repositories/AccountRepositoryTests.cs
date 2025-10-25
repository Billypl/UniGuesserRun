using Microsoft.EntityFrameworkCore;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.ValueObjects;
using UniGuesser.Infrastructure.Persistence;
using UniGuesser.Infrastructure.Repositories;

namespace UniGuesser.Tests.Repositories
{
    public class AccountRepositoryTests
    {
        private DbContextOptions<GameDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task CreateAsync_ValidUser_ReturnsUser()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            var user = new User
            {
                Nickname = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword123",
                CreatedAt = DateTime.UtcNow,
                Role = UserRoles.User
            };

            // Act
            var result = await repository.CreateAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Nickname, result.Nickname);
            Assert.Equal(user.Email, result.Email);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task GetAsync_ExistingUser_ReturnsUser()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            var user = new User
            {
                Nickname = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword123",
                CreatedAt = DateTime.UtcNow,
                Role = UserRoles.User
            };
            var createdUser = await repository.CreateAsync(user);

            // Act
            var result = await repository.GetAsync(createdUser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdUser.Nickname, result.Nickname);
            Assert.Equal(createdUser.Id, result.Id);
        }

        [Fact]
        public async Task GetAsync_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            // Act
            var result = await repository.GetAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_MultipleUsers_ReturnsAllUsers()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            var user1 = new User
            {
                Nickname = "User1",
                Email = "user1@example.com",
                PasswordHash = "hash1",
                CreatedAt = DateTime.UtcNow,
                Role = UserRoles.User
            };

            var user2 = new User
            {
                Nickname = "User2",
                Email = "user2@example.com",
                PasswordHash = "hash2",
                CreatedAt = DateTime.UtcNow,
                Role = UserRoles.User
            };

            await repository.CreateAsync(user1);
            await repository.CreateAsync(user2);

            // Act
            var results = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(results);
            Assert.Equal(2, results.Count());
        }

        [Fact]
        public async Task UpdateAsync_ExistingUser_UpdatesUser()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            var user = new User
            {
                Nickname = "OriginalNickname",
                Email = "original@example.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                Role = UserRoles.User
            };
            var createdUser = await repository.CreateAsync(user);

            // Act
            createdUser.Nickname = "UpdatedNickname";
            createdUser.Email = "updated@example.com";
            await repository.UpdateAsync(createdUser);

            // Assert
            var updatedUser = await repository.GetAsync(createdUser.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal("UpdatedNickname", updatedUser.Nickname);
            Assert.Equal("updated@example.com", updatedUser.Email);
        }

        [Fact]
        public async Task DeleteAsync_ExistingUser_ReturnsTrue()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            var user = new User
            {
                Nickname = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                Role = UserRoles.User
            };
            var createdUser = await repository.CreateAsync(user);

            // Act
            var result = await repository.DeleteAsync(createdUser.Id);

            // Assert
            Assert.True(result);
            var deletedUser = await repository.GetAsync(createdUser.Id);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingUser_ReturnsFalse()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            // Act
            var result = await repository.DeleteAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetByPublicIdAsync_ExistingUser_ReturnsUser()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            var user = new User
            {
                Nickname = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                Role = UserRoles.User
            };
            var createdUser = await repository.CreateAsync(user);

            // Act
            var result = await repository.GetAsync(createdUser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdUser.Id, result.Id);
            Assert.Equal(createdUser.Nickname, result.Nickname);
        }

        [Fact]
        public async Task GetByPublicIdAsync_WithString_ExistingUser_ReturnsUser()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            var user = new User
            {
                Nickname = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                Role = UserRoles.User
            };
            var createdUser = await repository.CreateAsync(user);

            // Act
            var result = await repository.GetAsync(createdUser.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdUser.Id, result.Id);
            Assert.Equal(createdUser.Nickname, result.Nickname);
        }

        [Fact]
        public async Task AddNewUsersAsync_MultipleUsers_AddsAllUsers()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            var users = new List<User>
            {
                new User
                {
                    Nickname = "User1",
                    Email = "user1@example.com",
                    PasswordHash = "hash1",
                    CreatedAt = DateTime.UtcNow,
                    Role = UserRoles.User
                },
                new User
                {
                    Nickname = "User2",
                    Email = "user2@example.com",
                    PasswordHash = "hash2",
                    CreatedAt = DateTime.UtcNow,
                    Role = UserRoles.User
                },
                new User
                {
                    Nickname = "User3",
                    Email = "user3@example.com",
                    PasswordHash = "hash3",
                    CreatedAt = DateTime.UtcNow,
                    Role = UserRoles.User
                }
            };

            // Act
            await repository.AddNewUsersAsync(users);

            // Assert
            var allUsers = await repository.GetAllAsync();
            Assert.Equal(3, allUsers.Count());
        }

        [Fact]
        public async Task GetUserByNicknameOrEmailAsync_ByNickname_ReturnsUser()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            var user = new User
            {
                Nickname = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                Role = UserRoles.User
            };
            await repository.CreateAsync(user);

            // Act
            var result = await repository.GetUserByNicknameOrEmailAsync("TestUser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestUser", result.Nickname);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task GetUserByNicknameOrEmailAsync_ByEmail_ReturnsUser()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            var user = new User
            {
                Nickname = "TestUser",
                Email = "test@example.com",
                PasswordHash = "hash",
                CreatedAt = DateTime.UtcNow,
                Role = UserRoles.User
            };
            await repository.CreateAsync(user);

            // Act
            var result = await repository.GetUserByNicknameOrEmailAsync("test@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestUser", result.Nickname);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task GetUserByNicknameOrEmailAsync_NonExisting_ReturnsNull()
        {
            // Arrange
            var options = CreateNewContextOptions();
            using var context = new GameDbContext(options);
            var repository = new AccountRepository(context);

            // Act
            var result = await repository.GetUserByNicknameOrEmailAsync("nonexistent");

            // Assert
            Assert.Null(result);
        }
    }
}
