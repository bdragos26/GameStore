using Blazored.LocalStorage;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace GameStore.Client
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ClaimsPrincipal defaultClaimsPrincipal = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userId = await _localStorage.GetItemAsync<int>("userId");
                var username = await _localStorage.GetItemAsync<string>("username");
                var email = await _localStorage.GetItemAsync<string>("email");

                if (userId == 0 || string.IsNullOrEmpty(username))
                    return await Task.FromResult(new AuthenticationState(defaultClaimsPrincipal));   

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, userId.ToString()),
                    new(ClaimTypes.Name, username),
                    new(ClaimTypes.Email, email)
                };

                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "GameStoreAuth"))));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(defaultClaimsPrincipal));
            }
        }

        public async Task UpdateAuthenticationState(User? user)
        {
            ClaimsPrincipal claimsPrincipal;

            if (user != null)
            {
                await _localStorage.SetItemAsync("userId", user.Id);
                await _localStorage.SetItemAsync("username", user.Username);
                await _localStorage.SetItemAsync("email", user.Email);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Email, user.Email)
                };

                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "GameStoreAuth"));
            }
            else
            {
                await _localStorage.RemoveItemAsync("userId");
                await _localStorage.RemoveItemAsync("username");
                await _localStorage.RemoveItemAsync("email");
                claimsPrincipal = defaultClaimsPrincipal;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}