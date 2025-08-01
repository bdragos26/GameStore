﻿using GameStore.Data;
using GameStore.Shared.DTOs;
using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services
{
    public interface IGameRatingService
    {
        Task<ServiceResponse<GameRating>> GetGameRatingAsync(int userId, int gameId);
        Task<ServiceResponse<GameRating>> UpdateRatingAsync(GameRating rating);
        Task<ServiceResponse<List<GameRating>>> GetRatingsForGameAsync(int gameId);
        Task<ServiceResponse<List<GameRating>>> GetRatingsByUserAsync(int userId);
        Task<ServiceResponse<bool>> DeleteRatingAsync(int userId, int gameId);
        Task<ServiceResponse<List<GameRatingDTO>>> GetTopRatedGamesAsync(int count);
    }
    public class GameRatingService : IGameRatingService
    {
        private readonly GameStoreContext _dbContext;

        public GameRatingService(GameStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceResponse<GameRating>> GetGameRatingAsync(int userId, int gameId)
        {
            var response = new ServiceResponse<GameRating>();

            try
            {
                var rating = await _dbContext.Ratings
                    .FirstOrDefaultAsync(r => r.UserId == userId && r.GameId == gameId);

                if (rating == null)
                {
                    response.Success = false;
                    response.Message = "Rating not found!";
                }
                else
                {
                    response.Data = rating;
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving rating: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<GameRating>> UpdateRatingAsync(GameRating rating)
        {
            var response = new ServiceResponse<GameRating>();

            try
            {
                var existingRating = await _dbContext.Ratings
                    .FirstOrDefaultAsync(r => r.UserId == rating.UserId && r.GameId == rating.GameId);

                if (existingRating == null)
                {
                    _dbContext.Ratings.Add(rating);
                    response.Message = "Rating added successfully.";
                }
                else
                {
                    existingRating.Score = rating.Score;
                    _dbContext.Ratings.Update(existingRating);
                    response.Message = "Rating updated successfully.";
                }

                await _dbContext.SaveChangesAsync();
                response.Data = rating;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error updating rating: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<GameRating>>> GetRatingsForGameAsync(int gameId)
        {
            var response = new ServiceResponse<List<GameRating>>();

            try
            {
                var ratings = await _dbContext.Ratings
                    .Where(r => r.GameId == gameId)
                    .Include(r => r.User)
                    .ToListAsync();

                response.Data = ratings;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving ratings: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<GameRating>>> GetRatingsByUserAsync(int userId)
        {
            var response = new ServiceResponse<List<GameRating>>();

            try
            {
                var ratings = await _dbContext.Ratings
                    .Where(r => r.UserId == userId)
                    .Include(r => r.GameDetails)
                    .ToListAsync();

                response.Data = ratings;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error retrieving ratings by user: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteRatingAsync(int userId, int gameId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var rating = await _dbContext.Ratings
                    .FirstOrDefaultAsync(r => r.UserId == userId && r.GameId == gameId);

                if (rating == null)
                {
                    response.Success = false;
                    response.Message = "Rating not found!";
                    return response;
                }

                _dbContext.Ratings.Remove(rating);
                await _dbContext.SaveChangesAsync();

                response.Data = true;
                response.Success = true;
                response.Message = "Rating deleted successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error deleting rating: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<GameRatingDTO>>> GetTopRatedGamesAsync(int count)
        {
            var response = new ServiceResponse<List<GameRatingDTO>>();

            try
            {
                var topGames = await _dbContext.Ratings.GroupBy(r => r.GameDetails)
                    .Select(g => new GameRatingDTO
                    {
                        Game = g.Key,
                        AverageScore = g.Average(r => r.Score),
                        RatingCount = g.Count()
                    })
                    .OrderByDescending(g => g.AverageScore)
                    .Take(count)
                    .ToListAsync();

                response.Success = true;
                response.Data = topGames;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }

            return response;
        }
    }
}
