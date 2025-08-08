using GameStore.Services;
using GameStore.Shared.DTOs;
using GameStore.Shared.Endpoints;
using GameStore.Shared.Models;

namespace GameStore.Endpoints
{
    public static class GameEndpoints
    {
        public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
        {
            var group = app.MapGroup(EndpointsRoutes.Games._base);

            group.MapGet("/", async (IGameService gameService) =>
            {
                var result = await gameService.GetAllGamesAsync();
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            });

            group.MapPost(EndpointsRoutes.Games.getFiltered, async (GameFilterDto filter, IGameService gameService) =>
            {
                var result = await gameService.GetFilteredGamesAsync(filter);
                return result.Success ? Results.Ok(result) : Results.BadRequest(result);
            });

            group.MapGet(EndpointsRoutes.Games.getById, async (int gameId, IGameService gameService) =>
            {
                var result = await gameService.GetGameByIdAsync(gameId);
                return result.Success ? Results.Ok(result) : Results.NotFound(result);
            });

            group.MapPost("/", async (Game newGame, IGameService gameService) =>
            {
                var result = await gameService.AddGameAsync(newGame);
                return result.Success ? Results.Created(EndpointsRoutes.Games.Add(result.Data!.GameId), result) : Results.BadRequest(result);
            });

            group.MapPut(EndpointsRoutes.Games.update, async (int gameId, Game updatedGame, IGameService gameService) =>
            {
                var result = await gameService.UpdateGameAsync(gameId, updatedGame);
                return result.Success ? Results.Ok(result) : Results.NotFound(result);
            });

            group.MapDelete(EndpointsRoutes.Games.delete, async (int gameId, IGameService gameService) =>
            {
                var result = await gameService.DeleteGameAsync(gameId);
                return result.Success ? Results.Ok(result) : Results.NotFound(result);
            });

            return group;
        }
    }
}
