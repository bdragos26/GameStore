using GameStore.Data;
using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services
{
    public interface IGameService
    {
        Task<List<Game>> GetAllGamesAsync();
        Task<Game> GetGameByIdAsync(int id);
        Task AddGameAsync(Game newGame);
        Task<Game> UpdateGameAsync(int id, Game updatedGame);
        Task DeleteGameAsync(int id);
    }
    public class GameService : IGameService
    {
        private readonly GameStoreContext _dbContext;

        public GameService(GameStoreContext gameContext)
        {
            _dbContext = gameContext;
        }

        public async Task<List<Game>> GetAllGamesAsync()
        {
            var games = await _dbContext.Games
                .Include(game => game.Genre)
                .AsNoTracking()
                .ToListAsync();
                
            return games;
        }

        public async Task<Game> GetGameByIdAsync(int id)
        {
            var game = await _dbContext.Games.FindAsync(id);
            return game;
        }

        public async Task AddGameAsync(Game newGame)
        { 
            _dbContext.Games.Add(newGame);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Game> UpdateGameAsync(int id, Game updatedGame)
        {
             var existingGame = await _dbContext.Games.FindAsync(id);

            _dbContext.Entry(existingGame)
                .CurrentValues
                .SetValues(updatedGame);

            await _dbContext.SaveChangesAsync();

            return existingGame;
        }

        public async Task DeleteGameAsync(int id)
        {
            await _dbContext.Games
                .Where(game => game.GameId == id)
                .ExecuteDeleteAsync();
        }
    }
}
