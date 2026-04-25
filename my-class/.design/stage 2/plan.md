# My Class MVP Plan. Stage 2.

## Summary

Stage 2 improves the post-login experience and teacher student management. The app should no longer expose Login as a normal navigation item. Instead, unauthenticated users are gated by a login modal, and successful teacher or student login redirects to `/home`.

The Students page becomes a more useful teacher tool by adding name search and a confirmed remove action. Removing a student permanently deletes that student from the current class, which forces that student to register again before logging in.

## Key Changes

- Replace navigation-based login with an unauthenticated gate:
  - remove the Login item from the navigation menu
  - show the login UI as a blocking MudBlazor modal/dialog when no login state exists
  - keep login state in `sessionStorage` under the existing key
  - redirect successful teacher and student logins to `/home`
- Keep student registration available from the login modal:
  - student can register when no matching `Student` record exists
  - successful registration stores `IsTeacher = false`
  - successful registration redirects to `/home`
- Enhance the teacher Students page:
  - add a search field above the grid
  - filter by first name, last name, and display name only
  - keep results scoped to the current class
  - add a row-level remove action
  - show a confirmation dialog before removing a student
  - hard-delete the selected student from the database
- Keep teacher-only protections:
  - only teacher login state can view the Students page
  - student users and unsigned-in users see a clear unauthorized message or are blocked by the login modal
  - removal must validate teacher state and current class scope server-side

## Public Interfaces / Types

- Extend `IStudentService` with teacher-only methods for:
  - querying students by current class and optional name search text
  - removing a student by id from the current class
- Add small result types only if needed to return success/failure messages for removal.
- Continue using existing `LoginState`, `IAuthService`, `ILoginStateService`, `ISessionStorageService`, and `ClassContext`.
- Do not add controllers, Minimal APIs, ASP.NET Core Identity, or external AJAX endpoints for this stage.

## Test Plan

- App opens to a login modal when no user is logged in.
- Navigation menu does not show a Login item.
- Teacher login with appsettings credentials stores teacher login state and redirects to `/home`.
- Student login stores student login state and redirects to `/home`.
- Student registration from the login modal still works and redirects to `/home`.
- Teacher can open `/students` and see only students from the current class.
- Student or unsigned-in users cannot use the Students page.
- Search filters students by first name, last name, and display name.
- Search does not match username unless it is also part of a displayed name.
- Remove action shows a confirmation dialog before deleting.
- Confirmed remove deletes the student from the database for the current class only.
- Removed student can no longer log in until registering again.
- Canceling remove leaves the student unchanged.
- `dotnet build .\MyClass.slnx` succeeds with no warnings or errors.

## Assumptions

- "Login page as modal form" means a blocking app-level modal shown whenever no valid login state exists.
- "No Login item in Navigation menu" means the nav link is removed entirely, not hidden only after login.
- Student removal is a hard delete, not a soft delete.
- Search is name-only: first name, last name, and display name.
- Stage 2 keeps the current local classroom auth model and does not introduce ASP.NET Core Identity.

## Task Breakdown

- [x] [Task 01 - Login Modal Gate](tasks/01-login-modal-gate.md)
- [x] [Task 02 - Post-Login Redirects](tasks/02-post-login-redirects.md)
- [x] [Task 03 - Student Grid Search](tasks/03-student-grid-search.md)
- [x] [Task 04 - Remove Student](tasks/04-remove-student.md)
- [ ] [Task 05 - Verification](tasks/05-verification.md)
