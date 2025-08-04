using Blazored.LocalStorage;
using GameStore.Shared.Endpoints;
using GameStore.Shared.Models;
using System.Net.Http.Json;

namespace GameStore.Client.Services.ApiClients
{
    public interface ICartClient
    {
        event Action OnChange;
        Task NotifyCartChanged();
        Task AddToCart(CartItem cartItem);
        Task<List<CartItem>> GetCartItems();
        Task<List<CartItem>> GetCartGames();
        Task RemoveGameFromCart(int gameId);
        Task UpdateQuantity(CartItem game);
        Task ClearCart();
    }
    public class CartClient : ICartClient
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;

        public CartClient(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }

        private async Task<string> GetCartKey()
        {
            var user = await _localStorage.GetItemAsync<User>("user");
            return $"cart-{user?.UserId.ToString() ?? "guest"}";
        }

        public event Action OnChange;

        public Task NotifyCartChanged()
        {
            OnChange?.Invoke();
            return Task.CompletedTask;
        }

        public async Task AddToCart(CartItem cartItem)
        {
            var cartKey = await GetCartKey();
            var cart = await _localStorage.GetItemAsync<List<CartItem>>(cartKey) ?? new List<CartItem>();

            var sameItem = cart.Find(x => x.GameId == cartItem.GameId);
            if (sameItem == null)
            {
                cart.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

            await _localStorage.SetItemAsync(cartKey, cart);
            OnChange?.Invoke();
        }

        public async Task<List<CartItem>> GetCartItems()
        {
            var cartKey = await GetCartKey();
            var cart = await _localStorage.GetItemAsync<List<CartItem>>(cartKey) ?? new List<CartItem>();
            return cart;
        }

        public async Task<List<CartItem>> GetCartGames()
        {
            try
            {
                var cartKey = await GetCartKey();
                var cartItems = await _localStorage.GetItemAsync<List<CartItem>>(cartKey) ?? new List<CartItem>();

                if (!cartItems.Any())
                    return new List<CartItem>();

                var response = await _http.PostAsJsonAsync(EndpointsRoutes.Cart.getCartGames, cartItems);

                if (!response.IsSuccessStatusCode)
                    return new List<CartItem>();

                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartItem>>>();

                return result?.Data ?? new List<CartItem>();
            }
            catch
            {
                return new List<CartItem>();
            }
        }

        public async Task RemoveGameFromCart(int gameId)
        {
            var cartKey = await GetCartKey();
            var cart = await _localStorage.GetItemAsync<List<CartItem>>(cartKey);
            if (cart == null) return;

            var cartItem = cart.Find(x => x.GameId == gameId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                await _localStorage.SetItemAsync(cartKey, cart);
                OnChange?.Invoke();
            }
        }

        public async Task UpdateQuantity(CartItem game)
        {
            var cartKey = await GetCartKey();
            var cart = await _localStorage.GetItemAsync<List<CartItem>>(cartKey);
            if (cart == null) return;

            var cartItem = cart.Find(x => x.GameId == game.GameId);
            if (cartItem != null)
            {
                cartItem.Quantity = game.Quantity;
                await _localStorage.SetItemAsync(cartKey, cart);
                OnChange?.Invoke();
            }
        }

        public async Task ClearCart()
        {
            var cartKey = await GetCartKey();
            await _localStorage.RemoveItemAsync(cartKey);
            OnChange?.Invoke();
        }
    }
}
