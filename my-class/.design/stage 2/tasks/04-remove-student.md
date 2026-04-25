# Task 04 - Remove Student

## Goal

Allow teachers to remove a student from the current class after confirmation.

## Work

- Add a row-level remove action to the Students grid.
- Show a MudBlazor confirmation dialog before deleting.
- Permanently delete the selected `Student` row from the database.
- Validate that the current user is a teacher.
- Validate that the selected student belongs to the current class.
- Refresh the grid after successful removal.
- Show a clear error message if removal fails.

## Deliverables

- Remove action on each student row
- Confirmation dialog
- Teacher-only remove method in `IStudentService`
- Current-class scoped hard delete
- Grid refresh and success/error feedback

## Acceptance Criteria

- Teacher can remove a student after confirming.
- Canceling confirmation leaves the student unchanged.
- Removed student disappears from the grid.
- Removed student can no longer log in.
- Removed student can register again for the current class.
- A teacher cannot remove a student from another class.
- Student users cannot remove students.

## Notes

- Removal is a hard delete, not a soft delete.
- This is intentionally a teacher-only classroom management action.
