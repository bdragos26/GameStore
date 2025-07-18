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
            var group = app.MapGroup("/games");
            //Get /games
            group.MapGet("/", async (IGameService gameService) =>
                Results.Ok(await gameService.GetAllGamesAsync()));

            //Get /games/id
            group.MapGet("/{id:int}", async (int id, IGameService gameService) =>
            {
                GameDetails? game = await gameService.GetGameByIdAsync(id);
                return game == null ? Results.NotFound() : Results.Ok(game);
            });

            //POST /games
            group.MapPost("/", async (GameDetails newGame, IGameService gameService) =>
            {
                await gameService.AddGameAsync(newGame);

                return Results.Created();
            });

            //PUT /games/id
            group.MapPut("/{id:int}", async (int id, GameDetails updatedGame, IGameService gameService) =>
            {
                var existingGame = await gameService.UpdateGameAsync(id, updatedGame);

                if (existingGame == null)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            });

            //DELETE /games/1
            group.MapDelete("/{id:int}", async (int id, IGameService gameService) =>
            {
                gameService.DeleteGameAsync(id);
                return Results.NoContent();
            });

            return group;
        }
    }
}
