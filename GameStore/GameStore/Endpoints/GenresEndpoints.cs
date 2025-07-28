using GameStore.Services;

namespace GameStore.Endpoints
{
    public static class GenresEndpoints
    {
        public static WebApplication MapGenreEndpoints(this WebApplication app)
        {
            app.MapGet("/genres", async (IGenreService genreService) =>
            {
                var response = await genreService.GetAllGenresAsync();
                return response.Success ? Results.Ok(response) : Results.BadRequest(response);
            });

            return app;
        }
    }
}
