using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace GameStore
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService localStorage;
        private readonly ClaimsPrincipal defaultClaimPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var username = await localStorage.GetItemAsync<string>("username");
            var email = await localStorage.GetItemAsync<string>("email");
            if (string.IsNullOrEmpty(username))
            {
                return new AuthenticationState(defaultClaimPrincipal);
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email)
            };
            return new AuthenticationState(new ClaimsPrincipal(
                new ClaimsIdentity(claims, nameof(CustomAuthStateProvider))));
        }

        public async Task MarkUserAuthenticated(string username, string email)
        {
            await localStorage.SetItemAsync("username", username);
            await localStorage.SetItemAsync("email", email);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email)
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, nameof(CustomAuthStateProvider)));
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await localStorage.RemoveItemAsync("username");
            await localStorage.RemoveItemAsync("email");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(defaultClaimPrincipal)));
        }
    }
}