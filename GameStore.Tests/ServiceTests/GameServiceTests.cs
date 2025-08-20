using GameStore.Data;
using GameStore.Services;
using GameStore.Shared.DTOs;
using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GameStore.Tests.ServiceTests
{
    public class GameServiceTests
    {
        private DbContextOptions<GameStoreContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<GameStoreContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        private Game CreateValidGame(int id = 1)
        {
            return new Game
            {
                GameId = id,
                Name = $"Test Game {id}",
                GenreId = 1,
                Price = 50,
                ReleaseDate = new DateOnly(2023, 1, 1)
            };
        }

        private void SeedGenres(GameStoreContext context)
        {
            context.Genres.Add(new Genre { GenreId = 1, Name = "Fighting" });
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAllGamesAsync_ReturnsGames()
        {
            var options = CreateNewContextOptions();
            using var context = new GameStoreContext(options);

            SeedGenres(context);

            context.Games.AddRange(
                CreateValidGame(1),
                CreateValidGame(2)
            );
            await context.SaveChangesAsync();

            var service = new GameService(context);

            var result = await service.GetAllGamesAsync();

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task GetGameByIdAsync_ExistingId_ReturnsGame()
        {
            var options = CreateNewContextOptions();
            using var context = new GameStoreContext(options);
            SeedGenres(context);

            var game = CreateValidGame(1);
            game.Name = "Test";
            context.Games.Add(game);
            await context.SaveChangesAsync();
            var service = new GameService(context);

            var result = await service.GetGameByIdAsync(1);

            Assert.True(result.Success);
            Assert.Equal("Test", result.Data.Name);
        }

        [Fact]
        public async Task GetGameByIdAsync_NonExistingId_ReturnsError()
        {
            var options = CreateNewContextOptions();
            using var context = new GameStoreContext(options);
            SeedGenres(context);

            var service = new GameService(context);

            var result = await service.GetGameByIdAsync(1);

            Assert.False(result.Success);
            Assert.Equal("Game not found", result.Message);
        }

        [Fact]
        public async Task AddGameAsync_AddsGame()
        {
            var options = CreateNewContextOptions();
            using var context = new GameStoreContext(options);
            SeedGenres(context);

            var service = new GameService(context);
            var newGame = CreateValidGame(1);

            var result = await service.AddGameAsync(newGame);

            Assert.True(result.Success);
            Assert.Equal(1, await context.Games.CountAsync());
        }

        [Fact]
        public async Task UpdateGameAsync_ExistingGame_Updates()
        {
            var options = CreateNewContextOptions();
            using var context = new GameStoreContext(options);
            SeedGenres(context);

            var existingGame = CreateValidGame(1);
            existingGame.Name = "Old";
            context.Games.Add(existingGame);
            await context.SaveChangesAsync();
            var service = new GameService(context);
            var updatedGame = CreateValidGame(1);
            updatedGame.Name = "New";

            var result = await service.UpdateGameAsync(1, updatedGame);

            Assert.True(result.Success);
            Assert.Equal("New", result.Data.Name);
        }

        [Fact]
        public async Task DeleteGameAsync_ExistingGame_Deletes()
        {
            var options = CreateNewContextOptions();
            using var context = new GameStoreContext(options);
            SeedGenres(context);

            var game = CreateValidGame(1);
            context.Games.Add(game);
            await context.SaveChangesAsync();
            var service = new GameService(context);

            var result = await service.DeleteGameAsync(1);

            Assert.True(result.Success);
            Assert.Empty(context.Games);
        }

        [Fact]
        public async Task GetFilteredGamesAsync_WithFilters_ReturnsFiltered()
        {
            var options = CreateNewContextOptions();
            using var context = new GameStoreContext(options);
            SeedGenres(context);

            context.Games.AddRange(
                new Game { GameId = 1, Name = "Action Game", GenreId = 1, Price = 20, ReleaseDate = new DateOnly(2023, 1, 1) },
                new Game { GameId = 2, Name = "Adventure Game", GenreId = 2, Price = 30, ReleaseDate = new DateOnly(2023, 1, 1) }
            );
            await context.SaveChangesAsync();
            var service = new GameService(context);
            var filter = new GameFilterDto { SearchTerm = "Action", GenreId = 1, PageNumber = 1, PageSize = 10 };

            var result = await service.GetFilteredGamesAsync(filter);

            Assert.True(result.Success);
            Assert.Single(result.Data.Items);
        }
    }
}
