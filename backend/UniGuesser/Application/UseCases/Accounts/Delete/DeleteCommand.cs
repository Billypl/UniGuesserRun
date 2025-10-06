using MediatR;
using UniGuesser.Application.Models.AccountModels;

namespace UniGuesser.Application.UseCases.Accounts.Delete
{
    public record DeleteCommand(Guid Guid) : IRequest<Unit>
    {
        public DeleteCommand(string id) : this(Guid.Parse(id)) { }
    }
}
