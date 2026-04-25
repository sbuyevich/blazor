# Task 07 - Verification

## Goal

Verify the MVP works end to end as a separate local classroom app.

## Work

- Build the `my-class` solution.
- Run the app locally.
- Verify database creation or migration.
- Verify valid, missing, and invalid class-code flows.
- Verify teacher login and logout.
- Verify student login, registration, and duplicate handling.
- Verify teacher-only Students navigation and direct route protection.
- Confirm no dependency on `student-projects` database or configuration.

## Deliverables

- Passing build
- Manual verification notes
- Any focused automated tests that fit the final project structure

## Acceptance Criteria

- The app builds successfully.
- The app starts with its own SQLite database.
- Valid landing page `?c={code}` classroom flow works.
- `/login` and `/students` work from the stored current class context without `c`.
- Missing and invalid `c` values show clear errors.
- Teacher auth flow works.
- Student auth and registration flows work.
- Students page is teacher-only and current-class scoped.
- No `student-projects` database, migrations, or settings are used.

## Notes

- Add automated tests where they are practical after the implementation shape is known.
