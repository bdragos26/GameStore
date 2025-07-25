namespace GameStore.Client.Endpoints
{
    public static class EndpointsRoutes
    {
        public static class GamesRoutes
        {
            public const string baseRoute = "/games";
            public const string baseWithIdRoute = "/{gameId:int}";
            public static string baseWithIdApi(int id) => $"{baseRoute}/{id}";
        }
        
        public static class UserRoutes
        {
            public const string baseRoute = "/users";
            public const string registerRoute = "/register";
            public const string loginRoute = "/login";
            public const string logoutRoute = "/logout";
            public const string resetPassRoute = "/resetPass";
            public const string baseWithIdRoute = "/{id:int}";
            public static string baseWithIdApi(int id) => $"{baseRoute}/{id}";
        }

        public static class GameRatingRoutes
        {
            public const string baseRoute = "/ratings";
            public const string baseWithUserAndGameIdRoute = "/{userId}/{gameId}";
            public const string baseWithGameIdRoute = "/{gameId}";
            public const string baseWithUserIdRoute = "/user/{userId}";
            public static string GetRating(int userId, int gameId) =>
                $"{baseRoute}?userId={userId}&gameId={gameId}";
            public static string UpdateRating(int userId, int gameId) =>
                $"{baseRoute}/{userId}/{gameId}";
            public static string GetRatingsForGame(int gameId) =>
                $"{baseRoute}/{gameId}";
            public static string GetRatingsByUser(int userId) =>
                $"{baseRoute}/user/{userId}";
        }
    }
}
