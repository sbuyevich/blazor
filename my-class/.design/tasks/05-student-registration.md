# Task 05 - Student Registration

## Goal

Allow students to self-register from a separate register page when no matching student record exists.

## Work

- Add a `/register` page using MudBlazor form controls.
- Add a sign-up link on `/login` that leads to `/register`.
- Capture student first name and last name on the registration form.
- Create student records only for the current class.
- Reject duplicate student usernames within the same class.
- Store student credential data securely if credentials include a password or secret.
- Log the student in after successful registration.
- Show clear validation messages for duplicate or invalid registration attempts.

## Deliverables

- Student self-registration UI on `/register` using MudBlazor components
- Sign-up link on `/login`
- Registration support in `IAuthService`
- Student creation logic scoped to the current class
- Validation messages

## Acceptance Criteria

- A new student can register for the current class.
- Registration creates a `Student` record in the app's own database.
- Registration stores first name and last name on the `Student` record.
- Duplicate student username in the same class is rejected.
- Successful registration stores `IsTeacher = false` login state.
- Registration is not possible without a valid current class.

## Notes

- Student post-login capabilities remain `TBD`.
- Current code stores registration login state in `localStorage` through `ILocalStorageService`.
- Current code hashes student passwords with `PasswordHashService` before saving `Student.PasswordHash`.
- Current code rejects duplicate usernames per class through both an application check and the database unique index.
