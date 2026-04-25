# Task 03 - Student Grid Search

## Goal

Add name search to the teacher Students page.

## Work

- Add a search field above the Students grid.
- Filter students by first name, last name, and display name.
- Do not search by username unless the username also appears in a displayed name.
- Keep results scoped to the current class.
- Keep the page teacher-only.
- Show a friendly empty state when no students match the search.

## Deliverables

- Search input above the grid
- Current-class student filtering
- Empty state for no matching students
- Updated `IStudentService` query support if filtering is performed server-side

## Acceptance Criteria

- Teacher can type a search term and see matching students.
- Search matches first name.
- Search matches last name.
- Search matches display name.
- Search does not match username-only values.
- Search never returns students from another class.
- Clearing search restores the full current-class list.

## Notes

- Client-side filtering is acceptable for the MVP if the grid already has the current-class student list.
- Server-side filtering is acceptable if it keeps component logic simpler.
