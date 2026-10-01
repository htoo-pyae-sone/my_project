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

## Administrative user API

The API provides these administrative user endpoints:

- `GET /api/AdminUser` — list users that have not been deleted.
- `GET /api/AdminUser/{id}` — get a user by database ID.
- `POST /api/AdminUser` — create a user; returns HTTP 201 and a link to the new user.
- `PUT /api/AdminUser/{id}` — update a user's name, email, and optional active status.
- `DELETE /api/AdminUser/{id}` — soft-delete a user.

Successful responses include the `Result<T>` envelope described below. Deleting a user marks the account as deleted while preserving its record and audit history.

## API results and errors

Application services return a `Result<T>` to describe an operation's outcome. A successful result includes its data; a failed result includes a result type, a stable error code, and an English message. API controllers map the result type to an HTTP status: general errors, validation errors, invalid data, and bad requests use 400; duplicate records and conflicts use 409; missing records use 404; forbidden uses 403; unauthorized uses 401; and system errors use 500. Success and warnings use 200.

A result-based response has this shape (some values can be null when they do not apply):

```json
{
  "isSuccess": false,
  "isError": true,
  "type": "ValidationError",
  "code": "EMAIL_REQUIRED",
  "message": "Please enter an email address.",
  "target": null,
  "data": null
}
```

Domain rule violations raised as `DomainException` are handled by API middleware. They return HTTP 400 with a smaller body:

```json
{
  "code": "EMAIL_REQUIRED",
  "message": "Please enter an email address."
}
```

Error codes are stable identifiers defined in `src/Shared/Constants/ErrorCodes.cs`; their English messages are in `src/Shared/Resources/Messages.en.json`. The same shared message file is embedded for `Result<T>` and copied to API output for middleware. The API logs domain exception codes and request paths. When adding an error, add both the code constant and its matching message key.

## Build and test

```sh
dotnet build MyProject.slnx
dotnet test tests/UnitTests/UnitTests.csproj
```

## Contributing

Keep API, UI, domain, and persistence responsibilities in their existing projects. Add or update focused tests when changing behavior, and keep this README current when setup or user-visible behavior changes.
