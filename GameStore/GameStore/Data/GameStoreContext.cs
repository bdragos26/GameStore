using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Data
{
    public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
    {
        public DbSet<GameDetails> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Fighting" },
                new Genre { Id = 2, Name = "Adventure" },
                new Genre { Id = 3, Name = "Platformer" },
                new Genre { Id = 4, Name = "Shooter" },
                new Genre { Id = 5, Name = "RPG" },
                new Genre { Id = 6, Name = "Sandbox" },
                new Genre { Id = 7, Name = "Action" },
                new Genre { Id = 8, Name = "Action RPG" },
                new Genre { Id = 9, Name = "Puzzle" },
                new Genre { Id = 10, Name = "Strategy" }
            );

            modelBuilder.Entity<GameDetails>()
                .HasOne(g => g.Genre)
                .WithMany()
                .HasForeignKey(g => g.GenreId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
