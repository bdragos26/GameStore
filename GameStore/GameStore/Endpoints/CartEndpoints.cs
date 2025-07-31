using GameStore.Services;
using GameStore.Shared.Endpoints;
using GameStore.Shared.Models;

namespace GameStore.Endpoints
{
    public static class CartEndpoints
    {
        public static WebApplication MapCartEndpoints(this WebApplication app)
        {
            app.MapPost(EndpointsRoutes.Cart.getCartGames, async (List<CartItem> cartItems, ICartService cartService) =>
            {
                var result = await cartService.GetCartGamesAsync(cartItems);
                return Results.Ok(result);
            });

            return app;
        }
    }
}
