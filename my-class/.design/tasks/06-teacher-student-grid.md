# Task 06 - Teacher Student Grid

## Goal

Add the teacher-only Students menu and student grid for the current class.

## Work

- Add `IStudentService` for student queries scoped to the current class.
- Add a teacher-only Students menu item using MudBlazor navigation components.
- Add `/students` page that uses the stored current class context.
- Render a MudBlazor table/grid of students from the current class.
- Block student users from opening the Students page.
- Show a clear unauthorized message or redirect for non-teacher users.

## Deliverables

- `IStudentService`
- Teacher-only Students menu item
- `/students` page
- Current-class MudBlazor student table/grid
- Non-teacher access handling

## Acceptance Criteria

- Teacher users can see the Students menu.
- Student users cannot see the Students menu.
- Teacher users can open the student grid.
- The grid shows only students from the current class.
- The grid uses MudBlazor table/grid components.
- Student users cannot access the Students page directly.

## Notes

- Teacher feature scope is limited to viewing the student grid for v1.
