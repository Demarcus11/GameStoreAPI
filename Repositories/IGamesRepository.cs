using C___ASP.NET_Core_.Entities;

namespace C___ASP.NET_Core_.Repositories;

public interface IGamesRepository 
{
    Task CreateAsync (Game game);
    Task DeleteAsync (int id);
    Task<Game?> GetAsync(int id);
    Task<IEnumerable<Game>> GetAllAsync();
    Task UpdateAsync(Game updatedGame);
}
