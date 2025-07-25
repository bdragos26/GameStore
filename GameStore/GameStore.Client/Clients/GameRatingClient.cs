using GameStore.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http.Json;
using GameStore.Client.Endpoints;

namespace GameStore.Client.Clients
{
    public interface IGameRatingClient
    {
        Task<GameRating?> GetGameRatingAsync(int userId, int gameId);
        Task<bool> UpdateGameRatingAsync(GameRating rating);
        Task<List<GameRating>> GetRatingsForGameAsync(int gameId);
        Task<List<GameRating>> GetRatingsByUserAsync(int userId);
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
            var response = await _httpClient
                .GetFromJsonAsync<ServiceResponse<GameRating>>(EndpointsRoutes.GameRatingRoutes.GetRating(userId, gameId));

            return response?.Success == true ? response.Data : null;
        }

        public async Task<bool> UpdateGameRatingAsync(GameRating rating)
        {
            var response = await _httpClient
                .PostAsJsonAsync(EndpointsRoutes.GameRatingRoutes.UpdateRating(rating.UserId, rating.GameId), rating);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<GameRating>> GetRatingsForGameAsync(int gameId)
        {
            var response = await _httpClient
                .GetFromJsonAsync<ServiceResponse<List<GameRating>>>(EndpointsRoutes.GameRatingRoutes.GetRatingsForGame(gameId));

            return response?.Data ?? new List<GameRating>();
        }
        public async Task<List<GameRating>> GetRatingsByUserAsync(int userId)
        {
            var response = await _httpClient
                .GetFromJsonAsync<ServiceResponse<List<GameRating>>>(EndpointsRoutes.GameRatingRoutes.GetRatingsByUser(userId));

            return response?.Data ?? new List<GameRating>();
        }
    }
}
