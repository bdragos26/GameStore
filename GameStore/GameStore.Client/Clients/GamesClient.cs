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
            => await _httpClient.GetFromJsonAsync<List<Game>>("/games") ?? new List<Game>();

        public async Task AddGameAsync(Game game)
            => await _httpClient.PostAsJsonAsync("/games", game);

        public async Task UpdateGameAsync(Game updatedGame)
            => await _httpClient.PutAsJsonAsync($"/games/{updatedGame.GameId}", updatedGame);

        public async Task DeleteGameAsync(int id)
            => await _httpClient.DeleteAsync($"/games/{id}");

        public async Task<Game> GetGameByIdAsync(int id)
            => await _httpClient.GetFromJsonAsync<Game>($"/games/{id}") 
               ?? throw new Exception("Could not find the game");
    }
}
