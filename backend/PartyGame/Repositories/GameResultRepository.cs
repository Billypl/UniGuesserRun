using MongoDB.Bson;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Models.ScoreboardModels;
using SortDirection = PartyGame.Models.ScoreboardModels.SortDirection;


namespace PartyGame.Repositories
{
    public interface IScoreboardRepository : IRepository<FinishedGame>
    {
        Task<List<FinishedGame>> GetGames(ScoreboardQuery scoreboardQuery);
       
    }

    public class GameResultRepository :Repository<FinishedGame>,IScoreboardRepository
    {
  
        public GameResultRepository(GameDbContext gameDbContext,string documentName):base(gameDbContext.Database,documentName )
        {
        }

        public async Task<List<FinishedGame>> GetGames(ScoreboardQuery scoreboardQuery)
        {
            var filterBuilder = Builders<FinishedGame>.Filter;
            var filter = FilterDefinition<FinishedGame>.Empty;

            // Filtracja po difficulty level
            if (scoreboardQuery.DifficultyLevel is not null)
            {
                filter &= filterBuilder.Regex(r => r.DifficultyLevel, new MongoDB.Bson.BsonRegularExpression(scoreboardQuery.DifficultyLevel.ToString(), "i"));
            }

            // Filtracja po SearchNickname
            if (!string.IsNullOrEmpty(scoreboardQuery.SearchNickname))
            {
                filter &= filterBuilder.Regex(r => r.Nickname, new MongoDB.Bson.BsonRegularExpression(scoreboardQuery.SearchNickname, "i"));
            }

            // Sortowanie po GameScore
            var sortDirection = scoreboardQuery.SortDirection == SortDirection.ASC
                ? Builders<FinishedGame>.Sort.Ascending(r => r.FinalScore)
                : Builders<FinishedGame>.Sort.Descending(r => r.FinalScore);

            // Paginacja
            var pagedScores = await Collection.Find(filter)
                .Sort(sortDirection)
                .Skip((scoreboardQuery.PageNumber - 1) * scoreboardQuery.PageSize)
                .Limit(scoreboardQuery.PageSize)
                .ToListAsync();

            return pagedScores;
        }
    }
}
