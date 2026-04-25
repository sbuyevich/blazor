# Task 06 - Teacher Student Grid

## Goal

Add the teacher-only Students menu and student grid for the current class.

## Work

- Add `IStudentService` for student queries scoped to the current class.
- Finish the existing teacher-only Teacher section/divider in the MudBlazor nav menu as needed.
- Keep the existing Students menu item under the Teacher section.
- Replace the existing `/students` placeholder page with a current-class student grid.
- Render a MudBlazor table/grid of students from the current class.
- Block student users from opening the Students page.
- Show a clear unauthorized message or redirect for non-teacher users.

## Deliverables

- `IStudentService`
- Teacher-only Teacher menu section
- Teacher-only Students menu item under that section
- `/students` page
- Current-class MudBlazor student table/grid
- Non-teacher access handling

## Acceptance Criteria

- Teacher users can see the Teacher menu section and Students menu item.
- Student users cannot see the Teacher menu section or Students menu item.
- Teacher users can open the student grid.
- The grid shows only students from the current class.
- The grid uses MudBlazor table/grid components.
- Student users cannot access the Students page directly.

## Notes

- Teacher feature scope is limited to viewing the student grid for v1.
- Current code already has a Teacher nav section gated by `LoginStateService.Current?.IsTeacher == true`.
- Current code already has a `/students` page, but it only shows an informational placeholder.
- `IStudentService` does not exist yet.
