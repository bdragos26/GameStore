using System.Net.Http.Json;
using GameStore.Shared.Models;
using GameStore.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace GameStore.Client.Clients
{
    public interface IUserClient
    {
        Task RegisterAsync(UserRegisterDto registerDto);
        Task<User?> LoginAsync(UserLoginDTO loginDto);
        Task<User?> GetUserByIdAsync(int id);
        Task<List<User>> GetUsersAsync();
    }

    public class UserClient : IUserClient
    {
        private readonly HttpClient _httpClient;

        public UserClient(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(navigationManager.BaseUri);
        }

        public async Task RegisterAsync(UserRegisterDto registerDto) 
            => await _httpClient.PostAsJsonAsync("/users/register", registerDto);

        public async Task<User?> LoginAsync(UserLoginDTO loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/users/login", loginDto);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<User>() : null;
        }

        public async Task<User?> GetUserByIdAsync(int id)
            => await _httpClient.GetFromJsonAsync<User>($"/users/{id}");

        public async Task<List<User>> GetUsersAsync()
            => await _httpClient.GetFromJsonAsync<List<User>>("/users") ?? new List<User>();
    }
}