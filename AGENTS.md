# Agent guide

## Project map

- `src/Api`: ASP.NET Core controller API; currently includes a sample weather endpoint.
- `src/WebApp`: Blazor interactive server UI.
- `src/Domain`: dependency registration and domain services; `FeatureManager.AddDomain` configures the EF Core context.
- `src/Database`: EF Core `AppDbContext`, entities, and configurations. The configured provider is MySQL with snake_case naming.
- `src/Contracts` and `src/Shared`: cross-project types.
- `tests/UnitTests`: xUnit tests.

## Working conventions

- Keep API, UI, domain, and persistence responsibilities in their existing projects.
- When writing or changing code, review the changed code for readability, correctness, maintainability, and useful comments. Add comments where they explain intent, a non-obvious decision, or a constraint. Do not add comments that merely narrate simple syntax, and do not comment every line.
- In the final response, call out any meaningful code quality issues or missing explanatory comments you find. Suggest a concrete improvement and explain why it is better in plain language. Separate optional suggestions from changes actually made; do not silently make unrelated or behavior-changing improvements.
- Preserve existing behavior unless the task calls for a behavior change. Keep changes focused and avoid editing generated or third-party assets under `wwwroot/lib`.
- Do not put credentials in tracked configuration. The API's domain registration requires `ConnectionStrings:DefaultConnection`; use user secrets or an environment variable for local configuration.
- The solution targets .NET 10. Follow the existing nullable reference type and implicit using settings.
- Add or update tests for behavior changes when appropriate. Do not claim the test suite passes unless it has been run.

## Per-task review

Before finishing a code task:

1. Review the code written or changed in that task for understandable names, simple structure, likely edge cases, and consistency with nearby code.
2. Check that comments explain why or clarify behavior where needed, rather than repeating what the code already says.
3. If you find an issue outside the requested change, explain the problem, propose a specific fix, and say what benefit it provides. Ask before making a separate behavior change when the user has not requested it.
4. Summarize what was changed and why. Mention relevant checks that were run and their results; never imply checks were run when they were not.

## Useful commands

```sh
dotnet build MyProject.slnx
dotnet test tests/UnitTests/UnitTests.csproj
dotnet run --project src/Api/Api.csproj
dotnet run --project src/WebApp/WebApp.csproj
```
