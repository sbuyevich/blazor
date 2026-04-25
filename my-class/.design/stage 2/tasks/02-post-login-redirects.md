# Task 02 - Post-Login Redirects

## Goal

Redirect users to `/home` after successful login or registration.

## Work

- On successful teacher login, store login state and navigate to `/home`.
- On successful student login, store login state and navigate to `/home`.
- On successful student registration, store student login state and navigate to `/home`.
- Leave invalid login and invalid registration attempts on the modal/form with clear validation messages.
- Ensure redirect behavior works when the app was opened from `/?c=demo`.

## Deliverables

- Teacher login redirects to `/home`
- Student login redirects to `/home`
- Student registration redirects to `/home`
- Failed attempts remain visible with error messages

## Acceptance Criteria

- Teacher login stores `IsTeacher = true` and lands on `/home`.
- Student login stores `IsTeacher = false` and lands on `/home`.
- Student registration stores `IsTeacher = false` and lands on `/home`.
- Invalid credentials do not redirect.
- Duplicate student registration does not redirect.

## Notes

- Keep using the existing `LoginStateService` and `ISessionStorageService`.
- Do not change the stored login state shape.
