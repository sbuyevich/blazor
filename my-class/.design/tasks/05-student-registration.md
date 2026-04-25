# Task 05 - Student Registration

## Goal

Allow students to self-register from the shared login flow when no matching student record exists.

## Work

- Add registration behavior to the login flow using MudBlazor form controls.
- Create student records only for the current class.
- Reject duplicate student usernames within the same class.
- Store student credential data securely if credentials include a password or secret.
- Log the student in after successful registration.
- Show clear validation messages for duplicate or invalid registration attempts.

## Deliverables

- Student self-registration UI in the login flow using MudBlazor components
- Registration support in `IAuthService`
- Student creation logic scoped to the current class
- Validation messages

## Acceptance Criteria

- A new student can register for the current class.
- Registration creates a `Student` record in the app's own database.
- Duplicate student username in the same class is rejected.
- Successful registration stores `IsTeacher = false` login state.
- Registration is not possible without a valid current class.

## Notes

- Student post-login capabilities remain `TBD`.
