using GameStore.Shared.DTOs;
using GameStore.Shared.Endpoints;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace GameStore.Client.Services.ApiClients
{
    public interface IUserClient
    {
        Task RegisterAsync(UserRegisterDto registerDto);
        Task<User?> LoginAsync(UserLoginDTO loginDto);
        Task LogoutAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task<List<User>> GetUsersAsync();
        Task UpdateUserAsync(User updatedUser);
        Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDto);
        Task DeleteUserAsync(int userId);
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
            var response = await _httpClient.PostAsJsonAsync(EndpointsRoutes.User._base +
                EndpointsRoutes.User.register, registerDto);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<User>>();
            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                throw new Exception(result?.Message);
            }
        }

        public async Task<User?> LoginAsync(UserLoginDTO loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync(EndpointsRoutes.User._base +
                EndpointsRoutes.User.login, loginDto);
            if (response.IsSuccessStatusCode)
            {
                var serviceResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<User>>();
                return serviceResponse?.Data;
            }
            return null;
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync(EndpointsRoutes.User._base +
                EndpointsRoutes.User.logout, null);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
            => await _httpClient.GetFromJsonAsync<User>(EndpointsRoutes.User.GetById(userId));

        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<User>>>(EndpointsRoutes.User._base);
            return response?.Data ?? new List<User>();
        }

        public async Task UpdateUserAsync(User updatedUser)
        {
            var response = await _httpClient.PutAsJsonAsync(EndpointsRoutes.User.Update(updatedUser.UserId), updatedUser);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDto)
        {
            var response = await _httpClient.PostAsJsonAsync(EndpointsRoutes.User._base +
                EndpointsRoutes.User.resetPass, resetPasswordDto);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<User>>();

            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                throw new Exception(result?.Message);
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync(EndpointsRoutes.User.Delete(userId));
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                throw new Exception(result?.Message);
            }
        }
    }
}