using C___ASP.NET_Core_.Dtos;
using C___ASP.NET_Core_.Entities;
using C___ASP.NET_Core_.Repositories;

namespace C___ASP.NET_Core_.Routes;

// static classes cannot be instaniated, it's used to group static methods.
public static class GamesRoutes
{
    const string GetGameEndpointName = "GetGame";

    // extension method that maps all API routes (extension methods are always atatic and use the "this" keyword)
    public static RouteGroupBuilder MapGamesRoutes(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/games")
                .WithParameterValidation();

        // GET - Get all games
        group.MapGet("/", async (IGamesRepository repository) => 
        (await repository.GetAllAsync()).Select(game => game.ToDto())); // turn each game into a game DTO

        // GET - Get single game
        group.MapGet("/{id}", async (IGamesRepository repository, int id) => 
        {
            Game? game = await repository.GetAsync(id);

            return game is not null ? Results.Ok(game.ToDto()) : Results.NotFound();
        }).WithName(GetGameEndpointName); // this name is used to locate an endpoint using this route, for example, after we make a new game using a POST request, we find that game using this route.

        // POST - Create new game
        group.MapPost("/", async (IGamesRepository repository, CreateGameDto gameDto) =>
        {
            Game game = new()
            {
                Name = gameDto.Name,
                Genre = gameDto.Genre,
                Price = gameDto.Price,
                ReleaseDate = gameDto.ReleaseDate,
                ImageUri = gameDto.ImageUri,
            };

            await repository.CreateAsync(game);
            return Results.CreatedAtRoute(GetGameEndpointName, new {id = game.Id}, game); // To see location header in POSTMAN, make a request then open the console, click on the POST request and scroll down the Response Headers and you'll see 'Location: "https:localhost:5120/games/4"'. This is useful because it tells us where to the created game resource is located.
        });

        // PUT- Update game
        group.MapPut("/{id}", async (IGamesRepository repository, int id, UpdateGameDto updatedGameDto) =>
        {   
            Game? game = await repository.GetAsync(id);

            if (game is null)
            {
                return Results.NotFound($"No game with id {id}");
            }

            game.Name = updatedGameDto.Name;
            game.Genre = updatedGameDto.Genre;
            game.Price = updatedGameDto.Price;
            game.ReleaseDate = updatedGameDto.ReleaseDate;
            game.ImageUri = updatedGameDto.ImageUri;

            await repository.UpdateAsync(game);

            return Results.NoContent();
        });

        // DELETE - Delete game
        group.MapDelete("/{id}", async (IGamesRepository repository, int id) => 
        { 
           Game? game = await repository.GetAsync(id);
           
           if (game is not null)
           {
                await repository.DeleteAsync(id);
           }
         
           return Results.NoContent();
        });

        return group; // group is of type RouteGroupBuilder
    }
}
