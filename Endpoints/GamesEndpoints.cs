using GameStore.Api.Dtos;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGameById";
    private static readonly List<GameDto> games =
    [
        new GameDto(1, "GTA V", "Action", 29.99m, new DateOnly(2013, 9, 17)),
        new GameDto(2, "FIFA 22", "Sports", 59.99m, new DateOnly(2021, 10, 1)),
        new GameDto(3, "Cyberpunk 2077", "RPG", 49.99m, new DateOnly(2020, 12, 10))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games");

        group.MapGet("", () => games);

        group
            .MapGet(
                "/{id}",
                (int id) =>
                    games.FirstOrDefault(g => g.Id == id) is GameDto game
                        ? Results.Ok(game)
                        : Results.NotFound()
            )
            .WithName(GetGameEndpointName);

        group
            .MapPost(
                "",
                (CreateGameDto gameDto) =>
                {
                    var game = new GameDto(
                        games.Max(g => g.Id) + 1,
                        gameDto.Name,
                        gameDto.Genre,
                        gameDto.Price,
                        gameDto.ReleaseDate
                    );
                    games.Add(game);
                    return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
                }
            )
            .WithParameterValidation();

        group.MapPut(
            "/{id}",
            (int id, UpdateGameDto updatedGame) =>
            {
                var game = games.FirstOrDefault(g => g.Id == id);
                if (game is null)
                {
                    return Results.NotFound();
                }
                var updated = game with
                {
                    Name = updatedGame.Name,
                    Genre = updatedGame.Genre,
                    Price = updatedGame.Price,
                    ReleaseDate = updatedGame.ReleaseDate
                };
                var index = games.IndexOf(game);
                games[index] = updated;
                return Results.NoContent();
            }
        );

        //Delete game
        group.MapDelete(
            " /{id}",
            (int id) =>
            {
                var game = games.FirstOrDefault(g => g.Id == id);
                if (game is null)
                {
                    return Results.NotFound();
                }
                games.Remove(game);
                return Results.NoContent();
            }
        );
        return group;
    }
}
