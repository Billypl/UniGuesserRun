using MediatR;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Domain.ValueObjects;

namespace UniGuesser.Application.UseCases.Accounts.Register
{
    public record RegisterAccountCommand
        : IRequest<Unit>
    {
        public string Nickname { get; init; }
        public string Email { get; init; }
        public string Password { get; init; }
        public string ConfirmPassword { get; init; }
        public UserRoles Role { get; init; } = UserRoles.User;

        public RegisterAccountCommand(RegisterUserDto registerUserDto)
        {
            Nickname = registerUserDto.Nickname;
            Email = registerUserDto.Email;
            Password = registerUserDto.Password;
            ConfirmPassword = registerUserDto.ConfirmPassword;

            if (!Enum.TryParse<UserRoles>(registerUserDto.Role, true, out var parsedRole))
            {
                parsedRole = UserRoles.User;
            }
            Role = parsedRole;
        }
    }
}
