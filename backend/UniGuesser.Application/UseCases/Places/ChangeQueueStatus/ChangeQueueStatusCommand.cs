using MediatR;

namespace UniGuesser.Application.UseCases.Places.ChangeQueueStatus;

public record ChangeQueueStatusCommand(Guid Guid, bool InQueueStatus) : IRequest<Unit>
{
    public ChangeQueueStatusCommand(string guid, bool inQueueStatus) : this(Guid.Parse(guid), inQueueStatus)
    {
    }
}