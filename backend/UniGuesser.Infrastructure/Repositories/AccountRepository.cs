using Microsoft.EntityFrameworkCore;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Repositories;
using UniGuesser.Infrastructure.Persistence;

namespace UniGuesser.Infrastructure.Repositories;

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