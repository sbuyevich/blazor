# Task 01 - App Scaffold

## Goal
Create the base `Blazor Web App` in `student-projects` and configure the application shell for server-side interactivity.

## Work
- Scaffold a new app from `dotnet new blazor`.
- Target `net10.0`.
- Configure Razor components with interactive server components.
- Set up the base `Program.cs` pipeline.
- Keep the app free of controllers, minimal APIs, and external AJAX endpoints.

## Deliverables
- A runnable `Blazor Web App` project in `student-projects`
- `Program.cs` configured for:
  - Razor components
  - Interactive Server render mode
  - authentication and authorization middleware hooks
  - antiforgery and standard app pipeline behavior

## Acceptance Criteria
- The app builds and starts successfully.
- The app uses Blazor interactive server rendering.
- No separate API layer exists in the project.

## Notes
- This task establishes the host and pipeline only.
- Database and auth details are implemented in later tasks.
