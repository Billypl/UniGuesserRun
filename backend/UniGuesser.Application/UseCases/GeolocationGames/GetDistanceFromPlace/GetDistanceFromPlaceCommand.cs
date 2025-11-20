using MediatR;
using UniGuesser.Domain.ValueObjects;

namespace UniGuesser.Application.UseCases.GeolocationGames.GetDistanceFromPlace;

public record GetDistanceFromPlaceCommand(Guid gameId, Coordinates playerPosition) : IRequest<double>
{
    public GetDistanceFromPlaceCommand(string gameId, Coordinates playerPosition) : this(Guid.Parse(gameId),
        playerPosition)
    {
    }
}