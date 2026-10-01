using Domain;
using DotNetEnv;

// Search parent directories so the repository-root .env works when the API starts in src/Api.
// Load it before creating the host so ASP.NET Core can read its values as configuration.
Env.TraversePath().Load();

// Register the API endpoints and the services provided by the domain layer.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDomain(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Keep the OpenAPI document available during development without exposing it by default in production.
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
