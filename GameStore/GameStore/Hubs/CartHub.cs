using Microsoft.AspNetCore.SignalR;

namespace GameStore.Hubs
{
    public class CartHub : Hub
    {
        public async Task JoinCartGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"cart-{userId}");
        }
        public async Task NotifyCartChanged(string userId)
        {
            await Clients.Group($"cart-{userId}").SendAsync("CartUpdated");
        }
    }
}
