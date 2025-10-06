using AutoMapper;
using MediatR;
using System.Numerics;
using UniGuesser.Adapters.Outbound.Repositories;

namespace UniGuesser.Application.UseCases.Places.GetRandomPlaces
{
    public class GetRadomPlacesHandler(IPlacesRepository placesRepository, IMapper mapper) : IRequestHandler<GetRandomPlacesQuery, List<Place>>
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
            var randomIndexes = new HashSet<int>();

            while (randomIndexes.Count < numberOfRoundsToTake)
            {
                randomIndexes.Add(random.Next(placesWithDifficulty.Count));
            }

            var randomPlaces = randomIndexes.Select(index => placesWithDifficulty[index]).ToList();

            if (randomPlaces.Count < request.RoundsToTake)
                throw new InvalidOperationException("Not enough places were fetched from the database.");

            return randomPlaces;
        }

    }
}
