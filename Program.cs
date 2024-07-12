using C___ASP.NET_Core_.Data;
using C___ASP.NET_Core_.Routes;



var builder = WebApplication.CreateBuilder(args);
// Register services
builder.Services.AddRepositories(builder.Configuration);

var server = builder.Build();

// apply migrations on server startup
await server.Services.InitializeDbAsync();

// Routes
server.MapGamesRoutes();

// Start server
server.Run();
