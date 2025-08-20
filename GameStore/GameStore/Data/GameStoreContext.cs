using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data
{
    public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GameRating> Ratings { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>().HasData(
                new Genre { GenreId = 1, Name = "Fighting" },
                new Genre { GenreId = 2, Name = "Adventure" },
                new Genre { GenreId = 3, Name = "Platformer" },
                new Genre { GenreId = 4, Name = "Shooter" },
                new Genre { GenreId = 5, Name = "RPG" },
                new Genre { GenreId = 6, Name = "Sandbox" },
                new Genre { GenreId = 7, Name = "Action" },
                new Genre { GenreId = 8, Name = "Action RPG" },
                new Genre { GenreId = 9, Name = "Puzzle" },
                new Genre { GenreId = 10, Name = "Strategy" }
            );

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Genre)
                .WithMany()
                .HasForeignKey(g => g.GenreId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<GameRating>()
                .HasKey(r => new { r.UserId, r.GameId });

            modelBuilder.Entity<GameRating>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<GameRating>()
                .HasOne(r => r.GameDetails)
                .WithMany()
                .HasForeignKey(r => r.GameId);

            modelBuilder.Entity<PasswordResetToken>()
                .HasIndex(t => t.Token)
                .IsUnique();

            modelBuilder.Entity<Game>()
                .Property(g => g.ImageData)
                .HasColumnType("varbinary(max)");
        }
    }
}
