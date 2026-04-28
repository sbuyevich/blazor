---
name: blazor-project-context
description: Use for repo-specific work in the MyClass Blazor project, especially when implementing classroom, student, quiz, database, MudBlazor UI, SignalR, custom auth, project convention, or design-artifact changes in this repository.
---

# Blazor Project Context

## Overview

This skill contains repository-local context for the `MyClass` app in `my-class/`.

`MyClass` is a .NET 10 Blazor Web App for classroom use on a local Wi-Fi network. Teachers and students use browser pages backed by SQLite data. The UI uses MudBlazor and interactive server rendering.

Use this skill when working on:
- classroom, student, quiz, or school/class features
- MudBlazor UI in the existing Blazor Web App
- EF Core SQLite data model and service changes
- custom teacher/student auth and class-context behavior
- SignalR quiz notifications
- repository-specific design artifacts, project conventions, and implementation workflow

## Workflow

1. Read [references/project-structure.md](references/project-structure.md) first to understand the current repo shape.
2. Read [references/current-plan.md](references/current-plan.md) when the task touches active or planned product work.
3. Read `my-class/AGENTS.md` before code changes; it is the authoritative working guide for commands, architecture, and project constraints.
4. Prefer repo-local planning artifacts under `my-class/.design/` over assumptions.
5. Keep new reusable project knowledge inside this plugin's `skills/` or `references/` folders when it belongs in source control.

## Current Project Direction

- App: `.NET 10` Blazor Web App with interactive server rendering.
- UI: MudBlazor.
- Persistence: EF Core with SQLite at `my-class/src/database/my-class.db`.
- Data startup: `DatabaseInitializer.InitializeAsync(app.Services)` creates and compatibility-updates the database; EF migrations are not currently used.
- Authentication: custom teacher/student login state, not ASP.NET Core Identity.
- Class context: resolved from the `c` query parameter, such as `/?c=demo`.
- Quiz flow: active quiz state is stored in denormalized `QuizAnswers` rows.

## Where To Look

- Primary code: `my-class/src/`
- Web project: `my-class/src/MyClass.Web/`
- Core project: `my-class/src/MyClass.Core/`
- Design artifacts: `my-class/.design/`
- App guide: `my-class/AGENTS.md`
- User guide: `my-class/README.md`
- Project-local skill files: `plugins/blazor-project/skills/blazor-project-context/`

## Implementation Notes

- Run commands from `my-class/src` unless a task says otherwise.
- Use `dotnet build .\MyClass.slnx` as the baseline verification.
- Use `dotnet run --project .\MyClass.Web\MyClass.Web.csproj --launch-profile http` to run the app at `http://localhost:5555`.
- Stop an existing app process before building if `MyClass.Web.exe` is locked.
- Use `IDbContextFactory<ApplicationDbContext>` in services instead of injecting a long-lived `ApplicationDbContext`.
- Keep business rules in `MyClass.Core\Services`; Blazor components should call services and render state.
- Keep teacher-only and student-only UI guarded with the existing `RoleGate`.
- Avoid editing local database files unless the task explicitly requires seed/current data changes.

## Notes

- Keep personal or machine-specific data out of this skill.
- Prefer updating these repo-local references instead of storing project knowledge only in `~/.agents` or `~/.codex/skills`.
- Use Context7 MCP for current docs when a task asks about libraries, frameworks, SDKs, APIs, CLIs, or cloud services.
