namespace Movies.API;

public static class ApiEndPoints
{
    //This will be common across all API requests,
    //For Ex: https://localhost:3423/api/movies
    private const string ApiBase = "api";

    public static class Movies
    {
        //This will be common to Movies Comtroller.
        //so all the actions related to Movies Controller should follow this
        private const string MoviesRoute = $"{ApiBase}/movies";

        public const string Create = MoviesRoute;

        public const string Get = $"/{MoviesRoute}/{{Id:guid}}";
        public const string GetAll = MoviesRoute;
    }
}