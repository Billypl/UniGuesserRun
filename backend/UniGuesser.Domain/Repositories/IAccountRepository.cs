using UniGuesser.Domain.Entities;

namespace UniGuesser.Domain.Repositories
{
    public interface IAccountRepository : IRepository<User>
    {
        Task AddNewUsersAsync(IEnumerable<User> newUsers);
        Task<User?> GetUserByNicknameOrEmailAsync(string nicknameOrEmail);

    }
}
