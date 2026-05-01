# Stage 7 School/Class CRUD Plan

## Summary

Create a teacher-only `/school-class` page for managing schools, classes, and already registered students independently from the app's current class context. The page provides CRUD for school/class records, filters classes by selected school, and shows a selected-class student grid where the teacher can search by display name and remove students.

## Key Changes

- Add `/school-class` as a class-context-independent teacher page:
  - Update `MainLayout` so this route can render without requiring a `c` query parameter or `CurrentClass`.
  - Guard the page with existing teacher login state; unauthenticated and non-teacher users see an access message.
  - Add a Teacher nav link for signed-in teachers.
- Add a school/class service in `MyClass.Core` using `IDbContextFactory<ApplicationDbContext>`:
  - List schools with their classes.
  - Create, rename, and delete schools.
  - Create, rename/update code, and delete classes.
  - Reject duplicate class codes after trim/normalization.
  - Allow deleting only empty schools/classes: schools with no classes, classes with no students.
- Build MudBlazor CRUD UI:
  - School selector/list with Add, Edit, and Delete actions.
  - Class selector/list filtered by selected school with Add, Edit, and Delete actions.
  - Dialogs for school name and class name/code.
  - Snackbar or inline messages for validation and delete blockers.
- Add a selected-class student grid:
  - Query students directly by selected `ClassId`, not by current app class context.
  - Search by `DisplayName`.
  - Show registered student details such as display name, username, active status, and created date.
  - Include only one row action: remove student after confirmation.
  - Do not include active-only filtering, reset active, or move-student actions.

## Public Interfaces / Types

- Add `ISchoolClassService` with result models following existing `Succeeded`, `Message`, payload conventions.
- Add service methods for listing schools/classes, CRUD operations, listing selected-class students, and removing a student by `studentId` within `classId`.
- Add lightweight view models for school/class options and student rows.
- No database schema change is required.
- Do not reuse `StudentGrid` as-is because it is tied to `ClassContext` and includes actions not wanted here.

## Test Plan

- `/school-class` renders without `?c=...`.
- Signed-in teacher can access the page.
- Student or unauthenticated user cannot use the page.
- School create/edit/delete works, with delete blocked when classes exist.
- Class create/edit/delete works, with duplicate codes rejected and delete blocked when students exist.
- Selecting a school filters classes.
- Selecting a class loads only students already registered in that class.
- Student search filters by display name.
- Teacher can remove a student from the selected class after confirmation.
- Removed student disappears from the grid and cannot log in unless registered again.
- Selecting schools/classes does not update session class code, login `ClassCode`, URL `c`, or current classroom context.
- `dotnet build .\MyClass.slnx` succeeds.

## Assumptions

- "crude" means CRUD.
- CRUD delete should be conservative and block records with children instead of cascade-deleting students or classes.
- Removing a student means deleting the `Students` row, matching the existing student grid behavior.
- Teacher sign-in remains the existing custom auth flow; this stage does not add a new classless login page.

## Task Breakdown

- [x] [Task 01 - School/Class Service](tasks/01-school-class-service.md)
- [x] [Task 02 - Class-Independent Page Access](tasks/02-class-independent-page-access.md)
- [x] [Task 03 - School/Class CRUD UI](tasks/03-school-class-crud-ui.md)
- [x] [Task 04 - Selected-Class Student Grid](tasks/04-selected-class-student-grid.md)
- [ ] [Task 10 - Verification](tasks/10-verification.md)
