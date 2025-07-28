using System.Formats.Asn1;
using System.Net.Http.Json;
using GameStore.Client.Endpoints;
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
            var response = await _httpClient.PostAsJsonAsync(EndpointsRoutes.UserRoutes.baseRoute + 
                EndpointsRoutes.UserRoutes.registerRoute, registerDto);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<User>>();
            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                throw new Exception(result?.Message);
            }
        }

        public async Task<User?> LoginAsync(UserLoginDTO loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync(EndpointsRoutes.UserRoutes.baseRoute + 
                EndpointsRoutes.UserRoutes.loginRoute, loginDto);
            if (response.IsSuccessStatusCode)
            {
                var serviceResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<User>>();
                return serviceResponse?.Data;
            }
            return null;
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync(EndpointsRoutes.UserRoutes.baseRoute +
                EndpointsRoutes.UserRoutes.logoutRoute, null);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
            => await _httpClient.GetFromJsonAsync<User>(EndpointsRoutes.UserRoutes.baseWithIdApi(userId));

        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<User>>>(EndpointsRoutes.UserRoutes.baseRoute);
            return response?.Data ?? new List<User>();
        }

        public async Task UpdateUserAsync(User updatedUser)
        {
            var response = await _httpClient.PutAsJsonAsync(EndpointsRoutes.UserRoutes.baseWithIdApi(updatedUser.UserId), updatedUser);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDto)
        {
            var response = await _httpClient.PostAsJsonAsync(EndpointsRoutes.UserRoutes.baseRoute + 
                EndpointsRoutes.UserRoutes.resetPassRoute, resetPasswordDto);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<User>>();

            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                throw new Exception(result?.Message);
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            var response = await _httpClient.DeleteAsync(EndpointsRoutes.UserRoutes.baseWithIdApi(userId));
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            if (!response.IsSuccessStatusCode || result == null || !result.Success)
            {
                throw new Exception(result?.Message);
            }
        }
    }
}