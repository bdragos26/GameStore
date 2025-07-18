using GameStore.Data;
using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services
{
    public interface IGenreService
    {
        Task<List<Genre>> GetAllGenresAsync();
    }
    public class GenreService : IGenreService
    {
        private readonly GameStoreContext _dbContext;

        public GenreService(GameStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Genre>> GetAllGenresAsync()
        {
            var genres = await _dbContext.Genres.ToListAsync();
            return genres;
        }
    }
}
