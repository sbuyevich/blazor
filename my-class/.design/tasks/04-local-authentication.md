# Task 04 - Local Authentication

## Goal

Implement the shared login page and browser-local login state for teachers and students.

## Work

- Add a shared `/login` page.
- Add `TeacherOptions` bound from app settings.
- Add `LoginState` for username, `IsTeacher`, and current class code.
- Add a small local storage abstraction for reading, writing, and clearing login state.
- Add `IAuthService` and `AuthService`.
- Validate teacher credentials against app settings.
- Validate existing student credentials against the `Student` table for the current class.
- Store successful login state in browser `localStorage`.
- Ensure server-side operations validate role and class scope instead of trusting `localStorage` alone.

## Deliverables

- `/login` page
- `TeacherOptions`
- `LoginState`
- Local storage auth-state helper
- `IAuthService`
- `AuthService`

## Acceptance Criteria

- Teacher can log in through the shared login page.
- Teacher login stores username, `IsTeacher = true`, and class code in `localStorage`.
- Existing student can log in through the shared login page.
- Student login stores username, `IsTeacher = false`, and class code in `localStorage`.
- Invalid credentials show an error and do not update login state.

## Notes

- This is intentionally not ASP.NET Core Identity.
- Browser state is convenience state, not a trusted authorization boundary.
- Do not build teacher registration or multiple teacher accounts.
- `TeacherOptions` represents the single global teacher account.
- Do not build a logout page for this task.
