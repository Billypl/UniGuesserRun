using MediatR;
using UniGuesser.Adapters.Outbound.Repositories;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Middleware.Exceptions;

namespace UniGuesser.Application.UseCases.Accounts.Delete
{

    public class DeleteHandler(IAccountRepository accountRepository) : IRequestHandler<DeleteCommand,Unit>
    {

        public async Task<Unit> Handle(DeleteCommand deleteCommand, CancellationToken cancellationToken)
        {
            User? user = await accountRepository.GetByPublicIdAsync(deleteCommand.Guid);

            if (user is null)
            {
                throw new AccountExceptions.UserNotFoundException(deleteCommand.Guid);
            }
            await accountRepository.DeleteAsync(user.Id);

            return Unit.Value;
        }

    }
}
