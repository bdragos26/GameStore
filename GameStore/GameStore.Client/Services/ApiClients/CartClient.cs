using Blazored.LocalStorage;
using GameStore.Shared.Endpoints;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;

namespace GameStore.Client.Services.ApiClients
{
    public interface ICartClient
    {
        event Action OnChange;
        Task InitializeSignalR();
        Task NotifyCartChanged();
        Task AddToCart(CartItem cartItem);
        Task<List<CartItem>> GetCartItems();
        Task<List<CartItem>> GetCartGames();
        Task RemoveGameFromCart(int gameId);
        Task UpdateQuantity(CartItem game);
        Task ClearCart();
    }
    public class CartClient : ICartClient, IDisposable
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;
        private readonly HubConnection _hubConnection;
        private string _userId;
        private bool _isInitialized;

        public CartClient(ILocalStorageService localStorage, HttpClient http, NavigationManager navigationManager)
        {
            _localStorage = localStorage;
            _http = http;
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(navigationManager.ToAbsoluteUri("/cartHub"))
                .Build();
        }

        public async Task InitializeSignalR()
        {
            if (_isInitialized)
                return;

            try
            {
                var authToken = await _localStorage.GetItemAsync<string>("authToken");
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(authToken) as JwtSecurityToken;
                _userId = jsonToken?.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value ?? "guest";

                if (_hubConnection.State == HubConnectionState.Disconnected)
                {
                    _hubConnection.On("CartUpdated", () =>
                    {
                        OnChange?.Invoke();
                        return Task.CompletedTask;
                    });

                    await _hubConnection.StartAsync();
                    await _hubConnection.InvokeAsync("JoinCartGroup", _userId);
                    _isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing SignalR: {ex.Message}");
            }
        }

        private async Task<string> GetCartKey()
        {
            var authToken = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(authToken))
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(authToken) as JwtSecurityToken;
                var userId = jsonToken?.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                return $"cart-{userId}";
            }
            return "cart-guest";
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
            await _hubConnection.InvokeAsync("NotifyCartChanged", _userId);
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
                await _hubConnection.InvokeAsync("NotifyCartChanged", _userId);
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
                await _hubConnection.InvokeAsync("NotifyCartChanged", _userId);
                OnChange?.Invoke();
            }
        }

        public async Task ClearCart()
        {
            var cartKey = await GetCartKey();
            await _localStorage.RemoveItemAsync(cartKey);
            await _hubConnection.InvokeAsync("NotifyCartChanged", _userId);
            OnChange?.Invoke();
        }

        public void Dispose()
        {
            _hubConnection.DisposeAsync().AsTask().Wait();
            _http.Dispose();
        }
    }
}
