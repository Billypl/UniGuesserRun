using AutoMapper;
using MediatR;
using UniGuesser.Application.Models.GameModels;
using UniGuesser.Domain.Exceptions;
using UniGuesser.Infrastructure;

namespace UniGuesser.Application.UseCases.GameSessions.GetGameDetails
{
    public class GetGameDetailsHandler(IGameSessionRepository gameSessionRepository, IMapper mapper) : IRequestHandler<GetGameDetailsQuery, FinishedGameDto>
    {
        public async Task<FinishedGameDto> Handle(GetGameDetailsQuery request, CancellationToken cancellationToken)
        {
            var session = await gameSessionRepository.GetAsync(request.Guid);
            if (session is null)
            {
                throw new GameSessionExceptions.GameNotFoundException(request.Guid);
            }
            return mapper.Map<FinishedGameDto>(session);
        }
    }

}
