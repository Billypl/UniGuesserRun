using MediatR;
using Microsoft.AspNetCore.Identity;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Application.UseCases.Accounts.Delete;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Middleware.Exceptions;

namespace UniGuesser.Application.UseCases.Accounts.Register
{

    public class RegisterHandler(IAccountRepository accountRepository, IPasswordHasher<User> passwordHasher) : IRequestHandler<RegisterAccountCommand,Unit>
    {


        public async Task<Unit> Handle(RegisterAccountCommand registerAccountCommand, CancellationToken cancellationToken)
        {
            if (await accountRepository.GetUserByNicknameOrEmailAsync(registerAccountCommand.Nickname) is not null)
            {
                throw new AccountExceptions.NicknameUsedException(registerAccountCommand.Nickname);
            }


            if (await accountRepository.GetUserByNicknameOrEmailAsync(registerAccountCommand.Email) is not null)
            {
                throw new AccountExceptions.EmailUsedException(registerAccountCommand.Email);
            }

            var newUser = new User()
            {
                Email = registerAccountCommand.Email,
                Nickname = registerAccountCommand.Nickname,
                CreatedAt = DateTime.Now,
                Role = registerAccountCommand.Role.ToString(),
            };

            var passwordHash = passwordHasher.HashPassword(newUser, registerAccountCommand.Password);
            newUser.PasswordHash = passwordHash;


            await accountRepository.CreateAsync(newUser);

            return Unit.Value;
        }

    }
}
