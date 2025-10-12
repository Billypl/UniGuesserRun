using MediatR;
using UniGuesser.Application.Models.AccountModels;

namespace UniGuesser.Application.UseCases.Accounts.AccountDetails
{
    public record AccountDetailsQuery(Guid Id) : IRequest<AccountDetailsDto>
    {
        public AccountDetailsQuery(string Id) : this(Guid.Parse(Id)) { }
    };
}
