using GameStore.Services;
using GameStore.Data;
using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GameStore.Tests.ServiceTests
{
    public class GenreServiceTests
    {
        [Fact]
        public async Task GetAllGenresAsync_ReturnsAllGenres()
        {
            var options = new DbContextOptionsBuilder<GameStoreContext>()
                .UseInMemoryDatabase(databaseName: "GenreServiceTestDb")
                .Options;
            using var context = new GameStoreContext(options);
            context.Genres.Add(new Genre { GenreId = 1, Name = "Action" });
            context.Genres.Add(new Genre { GenreId = 2, Name = "Adventure" });
            await context.SaveChangesAsync();
            var service = new GenreService(context);
            var result = await service.GetAllGenresAsync();
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task GetAllGenresAsync_EmptyDb_ReturnsEmptyList()
        {
            var options = new DbContextOptionsBuilder<GameStoreContext>()
                .UseInMemoryDatabase(databaseName: "GenreServiceTestDbEmpty")
                .Options;
            using var context = new GameStoreContext(options);
            var service = new GenreService(context);
            var result = await service.GetAllGenresAsync();
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }
    }
}
