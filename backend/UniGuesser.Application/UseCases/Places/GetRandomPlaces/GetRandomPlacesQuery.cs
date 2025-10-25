using MediatR;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.Application.UseCases.Places.GetRandomPlaces
{
    public record GetRandomPlacesQuery(int RoundsToTake, DifficultyLevel DifficultyLevel) : IRequest<List<Place>>
    {
        public GetRandomPlacesQuery(int roundsToTake, string difficultyLevel) : this(roundsToTake, Enum.Parse<DifficultyLevel>(difficultyLevel, true)) { }
    }
}
