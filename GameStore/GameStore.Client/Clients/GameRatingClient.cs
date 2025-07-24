using GameStore.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http.Json;

namespace GameStore.Client.Clients
{
    public interface IGameRatingClient
    {
        Task<GameRating?> GetGameRatingAsync(int userId, int gameId);
        Task<bool> UpdateGameRatingAsync(GameRating rating);
        Task<List<GameRating>> GetRatingsForGameAsync(int gameId);
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
                .GetFromJsonAsync<ServiceResponse<GameRating>>($"/ratings?userId={userId}&gameId={gameId}");

            return response?.Success == true ? response.Data : null;
        }

        public async Task<bool> UpdateGameRatingAsync(GameRating rating)
        {
            var response = await _httpClient
                .PostAsJsonAsync($"/ratings/{rating.UserId}/{rating.GameId}", rating);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<GameRating>> GetRatingsForGameAsync(int gameId)
        {
            var response = await _httpClient
                .GetFromJsonAsync<ServiceResponse<List<GameRating>>>($"/ratings/{gameId}");

            return response?.Data ?? new List<GameRating>();
        }
    }
}
