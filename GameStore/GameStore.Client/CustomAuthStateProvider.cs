using Blazored.LocalStorage;
using GameStore.Client.Services.ApiClients;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace GameStore.Client
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ClaimsPrincipal defaultClaimsPrincipal = new(new ClaimsIdentity());
        private readonly ICartClient _cartClient;

        public CustomAuthStateProvider(ILocalStorageService localStorage, ICartClient cartClient)
        {
            _localStorage = localStorage;
            _cartClient = cartClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            await Task.Delay(500);
            try
            {
                User? user = await _localStorage.GetItemAsync<User>("user");
                if (user == null)
                    return await Task.FromResult(new AuthenticationState(defaultClaimsPrincipal));

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role.ToString())
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
                await _localStorage.SetItemAsync("user", user);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role.ToString())
                };

                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "GameStoreAuth"));

                await MigrateGuestCartToUserCart(user.UserId);
            }
            else
            {
                await _localStorage.RemoveItemAsync("user");
                claimsPrincipal = defaultClaimsPrincipal;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            await _cartClient.NotifyCartChanged();
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