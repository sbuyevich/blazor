# Task 04 - Sign Up

## Goal
Add a minimal registration page so a new user can create an account without introducing ASP.NET Core Identity UI.

## Work
- Extend `IAuthService` and `AuthService` with user registration behavior.
- Validate that a new username is unique before creating the record.
- Hash the password before storing it in the database.
- Create a `/signup` page with:
  - username field
  - password field
  - confirm password field
- Show validation errors for:
  - missing fields
  - mismatched passwords
  - duplicate usernames
- Redirect successful registration into the normal authenticated flow.

## Deliverables
- registration method on the auth service
- `/signup` page
- hashed-password user creation flow

## Acceptance Criteria
- A new user can register with a unique username and password.
- The stored password is hashed, not plain text.
- Duplicate usernames are rejected with a clear message.
- Successful registration signs the user in or redirects them into the login flow.

## Notes
- Keep the UI intentionally minimal.
- Do not introduce account recovery, email confirmation, or profile management.
