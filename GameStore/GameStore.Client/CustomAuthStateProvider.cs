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
            await Task.Delay(500);
            try
            {
                //var userId = await _localStorage.GetItemAsync<int>("userId");
                //var username = await _localStorage.GetItemAsync<string>("username");
                //var email = await _localStorage.GetItemAsync<string>("email");

                User? user = await _localStorage.GetItemAsync<User>("user");

                //if (userId == 0 || string.IsNullOrEmpty(username))
                //    return await Task.FromResult(new AuthenticationState(defaultClaimsPrincipal));   
                if (user == null)
                    return await Task.FromResult(new AuthenticationState(defaultClaimsPrincipal));

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role)
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
                //await _localStorage.SetItemAsync("userId", user.GameId);
                //await _localStorage.SetItemAsync("username", user.Username);
                //await _localStorage.SetItemAsync("email", user.Email);
                await _localStorage.SetItemAsync("user", user);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role)
                };

                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "GameStoreAuth"));
            }
            else
            {
                //await _localStorage.RemoveItemAsync("userId");
                //await _localStorage.RemoveItemAsync("username");
                //await _localStorage.RemoveItemAsync("email");
                await _localStorage.RemoveItemAsync("user");
                claimsPrincipal = defaultClaimsPrincipal;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}