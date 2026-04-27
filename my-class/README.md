# MyClass

MyClass is a .NET 10 Blazor Web App for classroom use on a local Wi-Fi network. Teachers and students use browser pages backed by a local SQLite database, with the UI built using MudBlazor.

## Prerequisites

- .NET 10 SDK

## Run The App

From `my-class/src`:

```powershell
dotnet build .\MyClass.slnx
dotnet run --project .\MyClass.Web\MyClass.Web.csproj --launch-profile http
```

Open the app at:

```text
http://localhost:5555
```

To open the demo class context, use:

```text
http://localhost:5555/?c=demo
```

## Default Local Login

Teacher credentials are configured in `src/MyClass.Web/appsettings.json`.

```text
Username: t
Password: t
```

Students can register from the app when using a class link such as `/?c=demo`.

## Project Layout

- `src/MyClass.Web`: Blazor UI, pages, layout, app startup, and configuration.
- `src/MyClass.Core`: EF Core data model, services, options, authentication, quiz logic, students, and class context.
- `.assets/quiz`: Local quiz content used by the configured quiz loader.
- `src/database/my-class.db`: Local SQLite database.
- `.design`: Business notes, staged plans, and task breakdowns.
- `AGENTS.md`: Agent/developer guidance for working safely in this project.

## Quiz Content

Quiz content is read from `.assets/quiz`.

- Root quiz file: `.assets/quiz/quiz.json`.
- Each question folder contains a question image, `q.JPG` or `q.jpg`, and metadata in `q.json`.

## Troubleshooting

- If `dotnet build` says `MyClass.Web.exe` is locked, stop the running app process and build again.
- Avoid running multiple app instances against the same SQLite database. The app updates the database during startup, and SQLite can report `database is locked` when another process has it open.
