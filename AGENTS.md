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
- Treat every code change as requiring a review before finishing, whether the user asks for implementation or says “check my new codes.” For review-only requests, inspect the relevant changed code and report concrete findings with file and location. Do not edit code during a review unless the user explicitly authorizes the specific changes first. If there are no findings, say so plainly.
- After the user approves a proposed fix and the agent applies and reviews it, ask the user to commit the changes. Do not create a commit unless the user explicitly asks for one.
- When writing or changing code, review the changed code for readability, correctness, maintainability, and comments. Add concise comments to new types and public members, including DTO properties, to explain their purpose or contract. Comments should add meaning rather than restate the code; do not comment every line.
- After code changes, update `README.md` when the change affects user-visible behavior, project structure, configuration, setup, or run/build instructions. Update this `AGENTS.md` when the change affects project conventions, architecture guidance, or recurring instructions for future agents. Keep both documents accurate and concise; do not edit them for implementation details that do not help users or future contributors understand or operate the project.
- In the final response, call out any meaningful code quality issues or missing explanatory comments you find. Suggest a concrete improvement and explain why it is better in plain language. Separate optional suggestions from changes actually made; do not silently make unrelated or behavior-changing improvements.
- Preserve existing behavior unless the task calls for a behavior change. Keep changes focused and avoid editing generated or third-party assets under `wwwroot/lib`.
- Do not put credentials in tracked configuration. The API's domain registration requires `ConnectionStrings:DefaultConnection`; use user secrets or an environment variable for local configuration.
- Do not add `.env` files or a dotenv dependency just for local settings unless the task specifically requires dotenv support; .NET does not load `.env` files by default. The standard environment variable is `ConnectionStrings__DefaultConnection`.
- Never open, read, print, search, copy, or otherwise inspect `.env`. Treat it as private local configuration; use `.env.example` for configuration shape and keys. Do not include `.env` contents in logs, tool output, commits, or responses.
- The solution targets .NET 10. Follow the existing nullable reference type and implicit using settings.
- Add or update tests for behavior changes when appropriate. Do not claim the test suite passes unless it has been run.

## Per-task review

Before finishing a code task:

1. Review the code written or changed in that task for understandable names, simple structure, likely edge cases, and consistency with nearby code.
2. Check that comments explain why or clarify behavior where needed, rather than repeating what the code already says.
3. If you find an issue outside the requested change, explain the problem, propose a specific fix, and say what benefit it provides. Ask before making a separate behavior change when the user has not requested it.
4. Summarize what was changed and why. Mention relevant checks that were run and their results; never imply checks were run when they were not.

When the user says “check my new codes” (or asks to check/review recent code), treat it as an explicit code review request: inspect the new or changed code and report correctness and maintainability issues with file locations, along with proposed fixes. This request does not authorize edits. Wait for the user's explicit approval before changing code, even when a fix appears clear or within scope. Do not require the user to repeat these review expectations. If the intended code changes are unclear, review the most recent changes in the workspace and state what you reviewed.

## Useful commands

```sh
dotnet build MyProject.slnx
dotnet test tests/UnitTests/UnitTests.csproj
dotnet run --project src/Api/Api.csproj
dotnet run --project src/WebApp/WebApp.csproj
```
