using System.Net.Http.Json;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace GameStore.Client.Clients
{
    public interface IGamesClient
    {
        Task<List<GameDetails>> GetGamesAsync();
        Task AddGameAsync(GameDetails game);
        Task UpdateGameAsync(GameDetails updatedGame);
        Task DeleteGameAsync(int id);
        Task<GameDetails> GetGameAsync(int id);
    }
    public class GamesClient : IGamesClient
    {
        private readonly HttpClient _httpClient;

        public GamesClient(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(navigationManager.BaseUri);
        }
        public async Task<List<GameDetails>> GetGamesAsync()
            => await _httpClient.GetFromJsonAsync<List<GameDetails>>("/games") ?? new List<GameDetails>();

        public async Task AddGameAsync(GameDetails game)
            => await _httpClient.PostAsJsonAsync("/games", game);

        public async Task UpdateGameAsync(GameDetails updatedGame)
            => await _httpClient.PutAsJsonAsync($"/games/{updatedGame.Id}", updatedGame);

        public async Task DeleteGameAsync(int id)
            => await _httpClient.DeleteAsync($"/games/{id}");

        public async Task<GameDetails> GetGameAsync(int id)
            => await _httpClient.GetFromJsonAsync<GameDetails>($"/games/{id}") 
               ?? throw new Exception("Could not find the game");
    }
}
