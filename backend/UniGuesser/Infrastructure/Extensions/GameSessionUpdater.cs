using Microsoft.EntityFrameworkCore;
using Repositories.Repositories;
using UniGuesser.Application.Models.Enumerations;
using UniGuesser.Infrastructure.Persistence;

public class GameSessionUpdater : Repository<GameSession>
    {
        public GameSessionUpdater(GameDbContext context) : base(context)
        {
        }

        public void updateOverdueGameSessions()
        {
            var now = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

            IQueryable<GameSession> gameSessions = _dbSet
                .Where(gs => gs.ExpirationDate <= now)
                .Include(gs => gs.Player);

            foreach (var session in gameSessions)
            {
                if (session.Player == null)
                {
                    _dbSet.Remove(session); 
                }
                else
                {
                    session.GameState = GameStatus.Abandoned;
                    _dbSet.Update(session); 
                }
            }

            _context.SaveChanges();
        }
    }


public class GameSessionBackgroundUpdater : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public GameSessionBackgroundUpdater(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var gameUpdater = scope.ServiceProvider.GetRequiredService<GameSessionUpdater>();
                    gameUpdater.updateOverdueGameSessions(); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Updater error] {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
