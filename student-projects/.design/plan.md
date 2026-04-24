# Student Projects MVP Plan

## Summary
Build a new `Blazor Web App` in `student-projects` using `net10.0`, `Interactive Server` rendering, `SQLite`, and `EF Core`. Do not add a separate HTTP API layer. The UI calls injected server-side services directly, and those services perform CRUD through EF Core.

Use a minimal custom cookie-auth flow instead of ASP.NET Core Identity. This keeps the account surface small while still supporting login, logout, and a basic sign-up page. Seed one default user at startup so the app has an immediate working account even before any manual registration occurs.

## Key Changes
- Scaffold a new app from `dotnet new blazor` with no built-in auth template.
- Configure `Program.cs` for:
  - Razor components + interactive server components
  - cookie authentication + authorization
  - `AddDbContextFactory<ApplicationDbContext>` with SQLite
  - startup database creation/migration and seed data
- Add two core entities:
  - `AppUser`: `Id`, `UserName`, `PasswordHash`
  - `StudentProject`: `Id`, `Name`, `Description`, `OwnerUserId`, `CreatedAtUtc`
- Add one EF Core `DbContext` with `DbSet<AppUser>` and `DbSet<StudentProject>`.
- Keep data access out of components:
  - `IAuthService` / `AuthService` for validating login credentials and returning the user identity
  - add registration support for creating a new `AppUser` with a hashed password
  - `IProjectService` / `ProjectService` for list/create/update/delete operations scoped to the current logged-in user
- Implement minimal auth UX:
  - `/login` page with username + password form
  - `/signup` page with username + password registration form
  - `/logout` action or page that clears the auth cookie
  - redirect unauthenticated users to `/login`
- Implement project pages:
  - `/projects` shows only the current user’s projects
  - create form for `Name` + `Description`
  - edit page/form for owned projects only
  - delete action with confirmation
- Enforce ownership on the server side inside `ProjectService`; UI checks are convenience only.
- Seed one default user on startup with a hashed password stored in the database.
- Do not create controllers, minimal APIs, or external AJAX calls for CRUD.

## Public Interfaces / Types
- `ApplicationDbContext`
- `AppUser`
- `StudentProject`
- `IAuthService`
- `IProjectService`
- Authentication cookie scheme name and login path configuration
- Pages/routes:
  - `/login`
  - `/signup`
  - `/projects`
  - `/projects/new`
  - `/projects/{id}/edit`

## Test Plan
- App starts and creates or migrates the SQLite database successfully.
- Seeded default user can log in with the configured credentials.
- Invalid login shows an error and does not create an auth cookie.
- A new user can register with a unique username and hashed password.
- Duplicate usernames are rejected cleanly.
- Unauthenticated access to `/projects` redirects to `/login`.
- Logged-in user can create a project with name and description.
- Logged-in user sees only their own projects.
- Edit updates only the owner’s record.
- Delete removes only the owner’s record.
- Attempting to edit or delete another user’s project is rejected server-side.
- Logout clears the auth cookie and protected pages require login again.

## Assumptions
- `net10.0` is the target because the machine has the .NET 10 SDK installed and this is a new app.
- Render mode is `Interactive Server`, not WebAssembly, because you want no separate API layer and direct server-side CRUD.
- Auth is intentionally minimal and production-light: custom cookie auth with a seeded local user and basic registration, not full ASP.NET Core Identity.
- v1 data model is intentionally small: only `Name` and `Description` for projects.
- Passwords should still be stored as hashes, not plain text, even in this minimal setup.
