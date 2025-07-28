namespace GameStore.Shared.Endpoints
{
    public static class EndpointsRoutes
    {
        public static class Games
        {
            public const string _base = "/games";
            public const string getById = "/{gameId:int}";
            public const string update = "/{gameId:int}";
            public const string delete = "/{gameId:int}";
            public static string Update(int id) => $"{_base}/{id}";
            public static string Delete(int id) => $"{_base}/{id}";
            public static string GetById(int id) => $"{_base}/{id}";
            public static string Add(int id) => $"{_base}/{id}";
        }

        public static class User
        {
            public const string _base = "/users";
            public const string register = "/register";
            public const string login = "/login";
            public const string logout = "/logout";
            public const string resetPass = "/resetPass";
            public const string update = "/{userId:int}";
            public const string delete = "/{userId:int}";
            public static string GetById(int userId) => $"{_base}/{userId}";
            public static string Update(int userId) => $"{_base}/{userId}";
            public static string Delete(int userId) => $"{_base}/{userId}";
        }

        public static class Game
        {
            public const string _base = "/ratings";
            public const string update = "/{userId}/{gameId}";
            public const string delete = "/{userId}/{gameId}";
            public const string getRatingsForGame = "/{gameId}";
            public const string getRatingsByUser = "/user/{userId}";
            public const string topRating = "/top/{count:int}";

            public static string GetGameRating(int userId, int gameId) =>
                $"{_base}?userId={userId}&gameId={gameId}";
            public static string UpdateRating(int userId, int gameId) =>
                $"{_base}/{userId}/{gameId}";
            public static string GetRatingsForGame(int gameId) =>
                $"{_base}/{gameId}";
            public static string GetRatingsByUser(int userId) =>
                $"{_base}/user/{userId}";
            public static string DeleteRating(int userId, int gameId) =>
                $"{_base}/{userId}/{gameId}";
            public static string TopRating(int count) =>
                $"{_base}/top/{count}";
        }
    }
}
