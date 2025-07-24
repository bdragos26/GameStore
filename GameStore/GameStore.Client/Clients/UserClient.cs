using System.Formats.Asn1;
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
        Task LogoutAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<List<User>> GetUsersAsync();
        Task UpdateUserAsync(User updatedUser);
        Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDto);
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
        {
            var response = await _httpClient.PostAsJsonAsync("/users/register", registerDto);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<User>>();
            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                throw new Exception(result?.Message);
            }
        }

        public async Task<User?> LoginAsync(UserLoginDTO loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("users/login", loginDto);
            if (response.IsSuccessStatusCode)
            {
                var serviceResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<User>>();
                return serviceResponse?.Data;
            }
            return null;
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync("users/logout", null);
        }

        public async Task<User?> GetUserByIdAsync(int id)
            => await _httpClient.GetFromJsonAsync<User>($"/users/{id}");

        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<User>>>("/users");
            return response?.Data ?? new List<User>();
        }

        public async Task UpdateUserAsync(User updatedUser)
        {
            var response = await _httpClient.PutAsJsonAsync($"/users/{updatedUser.UserId}", updatedUser);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/users/resetPass", resetPasswordDto);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<User>>();

            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                throw new Exception(result?.Message);
            }
        }
    }
}