# Task 10 - Verification

## Goal

Verify Stage 7 School/Class CRUD behavior and confirm it does not disturb existing class-context flows.

## Work

- Build the solution.
- Manually verify access behavior.
- Manually verify school/class CRUD behavior.
- Manually verify delete blockers.
- Manually verify selected-class student search and removal.
- Manually verify current-class context is unchanged by this page.

## Deliverables

- Successful build.
- Manual verification notes.
- Any focused automated tests that fit the current project structure.

## Acceptance Criteria

- `dotnet build .\MyClass.slnx` succeeds after stopping any locked running app processes.
- `/school-class` renders without `?c=...`.
- Teacher can access and use the page.
- Student and unauthenticated users cannot use the page.
- School create/edit/delete works, with delete blocked when classes exist.
- Class create/edit/delete works, with duplicate codes rejected and delete blocked when students exist.
- Selecting a school filters classes.
- Selecting a class loads registered students for that class.
- Display-name search filters the student grid.
- Student removal asks for confirmation, deletes the student, and refreshes the grid.
- Selecting schools/classes does not update session class code, login `ClassCode`, URL `c`, or current classroom context.

## Notes

- Run commands from `my-class/src`.
- If build output files are locked by a running app or Visual Studio, stop those processes before final verification.
