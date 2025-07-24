using GameStore.Data;
using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services
{
    public interface IGenreService
    {
        Task<ServiceResponse<List<Genre>>> GetAllGenresAsync();
    }
    public class GenreService : IGenreService
    {
        private readonly GameStoreContext _dbContext;

        public GenreService(GameStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceResponse<List<Genre>>> GetAllGenresAsync()
        {
            var response = new ServiceResponse<List<Genre>>();

            try
            {
                var genres = await _dbContext.Genres.ToListAsync();
                response.Data = genres;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }
    }
}
