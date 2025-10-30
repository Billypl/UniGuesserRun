using AutoMapper;
using MediatR;
using UniGuesser.Domain.Entities;
using UniGuesser.Domain.Repositories;

namespace UniGuesser.Application.UseCases.Places.GetRandomPlaces;

public class GetRadomPlacesHandler(IPlacesRepository placesRepository, IMapper mapper)
    : IRequestHandler<GetRandomPlacesQuery, List<Place>>
{
    public async Task<List<Place>> Handle(GetRandomPlacesQuery request, CancellationToken cancellationToken)
    {
        var placesWithDifficulty = (await placesRepository
                .GetPlacesByDifficulty(request.DifficultyLevel))
            .Where(p => !p.InQueue)
            .ToList();

        if (placesWithDifficulty.Count == 0)
            return new List<Place>();

        var numberOfRoundsToTake = Math.Min(request.RoundsToTake, placesWithDifficulty.Count);

        var random = new Random();
        var randomPlaces = placesWithDifficulty
            .OrderBy(_ => random.Next())
            .Take(numberOfRoundsToTake)
            .Distinct()
            .ToList();

        return randomPlaces;
    }
}