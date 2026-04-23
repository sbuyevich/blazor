# Task 02 - Data And Database

## Goal
Add the server-side data model and SQLite persistence layer for users and projects.

## Work
- Add EF Core packages required for SQLite.
- Create `ApplicationDbContext`.
- Add the `AppUser` entity:
  - `Id`
  - `UserName`
  - `PasswordHash`
- Add the `StudentProject` entity:
  - `Id`
  - `Name`
  - `Description`
  - `OwnerUserId`
  - `CreatedAtUtc`
- Register `AddDbContextFactory<ApplicationDbContext>` in `Program.cs`.
- Configure the SQLite connection string.
- Add startup database initialization and seed logic.
- Seed one default user with a hashed password.

## Deliverables
- EF Core + SQLite configured in the app
- `ApplicationDbContext`
- `AppUser` and `StudentProject` models
- startup seed for the default user

## Acceptance Criteria
- The app creates or migrates the SQLite database on startup.
- The database contains tables for users and student projects.
- The default seeded user exists with a hashed password.

## Notes
- Keep passwords hashed even though auth is intentionally minimal.
- Ownership relationships must be represented in the database model.
