using AutoMapper;
using MediatR;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Application.Models.AccountModels;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Middleware.Exceptions;

namespace UniGuesser.Application.UseCases.Accounts.AccountDetails
{

    public class AccountDetailsHandler(IAccountRepository accountRepository, IMapper mapper) : IRequestHandler<AccountDetailsQuery, AccountDetailsDto>
    {
        public async Task<AccountDetailsDto> Handle(AccountDetailsQuery request, CancellationToken cancellationToken)
        {
            var account = await accountRepository.GetByPublicIdAsync(request.Id.ToString());

            if (account == null)
            {
                throw new AccountExceptions.UserNotFoundException(request.Id);
            }

            return mapper.Map<AccountDetailsDto>(account);
        }
    }

}
