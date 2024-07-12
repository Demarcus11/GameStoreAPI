using System.Reflection;
using C___ASP.NET_Core_.Entities;
using Microsoft.EntityFrameworkCore;

namespace C___ASP.NET_Core_.Data;

public class GameStoreContext : DbContext
{
    public GameStoreContext(DbContextOptions<GameStoreContext> options) : base(options)
    {       
    }

    public DbSet<Game> Games => Set<Game>();

    // Whenever the model is being created as part of the migration, the context is going to tell the migrations tooling that we have to apply the configuration we created (the decimal precision)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
