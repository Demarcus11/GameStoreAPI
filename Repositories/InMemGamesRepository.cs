using C___ASP.NET_Core_.Entities;

namespace C___ASP.NET_Core_.Repositories;

public class InMemGamesRepository : IGamesRepository
{
    private readonly List<Game> games =
    [
        new Game() 
        {
            Id = 1,
            Name = "Mortal Kombat 1",
            Genre = "Fighting",
            Price = 19.99M,
            ReleaseDate = new DateTime(1991, 2, 1),
            ImageUri = "https://placehold.co/100"
        },
        new Game() 
        {
            Id = 2,
            Name = "The Last of Us",
            Genre = "Action-Adventure",
            Price = 59.99M,
            ReleaseDate = new DateTime(2013, 5, 14),
            ImageUri = "https://placehold.co/100"
        },
        new Game() 
        {
            Id = 3,
            Name = "College Football 25",
            Genre = "Sports",
            Price = 69.99M,
            ReleaseDate = new DateTime(2024, 6, 19),
            ImageUri = "https://placehold.co/100"
        }
    ];
    
    // Retrieve all games, IEnumerable is a generic type so the client can easily manage the list we return
    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await Task.FromResult(games);
    }

    // Get single game
    public async Task<Game?> GetAsync(int id)
    {
        return await Task.FromResult(games.Find(game => game.Id == id));
    }

    // Create new game
    public async Task CreateAsync(Game game)
    {
        game.Id = games.Max(game => game.Id) + 1;
        games.Add(game);

        await Task.CompletedTask;
    }

    // Update a game
    public async Task UpdateAsync(Game updatedGame)
    {
        var index = games.FindIndex(game => game.Id == updatedGame.Id);
        games[index] = updatedGame;

        await Task.CompletedTask;
    }
        
    // Delete a game    
    public async Task DeleteAsync(int id)
    {
        var index = games.FindIndex(game => game.Id == id);
        games.RemoveAt(index);

        await Task.CompletedTask;
    }   
}
