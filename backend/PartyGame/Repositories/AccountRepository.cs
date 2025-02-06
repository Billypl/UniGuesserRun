using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Repositories
{
    public interface IAccountRepository
    {
        void AddNewUser(User newUser);
        void DeleteUser(User existingUser);
        void AddNewUsers(IEnumerable<User> newUsers);
        Task<User> GetUserByNicknameOrEmailAsync(string nicknameOrEmail);
        Task<User> GetUserById(string Id);
    }

    public class AccountRepository : IAccountRepository
    {
        private readonly GameDbContext _gameDbContext;

        public AccountRepository(GameDbContext gameDbContext, IMapper mapper)
        {
            _gameDbContext = gameDbContext;
        }

        public async void AddNewUser(User newUser)
        {
            await _gameDbContext.Users.InsertOneAsync(newUser);
        }

        public async void DeleteUser(User existingUser)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, existingUser.Id);
            _ = await _gameDbContext.Users.DeleteOneAsync(filter);
        }
        public async void AddNewUsers(IEnumerable<User> newUsers)
        {
            await _gameDbContext.Users.InsertManyAsync(newUsers);
        }

        public async Task<User> GetUserByNicknameOrEmailAsync(string nicknameOrEmail)
        {
            var filter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Eq(u => u.Nickname, nicknameOrEmail),
                Builders<User>.Filter.Eq(u => u.Email, nicknameOrEmail)
            );

            return await _gameDbContext.Users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserById(string id)
        {
            var filter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Eq(u => u.Id, ObjectId.Parse(id))
            );

            return await _gameDbContext.Users.Find(filter).FirstOrDefaultAsync();
        }


    }
}