using GameStore.Shared.DTOs;
using GameStore.Shared.Endpoints;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace GameStore.Client.Services.ApiClients
{
    public interface IGameRatingClient
    {
        Task<GameRating?> GetGameRatingAsync(int userId, int gameId);
        Task<bool> UpdateGameRatingAsync(GameRating rating);
        Task<List<GameRating>> GetRatingsForGameAsync(int gameId);
        Task<List<GameRating>> GetRatingsByUserAsync(int userId);
        Task<bool> DeleteRatingAsync(int userId, int gameId);
        Task<List<GameRatingDTO>> GetTopRatedGamesAsync(int count);
    }
    public class GameRatingClient : IGameRatingClient
    {
        private readonly HttpClient _httpClient;

        public GameRatingClient(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(navigationManager.BaseUri);
        }

        public async Task<GameRating?> GetGameRatingAsync(int userId, int gameId)
        {
            var response = await _httpClient.GetAsync(EndpointsRoutes.Game.GetGameRating(userId, gameId));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            var serviceResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<GameRating>>();
            return serviceResponse?.Success == true ? serviceResponse.Data : null;
        }

        public async Task<bool> UpdateGameRatingAsync(GameRating rating)
        {
            var response = await _httpClient
                .PostAsJsonAsync(EndpointsRoutes.Game.UpdateRating(rating.UserId, rating.GameId), rating);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<GameRating>> GetRatingsForGameAsync(int gameId)
        {
            var response = await _httpClient
                .GetFromJsonAsync<ServiceResponse<List<GameRating>>>(EndpointsRoutes.Game.GetRatingsForGame(gameId));

            return response?.Data ?? new List<GameRating>();
        }
        public async Task<List<GameRating>> GetRatingsByUserAsync(int userId)
        {
            var response = await _httpClient
                .GetFromJsonAsync<ServiceResponse<List<GameRating>>>(EndpointsRoutes.Game.GetRatingsByUser(userId));

            return response?.Data ?? new List<GameRating>();
        }

        public async Task<bool> DeleteRatingAsync(int userId, int gameId)
        {
            var response = await _httpClient.DeleteAsync(EndpointsRoutes.Game.DeleteRating(userId, gameId));
            return response.IsSuccessStatusCode;
        }

        public async Task<List<GameRatingDTO>> GetTopRatedGamesAsync(int count)
        {
            var response = await _httpClient
                .GetFromJsonAsync<ServiceResponse<List<GameRatingDTO>>>(EndpointsRoutes.Game.TopRating(count));

            return response?.Data ?? new List<GameRatingDTO>();
        }
    }
}
