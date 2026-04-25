# Task 05 - Student Active Reset

## Goal

Track whether students are active in the current class session and let the teacher reset all current-class students to inactive.

## Work

- Add a boolean `IsActive` field to the `Student` entity/table.
- Default new and existing students to `IsActive = false`.
- Set `IsActive = true` after a successful student login.
- Keep teacher login from changing student active state.
- Add a teacher-only Reset button on the Students page.
- Hide the existing Registered column on the Students grid.
- Add an `IsActive` column on the Students grid.
- Reset sets `IsActive = false` for all students in the current class only.
- Refresh the Students grid after reset.
- Show clear success/error feedback after reset.

## Deliverables

- `Student.IsActive` data model field
- Database setup/update support for the new field
- Student login active-state update
- Teacher-only current-class reset method in `IStudentService`
- Reset button on Students page
- Students grid shows `IsActive` instead of Registered
- Grid refresh and feedback after reset

## Acceptance Criteria

- `Students` table has an `IsActive` boolean field.
- Existing students are inactive after the field is added.
- New registered students start inactive until they log in.
- Successful student login sets that student's `IsActive` to true.
- Teacher login does not change any student active state.
- Reset button is visible only to teacher users who can access the Students page.
- Reset sets `IsActive = false` only for students in the current class.
- Reset does not affect students from another class.
- Students grid does not show the Registered column.
- Students grid shows each student's active state.
- Students grid refreshes after reset.
- Build succeeds with no warnings or errors.

## Notes

- This task tracks classroom session activity, not account enablement.
- Do not use `IsActive` to block student login unless a future requirement says so.
