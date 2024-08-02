using GameStore.api.Dtos;

namespace GameStore.api.EndPoints;

public static class GamesEndPoints
{

    const string GetGameEndpointName = "GetGame";

    private readonly static List<GameDto> games = [
    new (
        1,
        "Street Fighter II",
        "Fighting",
        19.99M,
        new DateOnly(1992, 7, 15)
    ),
    new (
        2,
        "Final Fantasy XIV",
        "RolePlaying",
        59.99M,
        new DateOnly(2010, 9, 30)
    ),
    new (
        3,
        "FIFA 23",
        "Sports",
        69.99M,
        new DateOnly(2022, 9, 27)
    )
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
                        .WithParameterValidation();

        //GET /games
        group.MapGet("/", () => games);

        //GET /games/1
        group.MapGet("/{id}", (int id) => 
        {
            GameDto? game = games.Find(game => game.Id == id);

            return game is null ? Results.NotFound() : Results.Ok(game);

        })
        .WithName(GetGameEndpointName);

        //POST /games
        group.MapPost("/", (CreateGameDto newGame)    =>
        {

            /*if (string.IsNullOrEmpty(newGame.Name))
            {
                Results.BadRequest("Name is required");
            }*/



            GameDto game = new(
                games.Count + 1,
                newGame.Name,
                newGame.Genre,
                newGame.Price,
                newGame.ReleaseDate
            );
            games.Add(game);

            return Results.CreatedAtRoute(GetGameEndpointName, new {id = game.Id}, game);

        });
        //.WithParameterValidation();

        //PUT /games
        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
        {
            var index = games.FindIndex(game => game.Id == id);

            if (index == -1)
            {
                return Results.NotFound();
            }

            games[index] = new GameDto(
                id,
                updatedGame.Name,
                updatedGame.Genre,
                updatedGame.Price,
                updatedGame.ReleaseDate
            );

            return Results.NoContent();

        });

        //DELTE /games
        group.MapDelete("/{id}", (int id) =>
        {
        games.RemoveAll(game => game.Id == id);

        return Results.NoContent();

        });

        //app.MapGet("/", () => "Hello World!");

        return group;

    }

}
