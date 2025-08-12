using Blazored.LocalStorage;
using GameStore.Client.Services.ApiClients;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace GameStore.Client
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ICartClient _cartClient;
        private readonly HttpClient _http;

        public CustomAuthStateProvider(ILocalStorageService localStorage, ICartClient cartClient, HttpClient http)
        {
            _localStorage = localStorage;
            _cartClient = cartClient;
            _http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var authToken = await _localStorage.GetItemAsync<string>("authToken");
            var identity = new ClaimsIdentity();
            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(authToken))
            {
                try
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(authToken), "jwt");
                    _http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", authToken);
                }
                catch
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    identity = new ClaimsIdentity();
                }
            }

            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }

        public async Task UpdateAuthenticationState(string? token)
        {
            ClaimsPrincipal claimsPrincipal;

            if (!string.IsNullOrEmpty(token))
            {
                await _localStorage.SetItemAsync("authToken", token);
                var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                claimsPrincipal = new ClaimsPrincipal(identity);
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var userId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await MigrateGuestCartToUserCart(userId);
            }
            else
            {
                await _localStorage.RemoveItemAsync("authToken");
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
                _http.DefaultRequestHeaders.Authorization = null;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            await _cartClient.NotifyCartChanged();
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer
                .Deserialize<Dictionary<string, object>>(jsonBytes);

            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }
        private async Task MigrateGuestCartToUserCart(int userId)
        {
            var guestCardKey = "cart-guest";
            var userCartKey = $"cart-{userId}";
            var guestCart = await _localStorage.GetItemAsync<List<CartItem>>(guestCardKey);

            if (guestCart?.Any() == true)
            {
                var userCart = await _localStorage.GetItemAsync<List<CartItem>>(userCartKey) ?? new List<CartItem>();
                foreach (var item in guestCart)
                {
                    var existingItem = userCart.FirstOrDefault(uc => uc.GameId == item.GameId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += item.Quantity;
                    }
                    else
                    {
                        userCart.Add(item);
                    }
                }

                await _localStorage.SetItemAsync(userCartKey, userCart);
                await _localStorage.RemoveItemAsync(guestCardKey);
            }
        }
    }
}