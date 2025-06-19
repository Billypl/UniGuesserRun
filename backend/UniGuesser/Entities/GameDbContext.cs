using Microsoft.EntityFrameworkCore;

namespace PartyGame.Entities
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options)
            : base(options)
        {
        }

        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Round> Rounds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Nickname).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired();

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAdd()
                    .HasConversion(
                        v => v.ToUniversalTime(), 
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc) 
                    );

                entity.HasIndex(u => u.Nickname).IsUnique();
               
            });


            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(gs => gs.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(gs => gs.ExpirationDate);
                entity.HasIndex(gs => gs.ActualRoundNumber);

                entity.Property(gs => gs.PublicId)
                    .HasDefaultValueSql("gen_random_uuid()");
                entity.Property(gs => gs.IsFinished)
                    .HasDefaultValue(false);

            });

            // Konfiguracja Place
            modelBuilder.Entity<Place>(entity =>
            {
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(p => p.AuthorId)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.Property(p => p.AuthorId)
               .IsRequired(false);

                entity.HasIndex(p => p.Latitude);
                entity.HasIndex(p => p.Longitude);
                entity.HasIndex(p => p.InQueue);

                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAdd()
                    .HasConversion(
                         v => v.ToUniversalTime(),
                         v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
    );
            });

            modelBuilder.Entity<Round>(entity =>
            {
                entity.HasOne(r => r.GameSession)
                      .WithMany(gs => gs.Rounds)
                      .HasForeignKey(r => r.GameSessionId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.PlaceToGuess)
                      .WithMany()
                      .HasForeignKey(r => r.PlaceId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(r => r.Score);
                entity.HasIndex(r => r.GameSessionId);
            });
        }
    }
}