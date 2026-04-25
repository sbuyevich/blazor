# Task 10 - Verification

## Goal

Verify stage 2 behavior end to end.

## Work

- Build the solution.
- Run the app with the HTTP launch profile.
- Verify unauthenticated modal gating.
- Verify teacher login redirect.
- Verify student login redirect.
- Verify student registration redirect.
- Verify Students page search.
- Verify confirmed student removal.
- Verify removed student login and re-registration behavior.
- Verify student active state after login.
- Verify current-class active reset.

## Deliverables

- Successful build
- Manual verification notes
- Any focused automated tests that fit the current project structure

## Acceptance Criteria

- `dotnet build .\MyClass.slnx` succeeds with no warnings or errors.
- App opens at `http://localhost:5555/?c=demo`.
- Login modal appears when no user is logged in.
- Login item is absent from navigation.
- Teacher and student login redirect to `/home`.
- Student registration redirects to `/home`.
- Students search filters by name only.
- Student removal requires confirmation.
- Removed student cannot log in until registering again.
- Student login marks that student active.
- Students grid shows active state and no Registered column.
- Reset marks current-class students inactive.
- Student users cannot access or modify the Students page.

## Notes

- Keep verification scoped to stage 2 behavior plus regressions in stage 1 auth/class-context flows.
