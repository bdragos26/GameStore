using GameStore.Data;
using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services
{
    public interface IGameRatingService
    {
        Task<GameRating> GetGameRatingAsync(int userId, int gameId);
        Task<GameRating> UpdateRatingAsync(GameRating rating);
        Task<List<GameRating>> GetRatingsForGameAsync(int gameId);
    }
    public class GameRatingService : IGameRatingService
    {
        private readonly GameStoreContext _dbContext;

        public GameRatingService(GameStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GameRating> GetGameRatingAsync(int userId, int gameId)
        {
            return await _dbContext.Ratings.
                FirstOrDefaultAsync(r => r.UserId == userId && r.GameId == gameId);
        }

        public async Task<GameRating> UpdateRatingAsync(GameRating rating)
        {
            var existingRating = await _dbContext.Ratings.
                FirstOrDefaultAsync(r => r.UserId == rating.UserId && r.GameId == rating.GameId);
            if (existingRating == null)
            {
                _dbContext.Ratings.Add(rating);
            }
            else
            {
                existingRating.Score = rating.Score;
                _dbContext.Ratings.Update(existingRating);
            }

            await _dbContext.SaveChangesAsync();
            return rating;
        }

        public async Task<List<GameRating>> GetRatingsForGameAsync(int gameId)
        {
            return await _dbContext.Ratings.Where(r => r.GameId == gameId)
                .Include(r => r.User).ToListAsync();
        }
    }
}
