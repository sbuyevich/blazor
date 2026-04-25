# Task 01 - Solution Scaffold

## Goal

Create `my-class` as a separate Blazor Web App solution with its own project, configuration, and runtime settings.

## Work

- Create a new solution under `my-class`.
- Scaffold a new Blazor Web App targeting `net10.0`.
- Use `Interactive Server` rendering.
- Do not enable a built-in authentication template.
- Add required EF Core SQLite package references.
- Configure app settings for teacher credentials and the SQLite connection string.
- Ensure launch settings are local to the `my-class` project.

## Deliverables

- `my-class` solution file
- Blazor Web App project
- Local `appsettings` configuration
- Local `launchSettings.json`
- Required package references

## Acceptance Criteria

- The app builds as an independent solution.
- The app does not reference `student-projects`.
- The app can run without using the `student-projects` database or configuration.
- The project targets `net10.0`.

## Notes

- Keep the generated Blazor structure recognizable.
- Do not add controllers, Minimal APIs, or ASP.NET Core Identity.
