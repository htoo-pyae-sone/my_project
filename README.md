# MyProject

MyProject is a .NET 10 solution containing a small ASP.NET Core API, a Blazor web app, and shared projects for database, domain, contracts, and common code. The API and web app are separate applications and can be started independently.

## Projects

- `src/Api` exposes controller-based HTTP endpoints. Its weather forecast endpoint currently returns generated sample data.
- `src/WebApp` is a Blazor interactive server app with Home, Counter, and Weather pages.
- `src/Domain` registers domain services and configures the database connection.
- `src/Database` contains the EF Core context, entities, and entity mappings. It uses MySQL and snake_case naming.
- `src/Contracts` and `src/Shared` are intended for types shared across projects.
- `tests/UnitTests` contains the xUnit test project.

## Requirements

- .NET SDK 10.0
- A MySQL connection string named `DefaultConnection` when starting the API

Configure the connection string outside source control, for example with .NET user secrets in the API project or the `ConnectionStrings__DefaultConnection` environment variable. The expected format is a MySQL connection string, such as `Server=localhost;Database=myproject;User=app;Password=...`. Do not commit real credentials. .NET does not load `.env` files by default, so setting that environment variable directly is the simplest local option.

## Run

From the repository root, start either app:

```sh
dotnet run --project src/Api/Api.csproj
dotnet run --project src/WebApp/WebApp.csproj
```

The API publishes its OpenAPI document in Development mode. Local URLs are configured in each project's `Properties/launchSettings.json`.

## Build and test

```sh
dotnet build MyProject.slnx
dotnet test tests/UnitTests/UnitTests.csproj
```

The test project is currently a starter scaffold; add focused tests as application behavior is implemented.
