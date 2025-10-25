using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using UniGuesser.Domain.ValueObjects;

namespace UniGuesser.Application.UseCases.GeolocationGames.GetDistanceFromPlace
{
    public record GetDistanceFromPlaceCommand(Guid gameId, Coordinates playerPosition) : IRequest<double>
    {
        public GetDistanceFromPlaceCommand(string gameId, Coordinates playerPosition) : this(Guid.Parse(gameId), playerPosition) { }
    }
}
