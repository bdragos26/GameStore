using GameStore.Data;
using GameStore.Services;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Endpoints
{
    public static class GenresEndpoints
    {
        public static WebApplication MapGenreEndpoints(this WebApplication app)
        {
            app.MapGet("/genres", async (IGenreService genreService) =>
                Results.Ok(await genreService.GetAllGenresAsync()));

            return app;
        }
    }
}
