# Student Projects Tasks

This folder breaks the MVP plan into implementation tasks.

## Task List
- [01-app-scaffold.md](./01-app-scaffold.md) - create the Blazor Web App and base server configuration
- [02-data-and-db.md](./02-data-and-db.md) - add EF Core, SQLite, entities, DbContext, and seed data
- [03-authentication.md](./03-authentication.md) - implement minimal cookie authentication and login/logout flow
- [04-sign-up.md](./04-sign-up.md) - add a minimal sign-up page and registration flow for new users
- [05-project-services.md](./05-project-services.md) - implement server-side project CRUD services with ownership checks
- [06-project-ui.md](./06-project-ui.md) - build the protected project pages and forms
- [07-testing-and-validation.md](./07-testing-and-validation.md) - verify the app behavior matches the MVP acceptance criteria

## Shared Constraints
- Target `net10.0`
- Use `Blazor Web App` with `Interactive Server`
- Use `SQLite` with `EF Core`
- Do not create a separate API layer
- Keep data access in server-side services, not UI components
- Use minimal custom cookie authentication, not ASP.NET Core Identity
