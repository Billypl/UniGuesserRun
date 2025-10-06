using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories;
using UniGuesser.Domain.Entities;
using UniGuesser.Infrastructure.Persistence;

namespace UniGuesser.Adapters.Outbound.Repositories
{
    public interface IAccountRepository : IRepository<User>
    {
        Task AddNewUsersAsync(IEnumerable<User> newUsers);
        Task<User?> GetUserByNicknameOrEmailAsync(string nicknameOrEmail);

    }

    public class AccountRepository : Repository<User>, IAccountRepository
    {
        public AccountRepository(GameDbContext gameDbContext) : base(gameDbContext)
        {
        }

        public async Task AddNewUsersAsync(IEnumerable<User> newUsers)
        {
            await _dbSet.AddRangeAsync(newUsers); // Add multiple users
            await _context.SaveChangesAsync(); // Save changes
        }

        public async Task<User?> GetUserByNicknameOrEmailAsync(string nicknameOrEmail)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Nickname == nicknameOrEmail || u.Email == nicknameOrEmail);
        }
    }
}
