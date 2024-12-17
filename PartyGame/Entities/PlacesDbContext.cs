using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PartyGame.Entities
{
    public class PlacesDbContext:DbContext
    {
        public DbSet<Place> Places { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Place>()
                .Property(p => p.Id)
                .IsRequired();

            modelBuilder.Entity<Place>()
                .OwnsOne(p => p.Coordinates);  

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=9090;Database=PartyGame;Username=postgres;Password=1234");
        }
    }
}
