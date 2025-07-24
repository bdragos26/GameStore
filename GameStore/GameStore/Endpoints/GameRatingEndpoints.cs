using System.Reflection.Metadata.Ecma335;
using GameStore.Services;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace GameStore.Endpoints
{
    public static class GameRatingEndpoints
    {
        public static RouteGroupBuilder MapGameRatingsEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/ratings");

            group.MapGet("/", async (IGameRatingService service, [FromQuery] int userId, [FromQuery] int gameId) =>
            {
                var response = await service.GetGameRatingAsync(userId, gameId);
                return response.Success ? Results.Ok(response) : Results.NotFound(response);
            });

            group.MapPost("/{userId}/{gameId}", async (IGameRatingService service, GameRating gameRating) =>
            {
                var response = await service.UpdateRatingAsync(gameRating);
                return response.Success ? Results.Ok(response) : Results.BadRequest(response);
            });

            group.MapGet("/{gameId}", async (IGameRatingService service, int gameId) =>
            {
                var response = await service.GetRatingsForGameAsync(gameId);
                return response.Success ? Results.Ok(response) : Results.BadRequest(response);
            });

            return group;
        }
    }
}
