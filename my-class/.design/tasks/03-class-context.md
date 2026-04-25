# Task 03 - Class Context

## Goal

Resolve the current school and class from the URL and show a clear error when the class cannot be loaded.

## Work

- Use `c` as the class-code query parameter.
- Add `IClassContextService` to load the current class and school from the database.
- Add a shared class context model for the resolved school, class, and code.
- Update layout/header UI to show school and class names when a valid class is loaded.
- Add missing-class-code handling.
- Add invalid-class-code handling.
- Preserve the class code in navigation links, including login and students pages.

## Deliverables

- `IClassContextService`
- Class context model
- Header display for school and class names
- Error UI for missing or invalid `c`
- Routes that preserve `?c={code}`

## Acceptance Criteria

- `/?c={code}` loads the matching class.
- `/login?c={code}` loads the matching class.
- `/students?c={code}` loads the matching class.
- Missing `c` shows a clear error.
- Invalid `c` shows a clear error.
- Navigation does not drop the class code.

## Notes

- Teacher and student actions are always scoped to the resolved current class.
