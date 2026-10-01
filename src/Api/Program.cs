using Api.Configuration;
using Api.Middleware;
using DotNetEnv;
using Domain;

// Load the repository-root .env for local development before ASP.NET Core creates its configuration.
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSharedErrorMessages();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDomain(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<DomainExceptionMiddleware>();

app.UseAuthorization();

app.MapHealthChecks("/api/health");
app.MapControllers();

app.Run();
