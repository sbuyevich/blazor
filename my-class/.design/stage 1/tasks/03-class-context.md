# Task 03 - Class Context

## Goal

Resolve the current school and class from the landing page URL and make it available to later pages.

## Work

- Use `c` as the class-code query parameter on the landing page only.
- Add `IClassContextService` to load the current class and school from the database.
- Add a shared class context model for the resolved school, class, and code.
- Update layout/header UI to show school and class names when a valid class is loaded.
- Add missing-class-code handling.
- Add invalid-class-code handling.
- Store the resolved class context so `/login` and `/students` do not require `c`.
- Do not preserve the class code in navigation links after landing page resolution.

## Deliverables

- `IClassContextService`
- Class context model
- Header display for school and class names
- Error UI for missing or invalid `c`
- Routes that use stored class context after `/?c={code}` loads

## Acceptance Criteria

- `/?c={code}` loads the matching class.
- Missing `c` shows a clear error.
- Invalid `c` shows a clear error.
- Keep class as static var in app to use it in other pages
- `/login` loads the stored current class without `c`.
- `/students` loads the stored current class without `c`.

## Notes

- Teacher and student actions are always scoped to the resolved current class.
