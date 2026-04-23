# Task 06 - Testing And Validation

## Goal
Verify the MVP behavior end to end and close any gaps against the plan.

## Work
- Build and run the application locally.
- Verify database creation and seeded user setup.
- Test login success and failure cases.
- Test unauthenticated redirect behavior.
- Test create, list, edit, and delete for the logged-in user.
- Test that another user's data cannot be edited or deleted.
- Confirm logout returns the app to a protected state.

## Deliverables
- A checked MVP implementation that matches the plan
- A short verification record or notes for any remaining gaps

## Acceptance Criteria
- App starts and the database is available.
- Seeded default user can log in.
- Invalid login fails cleanly.
- Protected routes require login.
- CRUD works for the current user's projects.
- Ownership restrictions are enforced server-side.
- Logout clears access to protected pages.

## Notes
- If tests are not automated yet, perform a manual verification pass.
- Record any skipped checks or known limitations.
