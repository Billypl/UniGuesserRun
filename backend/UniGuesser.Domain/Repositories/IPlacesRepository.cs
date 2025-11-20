using UniGuesser.Domain.Entities;
using UniGuesser.Domain.ValueObjects.Enumerations;

namespace UniGuesser.Domain.Repositories;

public interface IPlacesRepository : IRepository<Place>
{
    Task<long> GetPlacesCount();
    Task<List<Place>> GetPlacesByDifficulty(DifficultyLevel difficultyLevel);
}