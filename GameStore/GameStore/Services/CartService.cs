using GameStore.Data;
using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Services;

public interface ICartService
{
    Task<ServiceResponse<List<CartItem>>> GetCartGamesAsync(List<CartItem>? cartItems);
}
public class CartService : ICartService
{
    private readonly GameStoreContext _context;

    public CartService(GameStoreContext context)
    {
        _context = context;
    }



    public async Task<ServiceResponse<List<CartItem?>>> GetCartGamesAsync(List<CartItem>? cartItems)
    {
        var response = new ServiceResponse<List<CartItem?>>();

        if (cartItems == null || !cartItems.Any())
        {
            response.Data = new List<CartItem?>();
            response.Success = true;

            return response;
        }

        try
        {
            var ids = cartItems.Select(ci => ci.GameId).ToList();
            var games = await _context.Games
                .Where(g => ids.Contains(g.GameId))
                .ToListAsync();

            response.Data = cartItems.Select(item =>
                {
                    var game = games.FirstOrDefault(g => g.GameId == item.GameId);
                    return game != null ? new CartItem
                    {
                        GameId = game.GameId,
                        Name = game.Name,
                        Price = game.Price,
                        Quantity = item.Quantity
                    } : null;
                })
                .Where(x => x != null)
                .ToList();

            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
            return response;
        }
    }
}