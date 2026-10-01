# MyProject

MyProject is a .NET 10 solution with an ASP.NET Core API and a Blazor web app. Both apps share projects for database access, domain logic, and common types.

## Projects

- `src/Api` — ASP.NET Core API. Domain rule errors are returned as HTTP 400 JSON responses with a stable `code` and an English `message`.
- `src/WebApp` — Blazor interactive server app with Home, Counter, and Weather pages.
- `src/Domain` — application services and database registration.
- `src/Database` — EF Core context, entities, mappings, and migrations. MySQL is the configured database provider.
- `src/Contracts` and `src/Shared` — types and resources shared between projects.
- `tests/UnitTests` — xUnit test project.

## Requirements and configuration

- .NET SDK 10.0
- MySQL, for running the API

The API requires a connection string named `DefaultConnection`. Keep credentials out of source control. For local development, configure .NET user secrets in the API project or set the `ConnectionStrings__DefaultConnection` environment variable. A MySQL connection string looks like `Server=localhost;Database=myproject;User=app;Password=...`.

## Run the apps

Run commands from the repository root. Start either application independently:

```sh
dotnet run --project src/Api/Api.csproj
dotnet run --project src/WebApp/WebApp.csproj
```

The API publishes its OpenAPI document in Development mode. Local URLs are configured in each app's `Properties/launchSettings.json`.

## Error responses

When a domain rule rejects an API request, the API returns HTTP 400 with a body like:

```json
{
  "code": "EMAIL_REQUIRED",
  "message": "Please enter an email address."
}
```

The codes are defined in `src/Shared/Constants/ErrorCodes.cs`; English messages are in `src/Shared/Resources/Messages.en.json`. The API logs the error code and request path.

## Build and test

```sh
dotnet build MyProject.slnx
dotnet test tests/UnitTests/UnitTests.csproj
```

## Contributing

Keep API, UI, domain, and persistence responsibilities in their existing projects. Add or update focused tests when changing behavior, and keep this README current when setup or user-visible behavior changes.
