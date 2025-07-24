using System.Net.Http.Json;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace GameStore.Client.Clients
{
    public interface IGamesClient
    {
        Task<List<Game>> GetGamesAsync();
        Task AddGameAsync(Game game);
        Task UpdateGameAsync(Game updatedGame);
        Task DeleteGameAsync(int id);
        Task<Game> GetGameByIdAsync(int id);
    }
    public class GamesClient : IGamesClient
    {
        private readonly HttpClient _httpClient;

        public GamesClient(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(navigationManager.BaseUri);
        }
        public async Task<List<Game>> GetGamesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<Game>>>("/games");
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "Failed to load games");

            return response.Data!;
        }

        public async Task AddGameAsync(Game game)
        {
            var response = await _httpClient.PostAsJsonAsync("/games", game);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<Game>>();

            if (!response.IsSuccessStatusCode || result == null || !result.Success)
                throw new Exception(result?.Message ?? "Failed to add game");
        }

        public async Task UpdateGameAsync(Game updatedGame)
        {
            var response = await _httpClient.PutAsJsonAsync($"/games/{updatedGame.GameId}", updatedGame);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<Game>>();

            if (!response.IsSuccessStatusCode || result == null || !result.Success)
                throw new Exception(result?.Message ?? "Failed to update game");
        }

        public async Task DeleteGameAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/games/{id}");
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

            if (!response.IsSuccessStatusCode || result == null || !result.Success)
                throw new Exception(result?.Message ?? "Failed to delete game");
        }

        public async Task<Game> GetGameByIdAsync(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<Game>>($"/games/{id}");
            if (response == null || !response.Success)
                throw new Exception(response?.Message ?? "Game not found");

            return response.Data!;
        }
    }
}
