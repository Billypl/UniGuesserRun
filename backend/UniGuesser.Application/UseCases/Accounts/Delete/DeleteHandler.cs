using MediatR;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Domain.Repositories;

namespace UniGuesser.Application.UseCases.Accounts.Delete;

public class DeleteHandler(IAccountRepository accountRepository) : IRequestHandler<DeleteCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCommand deleteCommand, CancellationToken cancellationToken)
    {
        var user = await accountRepository.GetAsync(deleteCommand.Guid);

        if (user is null) throw new AccountExceptions.UserNotFoundException(deleteCommand.Guid);
        await accountRepository.DeleteAsync(user.Id);

        return Unit.Value;
    }
}