# Task 06 - Teacher Student Grid

## Goal

Add the teacher-only Students menu and student grid for the current class.

## Work

- Add `IStudentService` for student queries scoped to the current class.
- Add a teacher-only Students menu item.
- Add `/students?c={code}` page.
- Render a grid of students from the current class.
- Block student users from opening the Students page.
- Show a clear unauthorized message or redirect for non-teacher users.

## Deliverables

- `IStudentService`
- Teacher-only Students menu item
- `/students` page
- Current-class student grid
- Non-teacher access handling

## Acceptance Criteria

- Teacher users can see the Students menu.
- Student users cannot see the Students menu.
- Teacher users can open the student grid.
- The grid shows only students from the current class.
- Student users cannot access the Students page directly.

## Notes

- Teacher feature scope is limited to viewing the student grid for v1.
