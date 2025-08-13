using GameStore.Services;
using GameStore.Shared.Endpoints;
using GameStore.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Endpoints
{
    public static class GameRatingEndpoints
    {
        public static RouteGroupBuilder MapGameRatingsEndpoints(this WebApplication app)
        {
            var group = app.MapGroup(EndpointsRoutes.GameRating._base);

            group.MapGet("/", async (IGameRatingService service, [FromQuery] int userId, [FromQuery] int gameId) =>
            {
                var response = await service.GetGameRatingAsync(userId, gameId);
                return response.Success ? Results.Ok(response) : Results.NotFound(response);
            });

            group.MapPost(EndpointsRoutes.GameRating.update, [Authorize] async (IGameRatingService service, GameRating gameRating) =>
            {
                var response = await service.UpdateRatingAsync(gameRating);
                return response.Success ? Results.Ok(response) : Results.BadRequest(response);
            });

            group.MapGet(EndpointsRoutes.GameRating.getRatingsForGame, async (IGameRatingService service, int gameId) =>
            {
                var response = await service.GetRatingsForGameAsync(gameId);
                return response.Success ? Results.Ok(response) : Results.BadRequest(response);
            });

            group.MapGet(EndpointsRoutes.GameRating.getRatingsByUser, async (IGameRatingService service, int userId) =>
            {
                var response = await service.GetRatingsByUserAsync(userId);
                return response.Success ? Results.Ok(response) : Results.BadRequest(response);
            });

            group.MapDelete(EndpointsRoutes.GameRating.delete, [Authorize] async (IGameRatingService service, int userId, int gameId) =>
            {
                var response = await service.DeleteRatingAsync(userId, gameId);
                return response.Success ? Results.Ok(response) : Results.BadRequest(response);
            });

            group.MapGet(EndpointsRoutes.GameRating.topRating, async (IGameRatingService service, int count) =>
            {
                var response = await service.GetTopRatedGamesAsync(count);
                return response.Success ? Results.Ok(response) : Results.BadRequest(response);
            });

            return group;
        }
    }
}
