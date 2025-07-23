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
                var rating = await service.GetGameRatingAsync(userId, gameId);
                return rating is null ? Results.NotFound() : Results.Ok(rating);
            });

            group.MapPost("/{userId}/{gameId}", async (IGameRatingService gameRatingService, GameRating gameRating) =>
            {
                var rating = await gameRatingService.UpdateRatingAsync(gameRating);
                return Results.Ok(rating);
            });

            group.MapGet("/{gameId}", async (IGameRatingService gameRatingService, int gameId) =>
            {
                var ratings = await gameRatingService.GetRatingsForGameAsync(gameId);
                return Results.Ok(ratings);
            });

            return group;
        }
    }
}
