using MediatR;
using UniGuesser.Application.Models.AccountModels;

namespace UniGuesser.Application.UseCases.Accounts.Login
{
    public record LoginCommand(string NicknameOrEmail, string Password): IRequest<LoginResultDto>;
}
