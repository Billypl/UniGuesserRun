using AutoMapper;
using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Repositories
{
    public interface IGameSessionRepository:IRepository<GameSession>
    {
    
    }

    public class GameSessionRepository : Repository<GameSession>,IGameSessionRepository
    {

        public GameSessionRepository(GameDbContext gameDbContext):base(gameDbContext.Database,"GameSessions")
        {
         
        }


    }
}
