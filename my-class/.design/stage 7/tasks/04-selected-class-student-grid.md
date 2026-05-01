# Task 04 - Selected-Class Student Grid

## Goal

Show already registered students for the selected class and let the teacher remove students from that grid.

## Work

- Add a selected-class student grid to the School/Class page.
- Load students through `ISchoolClassService` by selected `ClassId`.
- Add a search field that filters by user display name.
- Render student details:
  - display name
  - username
  - active status
  - created date
- Add a single row action: remove student.
- Confirm removal before deleting the student.
- Refresh the grid after removal.
- Show useful empty states when:
  - no class is selected
  - the selected class has no students
  - no students match the display-name search

## Deliverables

- Selected-class student grid on `/school-class`.
- Confirmed remove-student action.

## Acceptance Criteria

- The grid lists only students from the selected class.
- Search filters by `DisplayName`.
- Teacher can remove a student after confirmation.
- Removed student disappears from the grid.
- Removed student cannot log in unless registered again.
- The grid does not include active-only filtering, reset active, or move-student actions.
- The grid does not depend on `ClassContext`.

## Notes

- Do not reuse `StudentGrid` as-is because it is tied to `ClassContext` and has extra actions.
- Removal should match existing behavior: delete the `Students` row.
