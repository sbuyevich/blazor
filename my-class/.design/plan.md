# My Class MVP Plan

## Summary

Build a separate `my-class` solution with a new `Blazor Web App` using `net10.0`, `Interactive Server` rendering, SQLite, EF Core, and MudBlazor. The app has its own SQLite database and is independent from the existing `student-projects` solution. It is intended for classroom use on a local Wi-Fi network, with teachers and students opening the same web app in a browser.

Use a minimal custom login flow instead of ASP.NET Core Identity. The app uses one shared login page for teachers and students, stores login state in browser `localStorage`, and carries an `IsTeacher` flag across the app. Server-side operations must still validate role and class scope instead of trusting browser state alone.

## Key Changes

- Scaffold a new Blazor Web App with no built-in authentication template.
- Keep the solution, project files, app settings, EF Core migrations, and SQLite database under `my-class`.
- Add MudBlazor as the UI component library and use it for app shell, forms, alerts, buttons, menus, and data grids/tables.
- Configure `Program.cs` for:
  - Razor components with interactive server components
  - SQLite-backed EF Core data access
  - `IDbContextFactory<ApplicationDbContext>` for Blazor-safe database work
  - MudBlazor services with `AddMudServices()`
  - app settings binding for teacher credentials
  - startup database creation or migrations
- Add MudBlazor providers to the layout/app shell:
  - `MudThemeProvider`
  - `MudPopoverProvider`
  - `MudDialogProvider`
  - `MudSnackbarProvider`
- Add core entities:
  - `School`: school record shown in the app header
  - `Class`: belongs to a school and has a unique class code
  - `Student`: belongs to a class and stores student login/registration details
- Implement current class selection:
  - read the class code from the `c` query parameter on the landing page only
  - load the matching school and class
  - store the resolved current class context for later pages
  - show school and class names in the app header
  - show an error page or error panel when `c` is missing or invalid
- Implement shared login and local auth state:
  - `/login` page uses MudBlazor form controls and supports teacher and student login
  - teacher login validates against app settings
  - student login validates against the `Student` table for the current class
  - successful login writes username, `IsTeacher`, and current class code to `localStorage`
- Implement student self-registration:
  - allow students to register from the shared login flow when no matching student record exists
  - create the student record for the current class only
  - reject duplicate student usernames within the same class
- Keep data access and rules out of components:
  - `IClassContextService` resolves the active school/class from landing-page `c` and the stored current class context
  - `IAuthService` handles teacher/student login and registration
  - `IStudentService` handles current-class student grid queries
- Implement teacher UI:
  - show Students menu only for teacher users using MudBlazor navigation components
  - Students page shows a MudBlazor table/grid of students for the current class only
- Leave student post-login capabilities as `TBD`; after login, students can reach only placeholder content until requirements are defined.
- Do not add controllers, Minimal APIs, or external AJAX endpoints for v1 unless a later requirement needs them.

## Public Interfaces / Types

- `ApplicationDbContext`
- `School`
- `Class`
- `Student`
- `TeacherOptions`
- `LoginState`
- `IClassContextService`
- `IAuthService`
- `IStudentService`
- MudBlazor package and app-level providers
- Pages/routes:
  - `/?c={code}`
  - `/login`
  - `/students`
  - `/class-error`

## Test Plan

- App starts and creates or migrates the SQLite database successfully.
- A valid `c` query parameter loads the matching school and class and displays both names in the header.
- Missing `c` query parameter shows a clear error message.
- Invalid `c` query parameter shows a clear error message.
- `/login` and `/students` use the stored current class context and do not require `c` in the URL.
- Teacher can log in with app settings credentials from the shared login page.
- Login UI is built with MudBlazor form components.
- Teacher login stores username, `IsTeacher = true`, and class code in `localStorage`.
- Invalid teacher login shows an error and does not update login state.
- Existing student can log in from the shared login page for the current class.
- New student can self-register when no matching record exists.
- Duplicate student username in the same class is rejected cleanly.
- Student login stores username, `IsTeacher = false`, and class code in `localStorage`.
- Teacher can open the Students menu and view only students from the current class.
- Student grid is built with MudBlazor table/grid components.
- Student users cannot see or open the teacher Students page.

## Assumptions

- `net10.0` is the target because this is a new app and current ASP.NET Core guidance prefers the latest stable version.
- Render mode is `Interactive Server` so the app can use server-side services and SQLite directly without a separate API layer.
- `my-class` is a separate solution with its own project files, configuration, migrations, and SQLite database.
- MudBlazor is the standard UI library for app layout, forms, navigation, alerts, dialogs/snackbars, and tables/grids.
- The class code query parameter is named `c` and is only required on the landing page.
- Browser `localStorage` is used only for lightweight local classroom state; server-side services still enforce teacher/student permissions and current-class filtering.
- The MVP supports one global teacher account for the whole app, configured through app settings.
- Teacher credentials are stored in app settings for this local classroom MVP.
- Student passwords or secret values should be stored as hashes if a password field is used.
- Student capabilities and the final database field list remain `TBD` until the BRD defines them.

## Task Breakdown

- [Task 01 - Solution Scaffold](tasks/01-solution-scaffold.md)
- [Task 02 - Data and Database](tasks/02-data-and-database.md)
- [Task 03 - Class Context](tasks/03-class-context.md)
- [Task 04 - Local Authentication](tasks/04-local-authentication.md)
- [Task 05 - Student Registration](tasks/05-student-registration.md)
- [Task 06 - Teacher Student Grid](tasks/06-teacher-student-grid.md)
- [Task 07 - Verification](tasks/07-verification.md)
