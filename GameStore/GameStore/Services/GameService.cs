using GameStore.Data;
using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services
{
    public interface IGameService
    {
        Task<ServiceResponse<List<Game>>> GetAllGamesAsync();
        Task<ServiceResponse<Game>> GetGameByIdAsync(int id);
        Task<ServiceResponse<Game>> AddGameAsync(Game newGame);
        Task<ServiceResponse<Game>> UpdateGameAsync(int id, Game updatedGame);
        Task<ServiceResponse<bool>> DeleteGameAsync(int id);
    }
    public class GameService : IGameService
    {
        private readonly GameStoreContext _dbContext;

        public GameService(GameStoreContext gameContext)
        {
            _dbContext = gameContext;
        }

        public async Task<ServiceResponse<List<Game>>> GetAllGamesAsync()
        {
            var response = new ServiceResponse<List<Game>>
            {
                Data = await _dbContext.Games
                    .Include(game => game.Genre)
                    .AsNoTracking()
                    .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<Game>> GetGameByIdAsync(int id)
        {
            var response = new ServiceResponse<Game>()
            {
                Data = await _dbContext.Games.FindAsync(id)
            };

            if (response.Data == null)
            {
                response.Success = false;
                response.Message = "Game not found";
                return response;
            }

            return response;
        }

        public async Task<ServiceResponse<Game>> AddGameAsync(Game newGame)
        {
            _dbContext.Games.Add(newGame);
            await _dbContext.SaveChangesAsync();

            return new ServiceResponse<Game>
            {
                Data = newGame 
            };
        }

        public async Task<ServiceResponse<Game>> UpdateGameAsync(int id, Game updatedGame)
        {
            var response = new ServiceResponse<Game>
            {
                Data = await _dbContext.Games.FindAsync(id)
            };

            if (response.Data == null)
            {
                response.Success = false;
                response.Message = "Game not found";
                return response;
            }

            _dbContext.Entry(response.Data)
                .CurrentValues
                .SetValues(updatedGame);

            await _dbContext.SaveChangesAsync();

            return response;
        }
        public async Task<ServiceResponse<bool>> DeleteGameAsync(int id)
        {
            var game = await _dbContext.Games.FindAsync(id);
            if (game == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false, 
                    Message = "Game not found"
                };
            }

            _dbContext.Games.Remove(game);
            await _dbContext.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true
            };
        }
    }
}
