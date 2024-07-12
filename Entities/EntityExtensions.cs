using C___ASP.NET_Core_.Dtos;

namespace C___ASP.NET_Core_.Entities;

public static class EntityExtensions
{
    // Method that extends Game and turns a game object into a game DTO object
    public static GameDto ToDto(this Game game)
    {
        return new GameDto(
            game.Id,
            game.Name,
            game.Genre,
            game.Price,
            game.ReleaseDate,
            game.ImageUri
        );
    }
}
