using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Repositories
{
    public interface IScoreboardRepository
    {
        void AddNewGame(FinishedGame newFinishedGame);
        Task<List<FinishedGame>> GetAllGames();
    }

    public class ScoreboardRepository : IScoreboardRepository
    {

        private readonly GameDbContext _gameDbContext;

        public ScoreboardRepository(GameDbContext gameDbContext)
        {
            _gameDbContext = gameDbContext;
        }

        public void AddNewGame(FinishedGame newFinishedGame)
        {
            _gameDbContext.GameResults.InsertOneAsync(newFinishedGame);
        }

        public async Task<List<FinishedGame>> GetAllGames()
        {
           return await _gameDbContext.GameResults.Find(FilterDefinition<FinishedGame>.Empty).ToListAsync();
        }
    }
}
