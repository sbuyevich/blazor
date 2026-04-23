# Task 03 - Authentication

## Goal
Implement a minimal cookie-based login flow without ASP.NET Core Identity.

## Work
- Configure cookie authentication in `Program.cs`.
- Set the login path for protected routes.
- Add authorization support.
- Create `IAuthService` and `AuthService`.
- Validate username and password against the database.
- Build a minimal `/login` page with username and password fields.
- Create logout behavior at `/logout`.
- Redirect unauthenticated users to `/login`.

## Deliverables
- Cookie auth configuration
- `IAuthService`
- `AuthService`
- `/login` page
- `/logout` flow

## Acceptance Criteria
- Valid seeded credentials sign the user in and issue an auth cookie.
- Invalid credentials do not sign the user in and show an error.
- Protected pages redirect unauthenticated users to `/login`.
- Logging out clears the auth cookie.

## Notes
- Keep the login UI intentionally minimal.
- Do not introduce registration or ASP.NET Core Identity UI.
