using GameStore.Client.Endpoints;
using GameStore.Data;
using GameStore.Services;
using GameStore.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Endpoints
{
    public static class GameEndpoints
    {
        public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
        {
            var group = app.MapGroup(EndpointsRoutes.GamesRoutes.baseRoute);

            group.MapGet("/", async (IGameService gameService) =>
            {
                var result = await gameService.GetAllGamesAsync();
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            });

            group.MapGet(EndpointsRoutes.GamesRoutes.baseWithIdRoute, async (int gameId, IGameService gameService) =>
            {
                var result = await gameService.GetGameByIdAsync(gameId);
                return result.Success ? Results.Ok(result) : Results.NotFound(result);
            });

            group.MapPost("/", async (Game newGame, IGameService gameService) =>
            {
                var result = await gameService.AddGameAsync(newGame);
                return result.Success ? Results.Created(EndpointsRoutes.GamesRoutes.baseWithIdApi(result.Data!.GameId), result) : Results.BadRequest(result);
            });

            group.MapPut(EndpointsRoutes.GamesRoutes.baseWithIdRoute, async (int gameId, Game updatedGame, IGameService gameService) =>
            {
                var result = await gameService.UpdateGameAsync(gameId, updatedGame);
                return result.Success ? Results.Ok(result) : Results.NotFound(result);
            });

            group.MapDelete(EndpointsRoutes.GamesRoutes.baseWithIdRoute, async (int gameId, IGameService gameService) =>
            {
                var result = await gameService.DeleteGameAsync(gameId);
                return result.Success ? Results.Ok(result) : Results.NotFound(result);
            });

            return group;
        }
    }
}
