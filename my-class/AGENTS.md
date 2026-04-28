# AGENTS.md

## Project Overview

This repository contains `MyClass`, a .NET 10 Blazor Web App for classroom use on a local Wi-Fi network. Teachers and students use browser pages backed by SQLite data. The UI uses MudBlazor and interactive server rendering.

Primary code lives under `my-class/src`:

- `MyClass.Web`: Blazor UI, layout, pages, app startup, configuration.
- `MyClass.Core`: EF Core data model, services, options, auth, quiz, students, class context.
- `database/my-class.db`: local SQLite database used by the app.
- `assets/quizzes/{quiz name}`: quiz content folder configured by `Quiz:RootFolder`.
- `.design`: staged business/design notes and task breakdowns.

## Commands

Run commands from `my-class/src` unless noted.

```powershell
dotnet build .\MyClass.slnx
dotnet run --project .\MyClass.Web\MyClass.Web.csproj --launch-profile http
```

The app runs at:

```text
http://localhost:5555
```

If build fails because `MyClass.Web.exe` is locked, stop the running app process first. The app initializes and updates the SQLite database during startup, so avoid starting multiple app instances against the same database.

## Architecture Notes

- `Program.cs` registers Razor components with interactive server rendering, MudBlazor, data protection keys under `.keys`, EF Core SQLite via `AddDbContextFactory<ApplicationDbContext>`, and scoped app services.
- The database is created and compatibility-updated by `DatabaseInitializer.InitializeAsync(app.Services)` on startup. EF migrations are not currently used.
- Authentication is custom. Do not introduce ASP.NET Core Identity or cookie auth unless explicitly requested.
- Login state is convenience browser state, stored via `ISessionStorageService` under `my-class.loginState`. Server-side services must still enforce role and class scope.
- Teacher credentials come from `appsettings.json` under `Teacher`.
- Students self-register and are stored in the `Students` table with PBKDF2-SHA256 password hashes.
- Current class context is resolved from the `c` query parameter, such as `/?c=demo`, and stored for later pages.

## Data And Services

- Use `IDbContextFactory<ApplicationDbContext>` in services rather than injecting a long-lived `ApplicationDbContext`.
- Keep business rules in `MyClass.Core\Services`; Blazor components should call services and render state.
- Existing service result types use explicit `Succeeded`, `Message`, and payload properties. Follow that pattern for new service APIs.
- `School`, `Class`, `Student`, and `QuizAnswer` are the current main entities.
- `Student.UserName` is unique per class. `Student.IsActive` controls quiz participation.
- Be careful with SQLite locks. Do not run multiple local app instances or database-mutating utilities at the same time.

## Quiz Behavior

Stage 4 quiz behavior is the current model:

- Quiz content is loaded from `Quiz:RootFolder`, currently `../assets/quizzes/{quiz name}`.
- Root `quiz.json` uses `title` and `TimeLimitSeconds`.
- Each question subfolder contains `q.jpg` and `q.json`.
- Question `q.json` includes `correctAnswer` as a string and may include `TimeLimitSeconds`.
- Root `TimeLimitSeconds` is the default; question `TimeLimitSeconds` overrides it when present.
- There is one active quiz run at a time.
- Do not recreate `QuizSessions` or `QuizSessionQuestions`; current quiz state comes from `QuizAnswers`.
- Starting or restarting a quiz clears `QuizAnswers` and creates rows for the first question.
- Moving next creates one denormalized `QuizAnswers` row per active student for the new question.
- Student answers are strings: `"1"`, `"2"`, `"3"`, or `"4"`.
- Empty `Answer` means timeout/no answer and is incorrect.
- `IsCorrect` is computed by exact string comparison between `Answer` and `CorrectAnswer`.

## UI Conventions

- Use MudBlazor components consistently.
- Pages live under `MyClass.Web/Components/Pages`.
- Shared UI components live under `MyClass.Web/Components`.
- Quiz UI lives under `MyClass.Web/Components/Quiz`.
- Role-specific access is handled with `RoleGate`; keep teacher-only pages and student-only pages guarded.
- Navigation is in `Components/Layout/NavMenu.razor`.
- Keep UI text concise and action-oriented for classroom use.

## Development Guidelines

- Prefer small, scoped changes that match existing service/component patterns.
- Do not move business rules into Razor components when a service should own them.
- Preserve existing custom auth and class-context behavior.
- Do not replace the startup database initializer with migrations unless the task explicitly asks for a migration strategy.
- Do not edit local database files unless the task requires changing seeded/current data.
- Use `rg` for searching.
- Use `dotnet build .\MyClass.slnx` as the baseline verification.

## Documentation Lookup

When answering or implementing questions about libraries, frameworks, SDKs, APIs, CLI tools, or cloud services, use Context7 MCP for current documentation before relying on memory. This applies to .NET, ASP.NET Core, Blazor, EF Core, MudBlazor, and related tooling.
