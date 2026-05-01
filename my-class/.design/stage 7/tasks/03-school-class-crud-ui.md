# Task 03 - School/Class CRUD UI

## Goal

Build the MudBlazor UI for managing schools and classes on the School/Class page.

## Work

- Load schools and classes through `ISchoolClassService`.
- Render a school selector/list with Add, Edit, and Delete actions.
- Render a class selector/list filtered by selected school with Add, Edit, and Delete actions.
- Use dialogs for:
  - add school
  - edit school
  - add class
  - edit class
- Use confirmation dialogs for:
  - delete school
  - delete class
- Keep the selected school after edits when possible.
- Select the newly created school or class after successful creation.
- Refresh school/class lists after successful create, edit, or delete.
- Surface service validation and delete blocker messages through snackbar or inline alerts.

## Deliverables

- School/Class page CRUD UI.
- Dialog components or page-local dialog content, matching existing MudBlazor style.

## Acceptance Criteria

- School dropdown/list is populated from `Schools`.
- Class dropdown/list is filtered by the selected school.
- Add/edit dialogs validate required values.
- Class add/edit requires both name and code.
- Duplicate class code failure is visible to the teacher.
- Delete school is blocked when classes exist.
- Delete class is blocked when students exist.
- CRUD actions update the displayed lists without requiring a page reload.

## Notes

- Keep UI text concise and action-oriented for classroom use.
- Do not update the app's current class context when selecting or creating a class.
